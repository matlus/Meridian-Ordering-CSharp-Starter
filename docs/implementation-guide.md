# Implementation Guide

## Purpose

This repository is a clean slate for implementing Meridian Ordering. The requirements define externally observable behavior, not a required implementation. Different architectures may satisfy the same requirements.

Students are expected to explain important decisions, reasoning, alternatives, trade-offs, and verification. A completed reference implementation is revealed later for comparison, not as the only correct design.

## What is provided

- Authoritative business and technical requirements
- Acceptance criteria
- Docker Compose infrastructure for local dependencies
- A buildable .NET solution with high-level responsibility areas

Infrastructure supports development; it does not define the application architecture. Application-owned database objects and messaging resources remain student work. k6 and load-testing assets are excluded from this stage.

## Getting started

Read the requirements as a whole before implementing individual rules. Identify the public application boundary and express functional acceptance scenarios through that boundary.

Scenarios should prove every observable outcome that matters:

- returned results
- refusals and failures
- persisted state
- published messages and composed communications
- rollback and absence of unintended effects

Begin with one happy-path vertical slice. Carry it through the public boundary, business behavior, persistence, and external effects as required. Then add unhappy paths, boundary conditions, and nuanced rules one scenario at a time. Test-driven development is permitted but not mandated; evidence at the functional acceptance boundary is the primary goal.

## Architectural design decisions

Before implementation, answer these questions:

- Where is the system boundary, and what is public at that boundary?
- Which component owns each responsibility?
- In which direction do dependencies point?
- How are external systems isolated from business decisions?
- What defines a transaction, and which effects must succeed or roll back together?
- How are external effects made safe under retries, including idempotency?
- How are exceptions classified, enriched with useful context, logged, and translated at the boundary?
- How is configuration supplied, validated, and kept separate from code?
- Who creates, owns, and disposes each resource?
- How does the design support Programming with Intent: names, abstractions, and control flow that state purpose before mechanism?

For every significant choice, record:

| Field | Question |
| --- | --- |
| Decision | What was chosen? |
| Reason | What requirement or constraint drives the choice? |
| Alternatives | What credible options were considered? |
| Why this option | Why does this choice fit better here? |
| Trade-offs | What costs, risks, or limitations are accepted? |
| Verification | How will evidence show the decision works? |

Code should convey intent through structure and naming. Reserve comments for genuinely non-obvious constraints or deliberate departures that code alone cannot communicate.
