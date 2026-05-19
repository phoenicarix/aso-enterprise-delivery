# ASO Power Platform, Dataverse, and Flows Standard

Solution boundaries, environment variables, connection references, schema, views, forms, security, and deterministic flow patterns.

> GitHub path: `power-platform/ASO_Power_Platform_Dataverse_and_Flows_Standard.md`

> Public safety: do not publish tenant IDs, credentials, secrets, real customer data, private endpoints, or connection strings.

## Power Platform, Dataverse, Sales, and flow standard

This file belongs in the Power Platform folder and covers the Power Platform implementation center of gravity: solutions, environment variables, connection references, Dataverse schema, Dynamics 365 Sales UI, security, and Power Automate deterministic flows.

### Solution boundaries

| Solution | Version in trial build | Contents | Creation rule |
| --- | --- | --- | --- |
| ASO.Core | 0.1.0.0 | Tables, columns, forms, views, security roles, choices | Create schema and Sales UI components here. |
| ASO.Automation | 0.1.0.0 | Cloud flows, environment variables, connection references | Create integration and automation config here. |
| ASO.Operations | 0.1.0.0 | Dashboards, admin views, model-driven operational app areas | Create operational monitoring and admin assets here. |
| ASO.Journeys | Deferred until CI phase unless needed | Customer Insights journey artifacts where ALM-supported | Do not create empty shell unless there is a real CIJ ALM reason. |

### Environment variables

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

### Connection references

| Connection reference | Connector | Create in Phase 1? | Rule |
| --- | --- | --- | --- |
| ASO Dataverse Connection Reference | Microsoft Dataverse | Yes | Used by solution-aware flows for Dataverse read/write. |
| ASO Approvals Connection Reference | Approvals | Yes if available | Used later for commercial approvals. |
| ASO Foundry HTTP Entra Connection Reference | HTTP with Microsoft Entra ID or custom connector | Defer until real endpoint/audience exists | Never hardcode Foundry endpoint or secret. |
| ASO APIM HTTP Entra Connection Reference | HTTP with Microsoft Entra ID or custom connector | Defer until real endpoint/audience exists | SAP access only through APIM + Functions. |
| ASO Teams Alerts Connection Reference | Microsoft Teams | Optional | Internal operational alerts only. |
| ASO Outlook Internal Notification Connection Reference | Office 365 Outlook | Optional | Internal notifications only; never customer lifecycle sends. |

### Global choices

| Choice schema | Values | Purpose |
| --- | --- | --- |
| aso_AIStatus | NotStarted; Running; Completed; Escalated; Failed; PartiallyCompleted; Blocked | Reusable AI/orchestration run status. |
| aso_CommunicationState | NotEligible; Eligible; Queued; InJourney; WaitingInteraction; Completed; Suppressed; Failed; Blocked | Controls Customer Insights lifecycle readiness and state. |
| aso_LifecycleCommunicationStage | Lead; QualifiedLead; Opportunity; QuoteProposal; Order; Onboarding; Retention; Expansion | Defines lifecycle stage for communication routing. |
| aso_JourneyParticipationStatus | NotStarted; Active; Paused; Completed; Exited; Error | Normalizes Customer Insights participation state. |
| aso_EmailConsentStatus | OptedIn; OptedOut; Pending; Unknown; Blocked | Normalizes consent state across systems. |
| aso_AgentType | FoundryParent; FoundryChildLeadOrigination; FoundryChildNurturing; FoundryChildOpportunityClassification; FoundryChildRiskCompetitor; FoundryChildNextBestAction; FoundryHandoff; SalesQualificationAgent; SalesOpportunityAgent; SalesDealCloseAgent; CustomerInsightsJourney; SAPWrapper | Identifies source in logs. |
| aso_OutputSourceType | CRMFact; SAPFact; SalesQualificationAgentOutput; SalesOpportunityAgentOutput; FoundryInference; SellerInput; CustomerJourneySignal | Separates facts, agent outputs, inferences, and seller input. |

