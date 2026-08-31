# Implementation Guide

## Purpose

This repository is a clean slate for implementing Meridian Ordering. The supplied requirements define the behavior the system must exhibit, not the code structure or implementation technique that must produce it. Treat requirement language, acceptance criteria, technical constraints, and non-functional expectations as inputs to engineering judgment rather than as a hidden design diagram.

The architecture skeleton gives the exercise a practical starting shape and names responsibilities that need attention. It is guidance, not a completed architecture. Students may complete it, reshape it, or replace parts of it when another design better satisfies the requirements. Different architectures can be valid when their boundaries, trade-offs, and evidence are clear.

The exercise expects students to explain significant decisions. That explanation should connect each decision to a requirement or constraint and address:

- the reasoning behind the decision
- credible alternatives that were considered
- why the selected option fits this system
- trade-offs and risks introduced by the option
- how the behavior and design will be verified

A completed reference implementation is revealed later for comparison. It is not the specification and should not be treated as the only correct answer. The value of the exercise is in moving from requirements to evidence-backed implementation decisions.

## What is provided

The starter includes:

- authoritative business and technical requirement documents
- fixed acceptance criteria and fixtures contained in those documents
- Docker Compose infrastructure for local SQL Server and RabbitMQ dependencies
- an optional Azure Service Bus emulator profile
- a buildable .NET 10 solution
- a public API host and a named `DomainFacade` application boundary
- empty, responsibility-named shells for managers, validators, data managers, gateways, message brokers, message composers, and service location
- an empty functional acceptance test area
- folder-level documentation describing what belongs in each area and what does not

These elements make the repository usable without supplying the solution. They deliberately do not include:

- completed business behavior
- implemented acceptance, class, controller, or HTTP-pipeline tests
- application database schema, stored procedures, or migrations
- configured application queues, topics, subscriptions, or bindings
- prescribed routes, JSON property names, HTTP status mappings, or error envelopes
- completed test-support helpers
- reference architecture or migration-decision documents
- k6 or other load-testing assets

Docker Compose provides processes that application code may depend on. It does not determine the application architecture, transaction boundaries, resource ownership, or integration design. Students own application provisioning and must decide which database and messaging resources their solution requires.

Load testing is excluded from this stage. First establish correctness through observable functional acceptance behavior and make the implementation explainable. Performance work can follow once there is meaningful behavior to exercise.

## Getting started

Read both requirement documents in full before writing the first production type. Rules that appear local may interact with acceptance criteria, failure semantics, persistence constraints, retry behavior, or external effects elsewhere in the same document. Build a view of the complete behavior before choosing the first slice.

Identify the public application boundary through which a caller requests customer registration or order placement. Decide what the boundary accepts and returns based on the requirements, while avoiding transport assumptions that the requirements do not make. Functional acceptance tests should exercise required behavior through this public boundary.

Before implementing a scenario, state the observable outcome it must prove. Depending on the requirement, evidence can include:

- returned results and their required information
- business refusals or rejections and all required reasons
- temporary or technical failures that remain distinct from business outcomes
- state that was persisted, including exact values significant to the requirement
- state that was not persisted
- messages that were published and the information they communicate
- communications sent through external services
- effects that occurred once under retry or resubmission
- rollback when a required operation fails
- absence of partial writes, duplicate effects, or other unintended outcomes

Start with one happy-path vertical slice. Choose one acceptance scenario that can travel through the public boundary, application coordination, validation, persistence, and required external collaboration. Implement enough of each layer to make that single scenario pass with real observable evidence. Avoid completing all controllers first, all persistence first, or all integrations first; those horizontal passes defer the feedback that reveals whether the parts work together.

After the first happy path, add one scenario at a time. A useful progression is:

1. Add other successful variations that introduce meaningful rules.
2. Add business refusals and rejections.
3. Add temporary dependency and persistence failures.
4. Prove rollback and no unintended effects.
5. Add retry, duplicate, and idempotency scenarios where required.
6. Cover boundary values, normalization, case handling, and other specified nuances.

Keep the acceptance suite focused on observable behavior rather than internal calls or class structure. Internal design should remain free to change without rewriting behavior specifications. Add narrower tests only when they improve feedback for complex internal logic; do not substitute them for functional acceptance evidence.

Test-driven development is permitted but not mandated. A student may write the scenario before implementation, build a thin slice and then capture it with a test, or alternate between the two. Regardless of workflow, a requirement is not complete until repeatable evidence demonstrates its public outcome and relevant side effects.

Review the implementation after each vertical slice:

- Does the code express the requirement in domain language?
- Can the next scenario be added without bypassing the public boundary?
- Are external dependencies replaceable at the correct boundary for acceptance verification?
- Are failures explicit rather than converted into success-shaped defaults?
- Can the test prove both intended effects and absence of unintended effects?
- Did the slice reveal an architectural decision that should be recorded?

## Architectural design decisions

