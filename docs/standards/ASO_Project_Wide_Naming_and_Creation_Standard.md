# ASO Project-Wide Naming and Creation Standard

Enterprise naming rules for Dataverse, Power Platform, Customer Insights, Foundry, Azure, SAP, HubSpot, views, fields, flows, variables, and connection references.

> GitHub path: `docs/standards/ASO_Project_Wide_Naming_and_Creation_Standard.md`

> Public safety: do not publish tenant IDs, credentials, secrets, real customer data, private endpoints, or connection strings.

## Project-wide naming and creation standard

This is the ASO naming and creation contract. It is written for makers, architects, developers, testers, and operators before they create Dataverse schema, solution assets, flows, views, journeys, Azure resources, agents, API routes, or GitHub files.

> **Note:** Golden rule: schema names are not cosmetic. They become technical contracts across Dataverse, flows, integrations, tests, dashboards, and documentation.

### Universal naming principles

| Rule | Standard | Example |
| --- | --- | --- |
| Publisher prefix | Use exactly one project-owned publisher prefix for all Dataverse custom components. | aso |
| Language | Use English technical names even where the user interface is localized. | Communication State |
| Schema permanence | Review schema names before creation; treat them as immutable technical contracts. | aso_foundryreviewrequired |
| No random prefixes | Never use new_, personal prefixes, or mixed prefixes. | Reject new_aiagentstatus |
| Environment-independent names | Do not put environment names inside Dataverse schema, flows, or CI journey keys unless the asset is environment-specific by design. | Use environment variables instead of DevFoundryUrl |
| Acronyms | Use approved acronyms only: ASO, AI, API, APIM, SAP, CIJ, ERP, CRM. | Do not invent MSFND or CIS |
| Searchability | Start operational names with the area or audience so lists group naturally. | ASO - Lead - Dispatch to Foundry |
| Business clarity | Display names should be readable; schema names should be stable and unambiguous. | Display: AI SAP Match Flag; schema: aso_aisapmatchflag |

### Naming style by component family

| Component family | Pattern | Example | Why |
| --- | --- | --- | --- |
| Solutions | ASO.<Layer> | ASO.Core, ASO.Automation, ASO.Operations | Groups components by ownership and lifecycle. |
| Dataverse custom tables | Display: singular business noun; schema: aso_<singularcompactnoun> | Agent Run Log / aso_agentrunlog | Stable table contract for integrations, logs, and ALM. |
| Dataverse columns | Display: business label; schema: aso_<lowercasecompactmeaning> | Foundry Review Required / aso_foundryreviewrequired | Avoids casing drift and later flow mapping mistakes. |
| Global choices | aso_<Concept> using PascalCase concept | aso_AIStatus | Reusable vocabulary across tables. |
| Environment variables | aso_<PascalCaseSettingName> | aso_FoundryOrchestratorUrl | Clear configuration contract consumed by flows and deployment settings. |
| Connection references | ASO <Connector/Purpose> Connection Reference | ASO Dataverse Connection Reference | Readable in solution-aware flow editing and import settings. |
| Views | <EntityPlural> - <Purpose> or <Audience> - <EntityPlural> - <Filter> | Leads - AI Review Required | Supports user adoption and avoids duplicate vague views. |
| Forms / tabs / sections | Business Title Case | AI & Orchestration, SAP Context | User-facing navigation clarity. |
| Flows | ASO - <Entity/Area> - <Trigger/Intent> - <Outcome> | ASO - Lead - Dispatch to Foundry | Operational traceability and support triage. |
| Customer Insights journeys | JRNY - ASO - <Lifecycle> - v<major> | JRNY - ASO - Lead Nurture - v1 | Release-aware journey catalogue. |
| Azure resources | <type>-<workload>-<purpose?>-<env>-<region>-<###> | func-aso-sap-dev-weu-001 | Aligns with cloud resource naming best practices. |
| API routes | Lowercase kebab path with version | /aso/sap/v1/customer-profile | Stable API contract with version boundary. |
| Foundry code files | lower_snake_case | risk_competitor.py | Repository readability and code convention alignment. |
| JSON contract fields | lower_snake_case | correlation_id, requires_human_review | Language-neutral API payload readability. |

### Field naming by data type

