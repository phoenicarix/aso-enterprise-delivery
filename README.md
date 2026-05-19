# Agentic Sales Orchestrator — Enterprise Delivery Reference

This repository documents a public reference implementation journey for an Agentic Sales Orchestrator built around Microsoft Dynamics 365 Customer Engagement, Dataverse, Power Platform, Customer Insights - Journeys, Microsoft Foundry, Azure API Management, Azure Functions, SAP integration patterns, and HubSpot ingress.

## Purpose

The goal is to demonstrate enterprise-grade architecture, naming standards, governance, ALM discipline, integration boundaries, and implementation guidance for a modern Dynamics 365 CE and Power Platform solution.

## Current status

Phase 1 completed:

- Trial Sales environment confirmed
- Separate Customer Insights trial dependency confirmed
- ASO publisher created with prefix `aso`
- Power Platform solutions created:
  - `ASO.Core`
  - `ASO.Automation`
  - `ASO.Operations`
- Environment variables created in `ASO.Automation`
- Connection references created where available
- Trial limitations documented

## Architecture boundaries

- Dynamics 365 Sales / Dataverse is the operational sales truth.
- Customer Insights - Journeys is the only outbound customer lifecycle communication layer.
- Power Automate owns deterministic execution, Dataverse updates, approvals, retries, and handoffs.
- Microsoft Foundry coordinates orchestration, safety policy, routing, schema validation, SAP-aware tools, and escalation.
- Dynamics 365 Sales AI / Copilot Sales agents provide seller-facing intelligence.
- SAP is accessed only through Azure API Management and Azure Functions.
- HubSpot remains lead/contact ingress only and is implemented last.
- High-impact commercial actions require seller or manager approval.

## Public repository safety

This repository must not contain secrets, tokens, credentials, real customer data, private tenant details, or production configuration values.

All examples are intended for learning, demonstration, and architecture discussion.
