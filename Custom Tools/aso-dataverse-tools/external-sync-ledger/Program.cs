using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

const string DataverseUrl = "https://phoenicarix-ci.crm4.dynamics.com";
const string SolutionUniqueName = "ASOCore";
const string EntityLogicalName = "aso_externalsyncledger";
const int LanguageCode = 1033;

var connectionString =
    $@"AuthType=OAuth;
       Url={DataverseUrl};
       LoginPrompt=Auto;
       ClientId=51f81489-12ee-4a9e-aaae-a2591f45987d;
       RedirectUri=http://localhost";

using var service = new ServiceClient(connectionString);

if (!service.IsReady)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Connection failed:");
    Console.WriteLine(service.LastError);
    Console.ResetColor();
    return;
}

Console.WriteLine($"Connected to: {DataverseUrl}");
Console.WriteLine($"Target solution: {SolutionUniqueName}");
Console.WriteLine($"Target table: {EntityLogicalName}");
Console.WriteLine();

if (!EntityExists(service, EntityLogicalName))
{
    Console.WriteLine("Creating External Sync Ledger table...");

    var createEntityRequest = new CreateEntityRequest
    {
        Entity = new EntityMetadata
        {
            SchemaName = "aso_externalsyncledger",
            DisplayName = Label("External Sync Ledger"),
            DisplayCollectionName = Label("External Sync Ledgers"),
            Description = Label("Tracks external IDs, hashes, processed times, and status across ingress and integration systems."),
            OwnershipType = OwnershipTypes.OrganizationOwned
        },
        PrimaryAttribute = new StringAttributeMetadata
        {
            SchemaName = "aso_name",
            DisplayName = Label("Name"),
            Description = Label("Primary name for the External Sync Ledger record."),
            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.ApplicationRequired),
            MaxLength = 200
        },
        SolutionUniqueName = SolutionUniqueName
    };

    service.Execute(createEntityRequest);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("CREATED TABLE: aso_externalsyncledger");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("SKIP existing table: aso_externalsyncledger");
    Console.ResetColor();
}

var existingAttributes = GetExistingAttributeLogicalNames(service, EntityLogicalName);

var fields = new List<AttributeMetadata>
{
    LocalChoice("aso_sourcesystem", "Source System",
        new[] { "HubSpot", "SAP", "PowerAutomate", "Foundry", "CustomerInsights" },
        "External or integration system that produced or owns the synchronized identifier."),

    Text("aso_externalentity", "External Entity", 100, "External entity or object name, for example contact, lead, business partner, or journey."),
    Text("aso_externalid", "External ID", 200, "External system identifier."),
    Text("aso_dataverseentity", "Dataverse Entity", 100, "Dataverse table/entity logical or business name."),
    Text("aso_dataverseid", "Dataverse ID", 100, "Dataverse row identifier."),
    Text("aso_lasthash", "Last Hash", 200, "Last processed hash used for idempotency and change detection."),
    DateTimeField("aso_lastprocessedon", "Last Processed On", "Timestamp of the latest successful or attempted processing."),

    GlobalChoice("aso_status", "Status", "aso_aistatus", "Processing status. Uses existing ASO AI Status global choice for MVP consistency."),

    Text("aso_correlationid", "Correlation ID", 100, "Correlation ID shared across integration, automation, and orchestration components.")
};

foreach (var field in fields)
{
    var logicalName = field.SchemaName!.ToLowerInvariant();

    if (existingAttributes.Contains(logicalName))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"SKIP existing: {logicalName}");
        Console.ResetColor();
        continue;
    }

    try
    {
        Console.WriteLine($"Creating: {logicalName} ...");

        var request = new CreateAttributeRequest
        {
            EntityName = EntityLogicalName,
            Attribute = field,
            SolutionUniqueName = SolutionUniqueName
        };

        service.Execute(request);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"CREATED: {logicalName}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"FAILED: {logicalName}");
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}

Console.WriteLine();
Console.WriteLine("Publishing External Sync Ledger table customizations...");

service.Execute(new PublishXmlRequest
{
    ParameterXml = "<importexportxml><entities><entity>aso_externalsyncledger</entity></entities></importexportxml>"
});

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Done. Validate in ASO.Core → Tables → External Sync Ledger → Columns.");
Console.ResetColor();

static bool EntityExists(ServiceClient service, string entityLogicalName)
{
    try
    {
        service.Execute(new RetrieveEntityRequest
        {
            LogicalName = entityLogicalName,
            EntityFilters = EntityFilters.Entity,
            RetrieveAsIfPublished = true
        });

        return true;
    }
    catch
    {
        return false;
    }
}

static HashSet<string> GetExistingAttributeLogicalNames(ServiceClient service, string entityLogicalName)
{
    var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
    {
        LogicalName = entityLogicalName,
        EntityFilters = EntityFilters.Attributes,
        RetrieveAsIfPublished = true
    });

    return response.EntityMetadata.Attributes
        .Where(a => !string.IsNullOrWhiteSpace(a.LogicalName))
        .Select(a => a.LogicalName!)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
}

static Label Label(string value) => new(value, LanguageCode);

static AttributeRequiredLevelManagedProperty Optional()
{
    return new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);
}

static StringAttributeMetadata Text(string schemaName, string displayName, int maxLength, string description)
{
    return new StringAttributeMetadata
    {
        SchemaName = schemaName,
        DisplayName = Label(displayName),
        Description = Label(description),
        RequiredLevel = Optional(),
        MaxLength = maxLength
    };
}

static DateTimeAttributeMetadata DateTimeField(string schemaName, string displayName, string description)
{
    return new DateTimeAttributeMetadata
    {
        SchemaName = schemaName,
        DisplayName = Label(displayName),
        Description = Label(description),
        RequiredLevel = Optional(),
        Format = DateTimeFormat.DateAndTime,
        DateTimeBehavior = DateTimeBehavior.UserLocal
    };
}

static PicklistAttributeMetadata LocalChoice(string schemaName, string displayName, string[] values, string description)
{
    var optionSet = new OptionSetMetadata
    {
        IsGlobal = false,
        OptionSetType = OptionSetType.Picklist
    };

    foreach (var value in values)
    {
        optionSet.Options.Add(new OptionMetadata(Label(value), null));
    }

    return new PicklistAttributeMetadata
    {
        SchemaName = schemaName,
        DisplayName = Label(displayName),
        Description = Label(description),
        RequiredLevel = Optional(),
        OptionSet = optionSet
    };
}

static PicklistAttributeMetadata GlobalChoice(string schemaName, string displayName, string globalChoiceName, string description)
{
    return new PicklistAttributeMetadata
    {
        SchemaName = schemaName,
        DisplayName = Label(displayName),
        Description = Label(description),
        RequiredLevel = Optional(),
        OptionSet = new OptionSetMetadata
        {
            IsGlobal = true,
            Name = globalChoiceName,
            OptionSetType = OptionSetType.Picklist
        }
    };
}