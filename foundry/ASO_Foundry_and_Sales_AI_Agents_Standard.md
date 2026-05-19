# ASO Foundry and Sales AI Agents Standard

Sales AI agent responsibility, Foundry orchestration, code structure, canonical contracts, safety policy, and evaluation approach.

> GitHub path: `foundry/ASO_Foundry_and_Sales_AI_Agents_Standard.md`

> Public safety: do not publish tenant IDs, credentials, secrets, real customer data, private endpoints, or connection strings.

## Foundry and Dynamics 365 Sales AI agent standard

This file covers two AI layers: Dynamics 365 Sales AI/Copilot Sales agents for seller-facing intelligence, and Microsoft Foundry for parent orchestration, child agents, safety, routing, schema validation, and deterministic handoff back to Power Automate.

### Sales AI / Copilot Sales agent responsibility

| Agent | Primary responsibility | Must not do |
| --- | --- | --- |
| Sales Qualification Agent | Research and assess inbound leads, fit, routing support, and seller-facing rationale. | No ERP posting; no direct customer lifecycle sends; no bypass of Foundry policy. |
| Sales Opportunity / Deal Close Agent | Assess opportunity importance, risk, close-readiness, and recommendations. | No SAP posting; no workflow execution; no direct lifecycle sends. |

### Sales agent configuration standards

| Configuration area | Standard |
| --- | --- |
| Mode | Start with Research-only or restricted engagement. Research and engage requires explicit compliance and Customer Insights boundary review. |
| Knowledge sources | Use approved public sites and approved internal documents only. Do not add unmanaged SAP exports, confidential pricing, contract templates, or uncontrolled sensitive files. |
| Lead selection | Pilot only: Open leads, approved source, email exists, consent not blocked, not already Running, pilot owner/BU. |
| Opportunity selection | Pilot only: Open opportunities, active stages, close date in pilot window, excludes closed won/lost and Running records. |
| Writeback | Native agent output may be seller-visible only. Foundry/Power Automate normalizes governed fields where native writeback is limited. |
| Testing | Use 5-10 pilot leads/opportunities. Confirm no unauthorized customer message and no ERP action occurred. |

### Foundry responsibilities

| Foundry owns | Foundry does not own |
| --- | --- |
| End-to-end orchestration visibility; agent sequencing and routing; safety guardrails; SAP-aware tool coordination through APIM; schema validation; separation of CRM facts, SAP facts, sales-agent outputs, and inferences; deterministic handoff to Power Automate. | CRM transactional truth; ERP posting; customer communication sends; secret storage; seller decision rights; unreviewed commercial actions. |

### Repository structure standard

```text
aso-foundry-orchestrator/
  src/
    orchestrator/
      parent.py
      router.py
      policy.py
      schemas.py
      telemetry.py
    agents/
      lead_origination.py
      nurturing.py
      qualification.py
      opportunity_classification.py
      risk_competitor.py
      next_best_action.py
      handoff.py
      account_development.py
    adapters/
      dataverse_client.py
      sap_client.py
      sales_agent_context.py
      customer_insights_context.py
    contracts/
      canonical_input.schema.json
      canonical_output.schema.json
      child_outputs.schema.json
  tests/
    unit/
    integration/
    evaluation/
  pipelines/
  README.md
```

### Canonical input and output requirements

| Contract element | Required fields |
| --- | --- |
| Input identity | message_id, correlation_id, record_type, record_id, trigger_type, trigger_reason |
| Tenant context | environment_name, business_unit, initiating_system |
| External refs | hubspot_contact_id, sap_customer_id, sap_business_partner_id, customer_insights_profile_id |
| Sales agent context | Qualification status, score, rationale; opportunity status, risk, recommendation |
| Execution policy | max_agent_retries, allow_sap_reads, allow_sap_writes, allow_customer_insights_activation, human_approval_required |
| Output identity | message_id, correlation_id, status, summary, record_type, record_id, trace_id |
| Output actions | update_dataverse, create_task, notify, submit_for_approval, enqueue_retry, start_customer_journey, sync_consent_state, none |
| Output governance | confidence, requires_human_review, recommended_lifecycle_stage, facts grouped by CRM/SAP/sales_agent/inferred |

### Safety policy

- Block or escalate when confidence is below threshold.
- Escalate when Sales Qualification Agent and Foundry disagree materially.
- Escalate when SAP account match is ambiguous.
- Escalate when routing changes ownership or account ownership.
- Block communication when it violates consent state.
- Escalate high-risk opportunity commercial actions.
- Block ERP submit if approval is missing.
- Reject output JSON that fails schema validation.
