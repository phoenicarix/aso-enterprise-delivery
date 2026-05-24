# ASO Dataverse Tools — README

**Project:** Agentic Sales Orchestrator  
**Environment:** Phoenicarix-CI  
**Dataverse URL:** `https://phoenicarix-ci.crm4.dynamics.com`  
**Solution:** `ASO.Core`  
**Solution unique name used by scripts:** `ASOCore`  
**Local tools folder:** `~/aso-dataverse-tools`  
**Date:** 2026-05-24

---

## 1. Purpose of this folder

This folder contains the local .NET / C# Dataverse tooling scripts used to create and extend the Agentic Sales Orchestrator Phase 2 Dataverse schema.

The scripts were created because many Dataverse columns and custom tables had to be created in a controlled, repeatable way. Instead of manually clicking through every field in Power Apps, we used small .NET console applications to create the schema programmatically through the Microsoft Dataverse SDK.

In simple terms: these tools created the structured “boxes” in Dynamics 365 / Dataverse where future AI, orchestration, SAP, HubSpot, Customer Insights, and governance information can be stored safely.

---

## 2. Important architecture context

The scripts target the **Phoenicarix-CI** environment, not the older Phoenicarix-Sales trial environment.

The schema was created inside the **ASO.Core** solution using the technical solution name:

```text
ASOCore
```

The scripts use the `aso_` publisher prefix and create metadata only. They do not activate flows, call SAP, send Customer Insights journeys, send emails, or start production automation.

---

## 3. Tools created

| Folder | Target component | Target Dataverse logical name | Purpose | Status |
|---|---|---:|---|---|
| `lead-extension` | Existing Lead table | `lead` | Adds ASO AI, SAP reference, HubSpot reference, Customer Insights, consent, and traceability columns to Lead. | Completed |
| `opportunity-extension` | Existing Opportunity table | `opportunity` | Adds ASO AI opportunity intelligence, SAP commercial context, Sales Agent, Foundry, Customer Insights, and governance columns to Opportunity. | Completed |
| `account-extension` | Existing Account table | `account` | Adds SAP reference, AI account health, growth, renewal, Customer Insights, consent, and governance columns to Account. | Completed |
| `contact-extension` | Existing Contact table | `contact` | Adds Customer Insights, consent, lifecycle, communication-state, preferred email, and preference-center tracking columns to Contact. | Completed |
| `agent-run-log` | New custom table | `aso_agentrunlog` | Creates an audit/trace table for AI/agent/orchestration runs, payloads, confidence, status, correlation IDs, and observability references. | Completed |
| `external-sync-ledger` | New custom table | `aso_externalsyncledger` | Creates an external-system sync ledger for HubSpot, SAP, Foundry, Power Automate, and Customer Insights identifiers and idempotency tracking. | Completed |
| `pending-commercial-action` | New custom table | `aso_pendingcommercialaction` | Creates a staging table for approval-gated commercial/SAP actions before governed SAP submission. | Completed |
| `journey-participation-ledger` | New custom table | `aso_journeyparticipationledger` | Creates a ledger for Customer Insights journey participation and interaction tracking. | Completed |

---

## 4. Special note: Pending Commercial Action relationship fix

During the first run of the `pending-commercial-action` script, the table and most columns were created successfully, but the Opportunity lookup relationship initially failed with this issue:

```text
The language code 1033 is not a valid language for this organization
```

The cause was that the relationship/associated-menu label used language code `1033` for English. The organization required a valid installed language for that label operation. The fix script used language code `1031` for German.

The missing relationship was then created successfully:

```text
aso_opportunity_aso_pendingcommercialaction
```

This relationship links:

```text
Opportunity → Pending Commercial Action
```

Meaning each Pending Commercial Action can be related to one Opportunity.

A zip file was also stored in the `pending-commercial-action` folder for the lookup-fix script:

```text
pending-commercial-action - opportunity lookup fix script only.zip
```

---

## 5. Common run pattern

Each tool generally followed this pattern:

