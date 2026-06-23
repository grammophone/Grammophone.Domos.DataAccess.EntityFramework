# Container And Adapter Pattern

The EF6 containers are provider-specific infrastructure. They expose native `DbSet<T>` properties and EF6 model-building methods.

The adapters are application-facing:

- `EFUsersDomainContainerAdapter<U, D>` implements `IUsersDomainContainer<U>`.
- `EFWorkflowUsersDomainContainerAdapter<U, BST, D>` implements `IWorkflowUsersDomainContainer<U, BST>`.
- `EFDomosDomainContainerAdapter<...>` implements the accounting and invoice Domos contracts.

Each adapter caches `IEntitySet<T>` wrappers created with `EFSet<T>`. This is the same pattern used by the DataAccess EF6 test domain.

Consumers should register adapter instances as their Domos domain container contracts and keep the concrete EF container as the adapter's inner context.