### Lead table extension

| Display name | Schema name | Type | Notes |
| --- | --- | --- | --- |
| HubSpot Contact ID | aso_hubspotcontactid | Text | External ingress key. |
| HubSpot Source | aso_hubspotsource | Text or Choice | Immutable after ingress unless governed. |
| SAP Business Partner ID | aso_sapbusinesspartnerid | Text | SAP reference only. |
| SAP Customer ID | aso_sapcustomerid | Text | SAP customer reference only. |
| AI Fit Level | aso_aifitlevel | Choice | Strong, Moderate, Weak. |
| AI Lead Score | aso_aileadscore | Whole number | 0-100. |
| AI Qualification Rationale | aso_aiqualificationrationale | Multiline text | Seller-facing rationale. |
| AI Outreach Draft | aso_aioutreachdraft | Multiline text | Draft only; not direct send. |
| AI Routing Decision | aso_airoutingdecision | Choice | Nurture, SDR, AE, Reject, NeedsReview, ExistingAccountReview. |
| AI Confidence | aso_aiconfidence | Decimal | 0.00-1.00. |
| AI Last Run On | aso_ailastrunon | Date/time | Timestamp of last AI run. |
| AI Agent Status | aso_aiagentstatus | Choice | Use aso_AIStatus. |
| AI SAP Context Summary | aso_aisapcontextsummary | Multiline text | SAP-derived summary. |
| AI SAP Match Flag | aso_aisapmatchflag | Yes/No | Likely SAP match. |
| AI Correlation ID | aso_aicorrelationid | Text | Latest orchestration correlation. |
| Sales Qualification Agent Status | aso_salesqualificationagentstatus | Choice | Status from Sales Qualification Agent. |
| Sales Qualification Agent Score | aso_salesqualificationagentscore | Whole number | Sales agent score if available. |
| Sales Qualification Agent Rationale | aso_salesqualificationagentrationale | Multiline text | Output from Sales Qualification Agent. |
| Sales Qualification Agent Last Run On | aso_salesqualificationagentlastrunon | Date/time | Timestamp. |
| Foundry Final Qualification Decision | aso_foundryfinalqualificationdecision | Choice/Text | Post-validation decision. |
| Foundry Review Required | aso_foundryreviewrequired | Yes/No | Human review gate. |
| Communication State | aso_communicationstate | Choice | Use aso_CommunicationState. |
| Lifecycle Communication Stage | aso_lifecyclecommunicationstage | Choice | Use aso_LifecycleCommunicationStage. |
| Journey Participation Status | aso_journeyparticipationstatus | Choice | Use aso_JourneyParticipationStatus. |
| Customer Insights Journey ID | aso_customerinsightsjourneyid | Text | Latest journey ID. |
| Customer Insights Journey Name | aso_customerinsightsjourneyname | Text | Latest journey name. |
| Customer Insights Last Entry On | aso_customerinsightslastentryon | Date/time | Last journey entry. |
| Customer Insights Last Interaction On | aso_customerinsightslastinteractionon | Date/time | Last interaction. |
| Customer Insights Last Interaction Type | aso_customerinsightslastinteractiontype | Choice | EmailSent, Open, Click, FormSubmit, Reply, Unsubscribe, Bounce, CustomAction. |
| Email Consent Status | aso_emailconsentstatus | Choice | Use aso_EmailConsentStatus. |
| Compliance Profile Name | aso_complianceprofilename | Text | Compliance profile used. |
| Communication Hold Reason | aso_communicationholdreason | Multiline text | Suppression or hold reason. |

### Opportunity table extension