```bash
mkdir -p ~/aso-dataverse-tools/<tool-folder>
cd ~/aso-dataverse-tools/<tool-folder>
dotnet new console --framework net8.0
dotnet add package Microsoft.PowerPlatform.Dataverse.Client
# Replace Program.cs with the provided script
dotnet run
```

Each `Program.cs` script then:

1. Connects to Dataverse using OAuth.
2. Targets `https://phoenicarix-ci.crm4.dynamics.com`.
3. Uses solution unique name `ASOCore`.
4. Checks whether the target table or columns already exist.
5. Creates missing tables/columns/relationships.
6. Writes clear terminal messages.
7. Publishes the relevant table customizations.
8. Requires validation in Power Apps.

---

## 6. Authentication pattern

The scripts use an interactive OAuth Dataverse connection:

```csharp
AuthType=OAuth;
Url=https://phoenicarix-ci.crm4.dynamics.com;
LoginPrompt=Auto;
ClientId=51f81489-12ee-4a9e-aaae-a2591f45987d;
RedirectUri=http://localhost
```

This means the script runs locally from the Mac and prompts the authenticated maker/admin user to sign in when needed.

---

## 7. Required local prerequisites

Before running any script, the Mac should have:

| Prerequisite | Purpose |
|---|---|
| .NET SDK 8 | Required to create and run the console applications. |
| Microsoft.PowerPlatform.Dataverse.Client NuGet package | Required for Dataverse SDK access. |
| Valid Power Apps / Dataverse user | Required for interactive OAuth login. |
| Access to Phoenicarix-CI | Required so the scripts target the correct environment. |
| Permissions to customize Dataverse | Required to create tables, columns, keys, and relationships. |
| ASO.Core solution | Required so components are created inside the correct solution layer. |

---

## 8. Validation path in Power Apps

After each script run, validate in Power Apps:

```text
make.powerapps.com
→ Environment: Phoenicarix-CI
→ Solutions
→ ASO.Core
→ Tables
→ <Target table>
→ Columns / Relationships / Keys
```

Search for:

```text
aso_
```

This confirms that the components were created with the correct publisher prefix.

---

## 9. Backup pattern used

After major schema milestones, ASO.Core was exported as both unmanaged and managed.

Recommended naming pattern:

```text
<SolutionName>_<Phase>_<Environment>_<Scope>_<managed|unmanaged>_<version>_<date>.zip
```

Examples:

```text
ASO.Core_Phase2_CI_LeadExtension_unmanaged_v1_0_20260523.zip
ASO.Core_Phase2_CI_LeadExtension_managed_v1_0_20260523.zip

ASO.Core_Phase2_CI_OpportunityExtension_unmanaged_v1_0_20260523.zip
ASO.Core_Phase2_CI_OpportunityExtension_managed_v1_0_20260523.zip

ASO.Core_Phase2_CI_AccountExtension_unmanaged_v1_0_20260523.zip
ASO.Core_Phase2_CI_AccountExtension_managed_v1_0_20260523.zip

ASO.Core_Phase2_CI_ContactExtension_unmanaged_v1_0_20260523.zip
ASO.Core_Phase2_CI_ContactExtension_managed_v1_0_20260523.zip

ASO.Core_Phase2_CI_AgentRunLogCreated_unmanaged_v1_0_20260524.zip
ASO.Core_Phase2_CI_AgentRunLogCreated_managed_v1_0_20260524.zip

ASO.Core_Phase2_CI_ExternalSyncLedgerCreated_unmanaged_v1_0_20260524.zip
ASO.Core_Phase2_CI_ExternalSyncLedgerCreated_managed_v1_0_20260524.zip

ASO.Core_Phase2_CI_PendingCommercialActionCreated_unmanaged_v1_0_20260524.zip
ASO.Core_Phase2_CI_PendingCommercialActionCreated_managed_v1_0_20260524.zip

ASO.Core_Phase2_CI_JourneyParticipationLedgerCreated_unmanaged_v1_0_20260524.zip
ASO.Core_Phase2_CI_JourneyParticipationLedgerCreated_managed_v1_0_20260524.zip
```

---

## 10. Alternate key work completed

