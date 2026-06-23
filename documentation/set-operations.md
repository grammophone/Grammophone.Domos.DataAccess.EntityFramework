# Set Operations

Domos Accounting uses set-based operations when enrolling funds transfer requests into batches and when deleting invoices.

In provider-neutral code these operations are expressed with `ExecuteUpdateAsync` and `ExecuteDeleteAsync` from `Grammophone.DataAccess.QueryExtensions`.

The EF6 Domos implementation derives from `EFDomainContainerPlus` so these portable operations are backed by the `Grammophone.DataAccess.EntityFramework.Plus` integration. The set operations execute immediately in the database and do not synchronize already tracked entities.