| Display name | Schema name | Type |
| --- | --- | --- |
| SAP Customer ID | aso_sapcustomerid | Text |
| SAP Order Reference | aso_saporderreference | Text |
| AI Opportunity Tier | aso_aiopportunitytier | Choice: A, B, C |
| AI Summary | aso_aisummary | Multiline text |
| AI Risks | aso_airisks | Multiline text |
| AI Stakeholder Gaps | aso_aistakeholdergaps | Multiline text |
| AI Next Actions | aso_ainextactions | Multiline text |
| AI Risk Level | aso_airisklevel | Choice: Low, Medium, High |
| AI Follow-up Message Draft | aso_aifollowupmessage | Multiline text |
| AI Confidence | aso_aiconfidence | Decimal |
| AI Last Run On | aso_ailastrunon | Date/time |
| AI Agent Status | aso_aiagentstatus | Choice |
| AI SAP Commercial Summary | aso_aisapcommercialsummary | Multiline text |
| AI SAP Product Summary | aso_aisapproductsummary | Multiline text |
| AI Correlation ID | aso_aicorrelationid | Text |
| Sales Opportunity Agent Status | aso_salesopportunityagentstatus | Choice |
| Sales Opportunity Agent Importance | aso_salesopportunityagentimportance | Choice/Text |
| Sales Opportunity Agent Risk | aso_salesopportunityagentrisk | Choice/Text |
| Sales Opportunity Agent Recommendation | aso_salesopportunityagentrecommendation | Multiline text |
| Sales Deal Close Agent Guidance | aso_salesdealcloseagentguidance | Multiline text |
| Sales Deal Close Agent Last Run On | aso_salesdealcloseagentlastrunon | Date/time |
| Foundry Final Opportunity Decision | aso_foundryfinalopportunitydecision | Multiline text |
| Foundry Review Required | aso_foundryreviewrequired | Yes/No |
| Communication State | aso_communicationstate | Choice |
| Lifecycle Communication Stage | aso_lifecyclecommunicationstage | Choice |
| Journey Participation Status | aso_journeyparticipationstatus | Choice |
| Customer Insights Journey ID | aso_customerinsightsjourneyid | Text |
| Customer Insights Journey Name | aso_customerinsightsjourneyname | Text |
| Customer Insights Last Entry On | aso_customerinsightslastentryon | Date/time |
| Customer Insights Last Interaction On | aso_customerinsightslastinteractionon | Date/time |
| Customer Insights Last Interaction Type | aso_customerinsightslastinteractiontype | Choice |
| Email Consent Status | aso_emailconsentstatus | Choice |
| Compliance Profile Name | aso_complianceprofilename | Text |
| Communication Hold Reason | aso_communicationholdreason | Multiline text |

### Account and Contact extension summary

| Table | Fields |
| --- | --- |
| Account | aso_sapbusinesspartnerid; aso_sapcustomerid; aso_aiaccounthealth; aso_aigrowthsummary; aso_airenewalrisk; aso_ailastrunon; aso_aisapaccountrichcontext; aso_aicorrelationid; aso_communicationstate; aso_lifecyclecommunicationstage; aso_journeyparticipationstatus; aso_customerinsightsjourneyid; aso_customerinsightsjourneyname; aso_customerinsightslastentryon; aso_customerinsightslastinteractionon; aso_customerinsightslastinteractiontype; aso_emailconsentstatus; aso_complianceprofilename; aso_communicationholdreason. |
| Contact | aso_communicationstate; aso_lifecyclecommunicationstage; aso_journeyparticipationstatus; aso_customerinsightsjourneyid; aso_customerinsightsjourneyname; aso_customerinsightslastentryon; aso_customerinsightslastinteractionon; aso_customerinsightslastinteractiontype; aso_emailconsentstatus; aso_complianceprofilename; aso_preferredemailaddress; aso_preferencecenterlastvisitedon. |

### ASO custom tables

