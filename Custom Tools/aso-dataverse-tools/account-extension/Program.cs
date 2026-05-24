using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

const string DataverseUrl = "https://phoenicarix-ci.crm4.dynamics.com";
const string SolutionUniqueName = "ASOCore";
const string EntityLogicalName = "account";
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

var existingAttributes = GetExistingAttributeLogicalNames(service, EntityLogicalName);

var fields = new List<AttributeMetadata>
{
    Text("aso_sapbusinesspartnerid", "SAP Business Partner ID", 100, "SAP Business Partner reference."),
    Text("aso_sapcustomerid", "SAP Customer ID", 100, "SAP Customer reference."),

    Text("aso_aiaccounthealth", "AI Account Health", 100, "AI account health. Kept as text in MVP because taxonomy may evolve."),
    Memo("aso_aigrowthsummary", "AI Growth Summary", "AI growth summary."),
    Text("aso_airenewalrisk", "AI Renewal Risk", 100, "AI renewal risk. Kept as text in MVP because taxonomy may evolve."),
    DateTimeField("aso_ailastrunon", "AI Last Run On", "Last AI run timestamp."),
    Memo("aso_aisapaccountrichcontext", "AI SAP Account Rich Context", "SAP account rich context."),
    Text("aso_aicorrelationid", "AI Correlation ID", 100, "Last run correlation."),

    GlobalChoice("aso_communicationstate", "Communication State", "aso_communicationstate", "Uses global Communication State choice."),
    GlobalChoice("aso_lifecyclecommunicationstage", "Lifecycle Communication Stage", "aso_lifecyclecommunicationstage", "Uses global Lifecycle Communication Stage choice."),
    GlobalChoice("aso_journeyparticipationstatus", "Journey Participation Status", "aso_journeyparticipationstatus", "Uses global Journey Participation Status choice."),

    Text("aso_customerinsightsjourneyid", "Customer Insights Journey ID", 100, "Latest journey reference."),
    Text("aso_customerinsightsjourneyname", "Customer Insights Journey Name", 200, "Latest journey name."),
    DateTimeField("aso_customerinsightslastentryon", "Customer Insights Last Entry On", "Last entry."),
    DateTimeField("aso_customerinsightslastinteractionon", "Customer Insights Last Interaction On", "Last interaction."),

    LocalChoice("aso_customerinsightslastinteractiontype", "Customer Insights Last Interaction Type",
        new[] { "EmailSent", "Open", "Click", "FormSubmit", "Reply", "Unsubscribe", "Bounce", "CustomAction" },
        "Customer Insights last interaction type."),

    GlobalChoice("aso_emailconsentstatus", "Email Consent Status", "aso_emailconsentstatus", "Uses global Email Consent Status choice."),
    Text("aso_complianceprofilename", "Compliance Profile Name", 200, "Profile used."),
    Memo("aso_communicationholdreason", "Communication Hold Reason", "Suppression / hold reason.")
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
Console.WriteLine("Publishing Account table customizations...");

service.Execute(new PublishXmlRequest
{
    ParameterXml = "<importexportxml><entities><entity>account</entity></entities></importexportxml>"
});

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Done. Validate in ASO.Core → Tables → Account → Columns.");
Console.ResetColor();

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

static MemoAttributeMetadata Memo(string schemaName, string displayName, string description)
{
    return new MemoAttributeMetadata
    {
        SchemaName = schemaName,
        DisplayName = Label(displayName),
        Description = Label(description),
        RequiredLevel = Optional(),
        MaxLength = 4000
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