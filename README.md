# Grammophone.Domos.DataAccess.EntityFramework

`Grammophone.Domos.DataAccess.EntityFramework` is the Entity Framework 6 implementation of the Domos data-access contracts.

The concrete EF6 containers expose native `DbSet<T>` properties and inherit from the Grammophone EF6 data-access implementation. Adapter classes expose the provider-neutral Domos interfaces by wrapping those native sets as `IEntitySet<T>`.

## Main Features

- `EFUsersDomainContainer<U>` maps users, roles, dispositions, registrations, credentials, sessions and file metadata.
- `EFWorkflowUsersDomainContainer<U, BST>` adds workflow graph, state, path and transition mappings.
- `EFDomosDomainContainer<...>` adds accounting, funds transfer and optional invoice mappings.
- `EF*DomainContainerAdapter` classes expose `IEntitySet<T>` properties for application logic.
- The EF6 Domos base container derives from `EFDomainContainerPlus` because accounting uses portable set-based update/delete operations.

## Documentation

- [Overview](documentation/overview.md)
- [Container and adapter pattern](documentation/container-adapter-pattern.md)
- [Model mappings](documentation/model-mappings.md)
- [Set operations](documentation/set-operations.md)
