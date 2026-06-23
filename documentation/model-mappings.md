# Model Mappings

The EF6 `OnModelCreating` methods translate Domos entity semantics into EF6 mappings.

User mappings include unique email and username indexes, creation date index, `UsersToRoles` many-to-many table, cascade relationships for dispositions and registrations, WebAuthn credential ownership and browser/session indexes.

Workflow mappings include workflow graph code-name uniqueness, state group and state code-name indexes and unique state-path code names.

Accounting mappings include indexes for credit systems, journals, postings, remittances, funds transfer events, requests, request groups, batches and batch messages. `EncryptedBankAccountInfo` is treated as an embedded complex/owned value through EF6's property path mapping.

Invoice mappings include invoice issue/due date indexes, invoice-to-request many-to-many links, cascade invoice line and tax component relationships, decimal precision for rates and quantities and invoice event indexes.