| Data type | Display name rule | Schema name rule | ASO example | Do not use |
| --- | --- | --- | --- | --- |
| Text identifier | End display name with ID for external IDs; schema ends with id. | aso_<system><entity>id | SAP Customer ID / aso_sapcustomerid | aso_sap_customer_id, SAPIdText |
| Business text | Use a clear business noun. | aso_<businessmeaning> | HubSpot Source / aso_hubspotsource | aso_text1, aso_notes2 |
| Multiline text | Use Summary, Rationale, Reason, Payload, Message, or Guidance. | aso_<concept><purpose> | AI Qualification Rationale / aso_aiqualificationrationale | aso_bigtext, aso_comments |
| Whole number | Use Score, Count, Days, Retry Count, or business numeric meaning. | aso_<concept><numericmeaning> | AI Lead Score / aso_aileadscore | aso_number1 |
| Decimal | Use for confidence, probability-like values, or measured decimals. | aso_<concept> | AI Confidence / aso_aiconfidence | aso_decimalvalue |
| Currency | Use Amount, Revenue, Value, or local business term. | aso_<concept>amount | Approved Commercial Amount / aso_approvedcommercialamount | aso_money1 |
| Yes/No | Use positive boolean wording: Required, Enabled, Allowed, Blocked, Called, Eligible, Approved, Match Flag. | aso_<concept><booleanmeaning> | Foundry Review Required / aso_foundryreviewrequired | Negative names like NotReady; ambiguous names like Flag1 |
| Choice - status | Use Status only for lifecycle/process state. | aso_<concept>status | AI Agent Status / aso_aiagentstatus | Using Status for category or type |
| Choice - stage | Use Stage for lifecycle phase. | aso_<concept>stage | Lifecycle Communication Stage / aso_lifecyclecommunicationstage | Level when it is really a lifecycle stage |
| Choice - type | Use Type for classification. | aso_<concept>type | Last Interaction Type / aso_customerinsightslastinteractiontype | Category without defined taxonomy |
| Date/time | Use On for event timestamps and Last <event> On for recent activity. | aso_<event>on | AI Last Run On / aso_ailastrunon | Date1, RunDateTime |
| Lookup | Use target table business noun; avoid ID in display. | aso_<targetnoun> | Opportunity lookup on Pending Commercial Action | Opportunity ID for lookup display |
| URL | Use URL in display; Url in environment variable schema names. | aso_<PascalCase>Url for env vars | APIM Base URL / aso_ApimBaseUrl | aso_APIMBaseURL |

### Yes/No naming rules

Yes/No fields must read naturally as true/false statements. Avoid negative booleans and vague labels.

| Business meaning | Good display name | Good schema name | Default | Reason |
| --- | --- | --- | --- | --- |
| Human review gate | Foundry Review Required | aso_foundryreviewrequired | No | Positive wording; true means review is required. |
| SAP match signal | AI SAP Match Flag | aso_aisapmatchflag | No | True means likely SAP match exists. |
| Feature toggle for SAP reads | Feature Flag - SAP Reads | aso_FeatureFlagSapReads | Yes in trial | True means read-only SAP context may be used through APIM/Functions when implemented. |
| ERP submit safety toggle | Feature Flag - ERP Submit | aso_FeatureFlagErpSubmit | No | True would allow write-path behavior only after integration, approval, idempotency, telemetry, and tests. |

### Environment variable contract

| Display name | Schema name | Type | Trial value | Purpose |
| --- | --- | --- | --- | --- |
| Foundry Orchestrator URL | aso_FoundryOrchestratorUrl | Text | TBD-FOR-PHASE-6 | Future Foundry parent orchestrator endpoint. |
| Foundry API Audience | aso_FoundryApiAudience | Text | TBD-FOR-PHASE-6 | Future Entra ID audience/app ID URI for Foundry calls. |
| APIM Base URL | aso_ApimBaseUrl | Text | TBD-FOR-PHASE-4 | Future APIM base URL for SAP wrapper access. |
| SAP Wrapper Version | aso_SapWrapperVersion | Text | v1 | Version of SAP wrapper API contract. |
| Customer Insights Compliance Profile | aso_ComplianceProfileName | Text | Global Commercial | Compliance profile used by future CI journeys. |
| Lead Journey Key | aso_LeadJourneyKey | Text | lead_nurture_v1 | Future lead nurture journey key. |
| Qualified Lead Journey Key | aso_QualifiedLeadJourneyKey | Text | qualified_lead_v1 | Future qualified lead journey key. |
| Opportunity Journey Key | aso_OpportunityJourneyKey | Text | opportunity_progression_v1 | Future opportunity progression journey key. |
| Quote Proposal Journey Key | aso_QuoteProposalJourneyKey | Text | quote_proposal_v1 | Future quote/proposal journey key. |
| Order Journey Key | aso_OrderJourneyKey | Text | order_confirmation_v1 | Future order confirmation journey key. |
| Onboarding Journey Key | aso_OnboardingJourneyKey | Text | onboarding_v1 | Future onboarding journey key. |
| Retention Journey Key | aso_RetentionJourneyKey | Text | retention_v1 | Future retention journey key. |
| Expansion Journey Key | aso_ExpansionJourneyKey | Text | expansion_v1 | Future expansion journey key. |
| HubSpot Sync Mode | aso_HubSpotSyncMode | Text | IngressOnly | HubSpot remains lead/contact ingress only and implemented last. |
| Feature Flag - SAP Reads | aso_FeatureFlagSapReads | Yes/No | Yes | Allows future read-only SAP context through APIM/Functions when implemented. |
| Feature Flag - ERP Submit | aso_FeatureFlagErpSubmit | Yes/No | No | ERP submit/write inactive until approval, APIM/Functions, idempotency, telemetry, and tests are complete. |

