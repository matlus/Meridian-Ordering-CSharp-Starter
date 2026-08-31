# Feature Specification: Register Customer

**Feature ID:** CUS-001
**Status:** Draft for ratification
**Owner:** Product, Customer Management

## 1. Overview

Meridian Supply sells industrial products to registered business customers. The Place Order feature (ORD-001) lets a registered customer place orders. This feature is how a person becomes one.

As a person at a company that buys from Meridian Supply, I want to register as a customer, so that Meridian Supply knows who I am and can accept my orders.

A registration request carries who the person is, how to reach them, where their company is, and two preferences. The request is checked as a whole:

- An **accepted** registration records one new customer with status Active. Nothing is returned: completing without a refusal or failure is the entire success signal, and the registration channel simply tells the person that registration succeeded.
- A **refused** registration states every problem found, together in one refusal, and records nothing.
- A **temporary failure** means a valid request could not be recorded. It is distinct from a refusal, and it also records nothing.

## 2. Definitions

- **Registration request**: the details a person submits to become a customer. It carries the fields listed in BR-1 and nothing else. In particular it carries no status and no customer identifier, because both belong to the system.
- **Registration channel**: however the person submits their registration (a web page, a partner's system). The channel itself is outside this feature.
- **Customer record**: what an accepted registration creates: the provided details plus what the system adds (a customer identifier, the status, and the date and time of registration).
- **Active**: the customer status that permits placing orders (ORD-001 BR-1). Every accepted registration starts Active.
- **Provided**: a field counts as provided only when it holds visible characters. A field that is absent, empty, or only spaces counts as not provided. Provided values are checked, compared, and recorded without their surrounding spaces.
- **Not stated**: the recorded form of an optional field the person left out. Anyone later reading the record can tell the field was never given.
- **Well-formed email address**: text containing exactly one @ sign and no spaces, with at least one character before the @, and after it a domain that contains a dot with characters on both sides of the dot. Nothing deeper is checked, and no mail is sent to prove the mailbox exists.
- **Same email**: two email addresses that match when letter casing is ignored.
- **Same name**: first name and last name both matching when letter casing and surrounding spaces are ignored. The company name plays no part in this comparison.
- **Refusal**: a business decision that a registration request is invalid or not allowed. Distinct from a temporary failure, which prevents a valid request from being completed.

## 3. Business Rules

**BR-1.** A registration request carries exactly these thirteen fields:

| Field | Required or optional |
|---|---|
| Email address | Required |
| First name | Required |
| Last name | Required |
| Company name | Required |
| Phone number | Optional |
| Job title | Optional |
| Street address | Optional (see BR-6) |
| City | Optional (see BR-6) |
| State or region | Optional, in all cases |
| Postal code | Optional (see BR-6) |
| Country | Optional (see BR-6) |
| Preferred language | Optional |
| Marketing opt-in choice | Optional |

The marketing opt-in choice, when provided, is yes or no. Any other value is refused, and the refusal names the value that arrived and the two valid choices. A registration carrying anything beyond these thirteen fields, including a status, a customer identifier, or a field the system does not know, is refused naming each unexpected field. No field has a business maximum length: any storage-driven limits belong to the build team's technical requirements, as they do in ORD-001.

**BR-2.** A field that is absent, empty, or made only of spaces is not provided. A registration missing any required field is refused.

**BR-3.** The email address must be well formed as defined above. A registration whose email address is not well formed is refused.

**BR-4.** Email identity ignores letter casing: differently cased spellings are the same email. The recorded form is the lowercase form.

**BR-5.** One email address belongs to at most one customer. A registration whose email matches a recorded customer's email is refused, and the refusal names the exact situation without revealing anything else about the existing customer:

- Same email and same name: the refusal states that a customer by this name already exists.
- Same email and a different name: the refusal states that this email address is already registered to a customer by a different name. The existing customer's name is not revealed.

A matching name alone refuses nothing: the same name with a different email is simply another person (see AC-11).

**BR-6.** The company address is provided whole or not at all. If any of street address, city, postal code, or country is provided, then all four must be provided; otherwise the registration is refused naming the missing parts. State or region is optional in every case, including on its own: a state or region provided without the other address fields is not an address problem and is recorded as given.

**BR-7.** One refusal carries every problem of the stage that refused it. The checks accumulate: the requester learns all of the request's own problems at once, each explained clearly. The request's own problems (the BR-1 field rules, BR-2, BR-3, BR-6) are decided first and reported together. Email uniqueness (BR-5) is decided only for a request that has none of those problems, so a duplicate email surfaces only once the request itself is clean (see AC-15). A refusal records nothing.

**BR-8.** An accepted registration records exactly one customer: every provided field (the email in its lowercase form), each optional field that was not provided recorded as not stated, except the marketing opt-in choice, which records "no" when not provided. The system adds the customer identifier, status Active, and the date and time of registration. The requester supplies neither the identifier nor the status.

**BR-9.** Nothing is returned on success. Registering is an action: it either completes, or it explains why it could not.

**BR-10.** If the customer cannot be recorded, the requester receives a temporary-failure outcome distinct from a refusal, and no customer record survives, in whole or in part. The person may simply register again later.

## 4. Customers Used by the Acceptance Examples

One customer already exists before every example:

| Email | First name | Last name | Company name |
|---|---|---|---|
| priya@northwind.example | Priya | Sharma | Northwind Tooling |

The examples register two new people. Their emails belong to no recorded customer:

| Email | First name | Last name | Company name |
|---|---|---|---|
| marcus.webb@contoso.example | Marcus | Webb | Contoso Machining |
| dana.reyes@fabrikam.example | Dana | Reyes | Fabrikam Precision |

## 5. Acceptance Criteria

The criteria below demonstrate the business rules through concrete examples. Every clause in a Then block is a distinct, verifiable obligation.

### Expected Paths

**AC-01: A full registration is accepted (BR-1, BR-2, BR-3, BR-4, BR-6, BR-8, BR-9)**

Given no customer holds the email marcus.webb@contoso.example
When Marcus Webb registers with all thirteen fields: email typed as Marcus.Webb@Contoso.example, first name Marcus, last name Webb, company Contoso Machining, phone +1 555 0142, job title Procurement Manager, street 400 Foundry Road, city Dayton, state Ohio, postal code 45402, country United States, preferred language English, marketing opt-in yes
Then exactly one customer is recorded holding every provided value, with the email recorded as marcus.webb@contoso.example
And the record carries a customer identifier, status Active, and the date and time of registration, none of which came from the request
And nothing is returned: the registration completes, and that completion is the success signal

**AC-02: A minimal registration is accepted (BR-1, BR-2, BR-8, BR-9)**

Given no customer holds the email dana.reyes@fabrikam.example
When Dana Reyes registers providing only the four required fields
Then exactly one customer is recorded with status Active and a registration date and time
And each optional field is recorded as not stated, except the marketing opt-in choice, which is recorded as no
And nothing is returned

### Refused Paths

For every refused path: the refusal states each specific problem, and no customer record is created.

**AC-03: Missing required fields, reported together (BR-2, BR-7).** A registration without a first name and without a company name is refused once, and the refusal names both missing fields.

**AC-04: Blank counts as missing (BR-2).** A registration whose first name is made only of spaces is refused as missing its first name.

**AC-05: Email not well formed (BR-3).** A registration with the email dana.reyes.fabrikam.example (no @ sign) is refused naming the malformed email.

**AC-06: Same email, same name (BR-4, BR-5).** Priya Sharma is already a customer. A registration arrives with email PRIYA@Northwind.example and the name priya sharma. Casing differences change nothing: the registration is refused stating that a customer by this name already exists.

**AC-07: Same email, different name (BR-5).** A registration arrives with email priya@northwind.example and the name Rohan Mehta. It is refused stating that this email address is already registered to a customer by a different name, and the refusal does not reveal that name.

**AC-08: Incomplete address (BR-6, BR-7).** A registration provides a street address and a city but no postal code and no country. It is refused once, naming the missing postal code and country.

**AC-09: Every problem reported at once (BR-2, BR-3, BR-6, BR-7).** A registration arrives with no company name, the email dana.reyes.fabrikam.example, and an address consisting only of a street. It is refused once, and the single refusal lists all five problems: the missing company name, the malformed email, and the missing city, postal code, and country.

**AC-13: Unrecognized marketing opt-in choice (BR-1, BR-7).** A registration arrives complete and correct except that its marketing opt-in choice is the word maybe. It is refused, and the refusal names the value maybe and states that the valid choices are yes and no.

**AC-14: Extra fields (BR-1, BR-7).** A registration arrives carrying the thirteen fields and also a status field and a field named account tier. It is refused once, and the refusal names both unexpected fields. In particular, the supplied status is never honored, because status belongs to the system.

**AC-15: A duplicate email surfaces only once the request is clean (BR-5, BR-7).** A registration arrives missing its first name and carrying the email priya@northwind.example, which already belongs to a customer. The refusal names only the missing first name. When the corrected registration arrives with a first name, it is then refused for the duplicate email under the matching BR-5 message.

### Edge Cases

**AC-10: State or region alone (BR-6).** An otherwise complete registration, with all four required fields present, provides Ohio as its state or region and no other address field. This is not an address problem: the registration is accepted and Ohio is recorded as given.

**AC-11: Shared name, different email (BR-5).** Priya Sharma of Northwind Tooling is already a customer. A different Priya Sharma registers with the email priya.sharma@contoso.example. The registration is accepted: a second, separate customer now exists, because the email, not the name, is what identifies a customer.

### Failure Paths

**AC-12: The customer cannot be recorded (BR-10)**

Given a registration that would be accepted
When the customer cannot be recorded, or recording stops partway through
Then the requester receives a temporary-failure outcome, distinct from a refusal
And no customer record survives, in whole or in part

## 6. Out of Scope

Authentication and login: the registration channel does not check who is calling, and no password or credential exists anywhere in this feature. Email verification of any kind, including sending a code the registrant must return. Welcome or confirmation emails: an accepted registration sends nothing. Updating or deactivating a customer: the Inactive status ORD-001 depends on exists in the business, but nothing in this feature assigns it. Customer lookup, including how a registered customer learns the identifier used when placing orders. Checking addresses against postal records, phone numbers against numbering plans, or country and language values against official lists: these fields are recorded as given. Marketing consent management beyond recording the opt-in choice.