Make the major design decisions before implementation creates accidental answers. Revisit them when a vertical slice supplies new evidence. The provided shells make important responsibilities visible, but they do not decide how those responsibilities collaborate.

**System and application boundary**

- What operation is exposed for each required business capability?
- Which contracts are public, and which types remain implementation details?
- Which concerns belong to the API host, and which belong behind `DomainFacade`?
- How will transport-specific parsing and composition remain separate from business decisions?
- Which transport details are choices because the requirements do not prescribe them?

**Responsibility ownership**

- Which component owns orchestration for customer registration and order placement?
- Which component owns each validation rule?
- Where are persistence-focused decisions made?
- Who composes fulfillment notifications and confirmation communications?
- Who selects and invokes a messaging implementation?
- Who translates external responses into application outcomes?
- Are responsibilities cohesive, or is one component accumulating unrelated reasons to change?

**Dependency direction**

- Which layer may depend on which other layer?
- Do business decisions depend on framework, database, broker, or HTTP-client details?
- Where should abstractions exist to keep external mechanisms replaceable?
- Does dependency construction remain visible at startup?
- If service location is retained, what boundary prevents hidden dependencies? If it is replaced, why is the alternative clearer?

**Persistence and transactions**

- What information must be read and written together?
- Where does a transaction begin and end?
- Which acceptance outcomes require all-or-nothing persistence?
- How will constraints that protect required uniqueness or consistency be represented?
- How will persistence report business conflicts separately from temporary technical failure?
- Which component owns connections, commands, transactions, and their disposal?
- How will functional acceptance tests observe committed state and verify rollback without depending on implementation internals?

**External effects and idempotency**

- Which effects happen inside the persistence transaction, and which cannot?
- What durable state represents an external action that still needs to occur?
- How will retry eligibility be decided?
- How will repeated requests avoid duplicate persistence, messages, and communications?
- Which identifier establishes identity for an operation?
- How will the system distinguish an identical resubmission from conflicting content?
- What happens when persistence succeeds but a later external dependency fails?
- What evidence proves an effect happened once, not merely that a method was called?

**Exception and failure policy**

- Which outcomes are expected business refusals, which are conflicts, and which are technical failures?
- Where are dependency exceptions translated into application-specific meaning?
- What context must be added so a failure is diagnosable without exposing sensitive data?
- Which layer logs a failure, and how will duplicate logging be avoided?
- Where are application failures translated into public boundary responses?
- How will the design avoid broad catches, silent returns, and success-shaped fallbacks?

**Configuration**

- Which values vary by environment?
- How are required values supplied and validated before they are used?
- Which defaults are safe for local development, and which values must always be explicit?
- How is configuration represented without leaking credentials into source control?
- Which component interprets provider selection or retry settings?
- How will tests control configuration without depending on workstation state?

**Resource ownership**

- Who creates and disposes database, HTTP, and messaging resources?
- What lifetime does each resource require?
- Which infrastructure resources are provisioned outside the application?
- How does startup fail when required resources or configuration are missing?
- How are cancellation, timeout, and shutdown responsibilities propagated?
- Can parallel acceptance scenarios use resources without hidden shared mutable state?

**Programming with Intent**

- Do names state business purpose rather than implementation mechanism?
- Does each method operate at one understandable level of abstraction?
- Can a reader see the policy before reading low-level details?
- Are important choices represented by types and structure rather than comments?
- Are conditionals and failure paths phrased in domain language?
- Are abstractions introduced because they clarify responsibility, not merely to increase indirection?
- Does control flow make required sequencing and all-or-nothing behavior apparent?

Code communicates intent. Prefer clear names, explicit access modifiers, focused types, and visible dependency direction. Comments are reserved for genuinely non-obvious constraints or deliberate departures whose reason cannot be expressed through code. Do not use comments to narrate ordinary code or disclose implementation answers.

Record significant choices concisely:

| Field | Content |
| --- | --- |
| Decision | The option selected and the boundary it affects |
| Reason | The requirement, quality attribute, or constraint driving the choice |
| Alternatives | Credible options considered |
| Why this option | Why the selected option fits this context better |
| Trade-offs | Costs, risks, limitations, and consequences accepted |
| Verification | Acceptance evidence, focused tests, measurements, or operational checks |

Use these questions for an architecture review before declaring a capability complete:

- Can every important requirement be traced to observable acceptance evidence?
- Does the public boundary expose only what callers need?
- Is each business rule owned once?
- Do dependencies point toward business policy rather than external mechanisms?
- Are transaction boundaries explicit and consistent with rollback requirements?
- Are external effects recoverable and safe under retries?
- Are failure categories preserved from dependency to public boundary?
- Is configuration validated and secret-free?
- Is resource ownership unambiguous?
- Can the architecture change internally without rewriting functional acceptance scenarios?
- Does the code communicate intent without relying on explanatory comments?
- Are the documented trade-offs still acceptable after implementation evidence?