| Table | Schema | Purpose | Key columns |
| --- | --- | --- | --- |
| Agent Run Log | aso_agentrunlog | Logs each AI, Foundry, Sales Agent, Power Automate, and SAP wrapper run. | Agent Type; Record Type; Record ID; Message ID; Correlation ID; Input Payload; Output Payload; Confidence; Status; Error Message; Started On; Finished On; Trace ID; Sources Used; SAP Called; Retry Count; Foundry Run ID; Sales Agent Run ID; Customer Insights Journey ID. |
| External Sync Ledger | aso_externalsyncledger | Tracks idempotent sync status with HubSpot, SAP, Foundry, Customer Insights, and Power Automate. | Source System; External Entity; External ID; Dataverse Entity; Dataverse ID; Last Hash; Last Processed On; Status; Correlation ID. |
| Pending Commercial Action | aso_pendingcommercialaction | Holds commercial actions that require review/approval before SAP submission. | Opportunity; Action Type; Payload; Status; SAP Document ID; Error Message; Approval ID; Idempotency Key; Submitted On. |
| Journey Participation Ledger | aso_journeyparticipationledger | Tracks journey starts, exits, interactions, errors, and communication state. | Record Type; Record ID; Lifecycle Communication Stage; Journey ID; Journey Name; Participation Status; Entry Trigger Name; Entry Source; Started On; Last Interaction On; Last Interaction Type; Exit Reason; Correlation ID; Error Message. |
| Sales Agent Insight Snapshot | aso_salesagentinsightsnapshot | Optional normalization table if native Sales Agent outputs are not easily consumed on Lead/Opportunity. | Record Type; Record ID; Agent Type; Insight Title; Insight Summary; Risk Level; Importance; Recommendation; Source Timestamp; Correlation ID; Raw Reference ID. |

### Views

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

### Power Automate general rules

| Rule | Standard |
| --- | --- |
| Solution-aware | All ASO flows are created in ASO.Automation. |
| Connection references | Use approved connection references; do not bind production logic to personal unmanaged connections. |
| Environment variables | Use variables for Foundry, APIM, SAP wrapper, Customer Insights keys, HubSpot sync mode, and feature flags. |
| No hardcoding | Do not hardcode URLs, IDs, journey keys, or integration endpoints. |
| Trigger conditions | Use trigger conditions and selective columns to prevent recursion and over-triggering. |
| Scopes | Use standard scopes: Initialize, Validate, Build Payload, Call Foundry, Process Response, Dataverse Updates, Logging, Error Handling. |
| Correlation | Every run generates/preserves message_id and correlation_id. |
| External calls | Every external call passes correlation headers. |
| Customer communication | Never send customer lifecycle email from Power Automate. |
| Error handling | Write Agent Run Log or operational logs on success/failure as appropriate. |

### Flow catalogue

| Flow | Trigger / Purpose | Critical rules |
| --- | --- | --- |
| ASO - Lead - Dispatch to Foundry | Dataverse Lead added/modified; dispatch canonical payload to Foundry. | Use trigger conditions, set AI status Running, include message_id/correlation_id, write Agent Run Log. |
| ASO - Opportunity - Dispatch to Foundry | Opportunity updates; send opportunity context to Foundry. | Include Sales Opportunity/Deal Close context, SAP normalized context, update fields and tasks. |
| ASO - CIJ - Participation Sync | CIJ interaction/participation update or custom action. | Upsert Journey Participation Ledger; update normalized fields; create seller task when engagement requires follow-up. |
| ASO - CIJ - Consent Sync | Consent/preference update or scheduled sync. | Map native consent to aso_emailconsentstatus; suppress/block communication when needed. |
| ASO - Commercial - Approval and SAP Submit | Commercial milestone or command creates Pending Commercial Action. | Human approval required; call APIM only after approval; use idempotency key; no blind retry. |
| ASO - Operations - Replay Failed Run | Manual trigger from Agent Run Log or command button. | Validate AI Operations Reviewer role; enforce retry count; reuse correlation ID with new message ID. |