An alternate key was created on **External Sync Ledger** for external-system identity control.

Recommended / created key:

```text
External Sync Ledger Source Entity ID
```

Columns:

```text
Source System
External Entity
External ID
```

Deferred MVP keys documented separately:

```text
Lead → AK_Lead_HubSpotContactId on aso_hubspotcontactid
Account → AK_Account_SAPCustomerId on aso_sapcustomerid
```

These were deferred because real HubSpot/SAP uniqueness and sample-data behavior must be confirmed before enforcing uniqueness constraints.

---

## 11. Duplicate detection decision

Duplicate detection rules were reviewed but deferred for MVP.

Reason: the most important integration idempotency need is already covered through the External Sync Ledger alternate key. Business-facing duplicate detection rules for Lead, Account, Contact, and Opportunity require realistic sample data and agreed matching policies.

---

## 12. Generated documentation packages

Customer-ready documentation was created for the major scripts and schema changes, including line-by-line code explanations.

Generated documentation packages include:

| Area | Documentation type |
|---|---|
| Lead extension | Programmatic runbook + script explanation |
| Opportunity extension | Programmatic runbook + true line-by-line explanation |
| Account + Contact extension | Combined programmatic runbook + line-by-line explanation |
| Agent Run Log | Programmatic runbook + line-by-line explanation |
| External Sync Ledger | Programmatic runbook + line-by-line explanation |
| Pending Commercial Action | Main runbook + Opportunity lookup fix runbook |
| Journey Participation Ledger | Main runbook; relationship-fix addendum should be ignored because no actual fix was needed |

---

## 13. What not to do with these tools

Do not treat these scripts as production deployment pipelines yet.

Do not run them against another environment without changing and reviewing:

```csharp
DataverseUrl
SolutionUniqueName
EntityLogicalName
LanguageCode
```

Do not use these scripts to send customer communications.

Do not use these scripts to call SAP.

Do not assume the local choice values are final production taxonomies without business validation.

Do not delete generated Dataverse columns casually, because deleting schema can break forms, flows, views, solutions, and historical data.

---

## 14. Recommended next implementation step

The next implementation step after schema creation, alternate-key decisioning, and duplicate-detection review is:

```text
Step 12 — Update model-driven forms
```

Recommended sequence:

1. Lead form
2. Opportunity form
3. Account form
4. Contact form

Suggested form tabs:

```text
AI & Orchestration
Customer Insights & Governance
```

The goal is to make the created fields visible in a clean, role-friendly layout without mixing AI/governance fields into standard sales-entry sections.

---

## 15. Quick health checklist

Before moving further, confirm:

| Check | Expected |
|---|---|
| Environment | Phoenicarix-CI |
| Solution | ASO.Core |
| Lead columns | Created |
| Opportunity columns | Created |
| Account columns | Created |
| Contact columns | Created |
| Agent Run Log table | Created |
| External Sync Ledger table | Created |
| Pending Commercial Action table | Created |
| Pending Commercial Action → Opportunity relationship | Created |
| Journey Participation Ledger table | Created |
| External Sync Ledger alternate key | Created |
| Managed + unmanaged backups | Exported after each major milestone |
| Duplicate detection | Reviewed and deferred for MVP |
| Form updates | Next step |

---

## 16. Recommended source control note

The folder `~/aso-dataverse-tools` should be treated as implementation tooling. It is recommended to move it into source control later, for example:

```text
repo/
  dataverse-tools/
    lead-extension/
    opportunity-extension/
    account-extension/
    contact-extension/
    agent-run-log/
    external-sync-ledger/
    pending-commercial-action/
    journey-participation-ledger/
```

Do not commit secrets. The current scripts use interactive login and do not store a password.

---

## 17. Owner notes

Implementation context:

```text
Project: Agentic Sales Orchestrator
Phase: Phase 2 — Dataverse Schema + Sales Form MVP
Environment: Phoenicarix-CI
Solution: ASO.Core
Primary local tools path: ~/aso-dataverse-tools
```

This README is intended as the first page for anyone opening the tools folder later and asking: “What are these scripts, why do they exist, and what did they change?”