### Connection reference contract

| Connection reference | Connector | Create in Phase 1? | Rule |
| --- | --- | --- | --- |
| ASO Dataverse Connection Reference | Microsoft Dataverse | Yes | Used by solution-aware flows for Dataverse read/write. |
| ASO Approvals Connection Reference | Approvals | Yes if available | Used later for commercial approvals. |
| ASO Foundry HTTP Entra Connection Reference | HTTP with Microsoft Entra ID or custom connector | Defer until real endpoint/audience exists | Never hardcode Foundry endpoint or secret. |
| ASO APIM HTTP Entra Connection Reference | HTTP with Microsoft Entra ID or custom connector | Defer until real endpoint/audience exists | SAP access only through APIM + Functions. |
| ASO Teams Alerts Connection Reference | Microsoft Teams | Optional | Internal operational alerts only. |
| ASO Outlook Internal Notification Connection Reference | Office 365 Outlook | Optional | Internal notifications only; never customer lifecycle sends. |

### View naming categories

| View name | Entity | Purpose |
| --- | --- | --- |
| Sales Rep - Leads - AI Review Required | Lead | Seller queue for leads requiring human review. |
| Sales Rep - Leads - Customer Insights Eligible | Lead | Leads eligible for CIJ lifecycle communication. |
| Sales Manager - Opportunities - High Risk | Opportunity | Coaching queue for high-risk opportunities. |
| Sales Manager - Opportunities - Proposal Journey Eligible | Opportunity | Opportunities ready for proposal journey. |
| AI Ops - Agent Run Logs - Failed Last 24 Hours | Agent Run Log | Incident triage. |
| AI Ops - Agent Run Logs - Needs Review | Agent Run Log | Review cases from Foundry/sales-agent conflict. |
| Integration - External Sync Ledger - Failed Sync | External Sync Ledger | Failed external sync operations. |
| Integration - Pending Commercial Actions - Awaiting Approval | Pending Commercial Action | Approval queue. |
| Journey Ops - Participation Ledger - Failed or Error | Journey Participation Ledger | CIJ journey issue triage. |
| Data Quality - Leads - Missing Consent Status | Lead | Consent normalization cleanup. |
| Data Quality - Accounts - Missing SAP Customer ID | Account | SAP reference cleanup. |

### Creation anti-patterns

| Anti-pattern | Why it is wrong | Correct approach |
| --- | --- | --- |
| new_ prefix | Wrong publisher; makes project-owned assets difficult to identify. | Use aso prefix through the ASO publisher. |
| aso_aso_ double prefix | Caused by typing the prefix twice; breaks schema contract. | Enter only the part after aso_ when the UI already shows the prefix box. |
| APIMBaseURL / SAPWrapperVersion acronym drift | Inconsistent casing causes documentation and flow mapping errors. | Use ApimBaseUrl and SapWrapperVersion in environment variable schema names. |
| Customer Email Connection | Implies Outlook/Power Automate customer lifecycle sends. | Use Customer Insights - Journeys for lifecycle sends; Outlook only for internal notifications. |
| SAP Direct Connection | Violates architecture boundary. | Use APIM + Azure Functions only. |
| Views called My View, Test, Old | Unclear owner and action. | Use Audience - Entity - Filter/Action pattern. |
