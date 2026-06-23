# Overview

`Grammophone.Domos.DataAccess.EntityFramework` provides the EF6 backing implementation for Domos data access.

Concrete containers expose native EF6 `DbSet<T>` properties so Entity Framework can build the model. Application logic should not consume those concrete containers as Domos contracts. Instead, adapter classes expose `IEntitySet<T>` wrappers.

The implementation mirrors the Domos feature levels:

- `EFUsersDomainContainer<U>` for users and security support.
- `EFWorkflowUsersDomainContainer<U, BST>` for workflow support.
- `EFDomosDomainContainer<...>` for accounting, funds transfers and optional invoices.

The base EF6 users container derives from `EFDomainContainerPlus`, because Domos Accounting uses set-based update/delete operations exposed through portable query extensions.
