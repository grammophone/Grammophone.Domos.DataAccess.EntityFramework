using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFramework;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Accounting;
using Grammophone.Domos.Domain.Workflow;

namespace Grammophone.Domos.DataAccess.EntityFramework
{
	/// <summary>
	/// Entity Framework implementation of a Domos repository,
	/// containing users, roles, accounting, workflow, managers and permissions.
	/// </summary>
	/// <typeparam name="U">
	/// The type of users. Must be derived from <see cref="User"/>.
	/// </typeparam>
	/// <typeparam name="BST">
	/// The base type of the system's state transitions, derived from <see cref="StateTransition{U}"/>.
	/// </typeparam>
	/// <typeparam name="P">The type of the postings, derived from <see cref="Posting{U}"/>.</typeparam>
	/// <typeparam name="R">The type of remittances, derived from <see cref="Remittance{U}"/>.</typeparam>
	/// <typeparam name="J">
	/// The type of accounting journals, derived from <see cref="Journal{U, ST, P, R}"/>.
	/// </typeparam>
	/// <remarks>
	/// The global cascade delete convention is turned off. When needed, please enable
	/// cascade delete on a per entity basis by overriding <see cref="OnModelCreating(DbModelBuilder)"/>.
	/// </remarks>
	public abstract class EFDomosDomainContainer<U, BST, P, R, J> 
		: EFWorkflowUsersDomainContainer<U, BST>, IDomosDomainContainer<U, BST, P, R, J>
		where U : User
		where BST : StateTransition<U>
		where P : Posting<U>
		where R : Remittance<U>
		where J : Journal<U, BST, P, R>
	{
		#region Construction

		/// <summary>
		/// Constructs a new container instance using conventions to 
		/// create the name of the database to which a connection will be made.
		/// The by-convention name is the full name (namespace + class name)
		/// of the derived container class.
		/// The <see cref="TransactionMode"/> is set to <see cref="TransactionMode.Real"/>.
		/// </summary>
		public EFDomosDomainContainer()
		{
		}

		/// <summary>
		/// Constructs a new container instance using conventions to 
		/// create the name of the database to which a connection will be made.
		/// The by-convention name is the full name (namespace + class name)
		/// of the derived container class.
		/// </summary>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFDomosDomainContainer(TransactionMode transactionMode)
			: base(transactionMode)
		{
		}

		/// <summary>
		/// Constructs a new container instance using the given string as the name
		/// or connection string for the database to which a connection will be made. 
		/// The <see cref="TransactionMode"/> is set to <see cref="TransactionMode.Real"/>.
		/// </summary>
		/// <param name="nameOrConnectionString">
		/// Either the database name or a connection string.
		/// </param>
		public EFDomosDomainContainer(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{

		}

		/// <summary>
		/// Constructs a new container instance using the given string as the name
		/// or connection string for the database to which a connection will be made. 
		/// </summary>
		/// <param name="nameOrConnectionString">
		/// Either the database name or a connection string.
		/// </param>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFDomosDomainContainer(string nameOrConnectionString, TransactionMode transactionMode)
			: base(nameOrConnectionString, transactionMode)
		{
		}

		/// <summary>
		/// Constructs a new container instance using a given connection.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="ownTheConnection">If true, hand over connection ownership to the container.</param>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFDomosDomainContainer(System.Data.Common.DbConnection connection, bool ownTheConnection, TransactionMode transactionMode)
			: base(connection, ownTheConnection, transactionMode)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Entity set of accounts in the system.
		/// </summary>
		public IDbSet<Account> Accounts { get; set; }

		/// <summary>
		/// Entity set of credit systems in the system.
		/// </summary>
		public IDbSet<CreditSystem> CreditSystems { get; set; }

		/// <summary>
		/// Entity set of accounting journals in the system.
		/// </summary>
		public IDbSet<J> Journals { get; set; }

		/// <summary>
		/// Entity set of the accounting postings in the system.
		/// </summary>
		public IDbSet<P> Postings { get; set; }

		/// <summary>
		/// Entity set of the accounting remittances in the system.
		/// </summary>
		public IDbSet<R> Remittances { get; set; }

		/// <summary>
		/// The Electronic Funds Transfer (EFT/ACH) requests in the system.
		/// </summary>
		public IDbSet<FundsTransferRequest> FundsTransferRequests { get; set; }

		/// <summary>
		/// The events taking place for <see cref="FundsTransferRequests"/> in the system.
		/// </summary>
		public IDbSet<FundsTransferEvent> FundsTransferEvents { get; set; }

		/// <summary>
		/// Batches of <see cref="FundsTransferRequest"/>s.
		/// </summary>
		public IDbSet<FundsTransferRequestBatch> FundsTransferRequestBatches { get; set; }

		/// <summary>
		/// Collations of <see cref="FundsTransferEventCollation"/>s.
		/// </summary>
		public IDbSet<FundsTransferEventCollation> FundsTransferEventCollations { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Add indexes and other non-implied database artifacts.
		/// </summary>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region CreditSystem

			modelBuilder.Entity<CreditSystem>()
				.Property(cs => cs.CodeName)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_CreditSystem_CodeName") { IsUnique = true }));

			#endregion

			#region Journal

			modelBuilder.Entity<J>()
				.Property(p => p.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Journal_CreationDate")));

			#endregion

			#region Posting

			modelBuilder.Entity<J>()
				.HasMany(j => j.Postings)
				.WithRequired()
				.HasForeignKey(p => p.JournalID);

			modelBuilder.Entity<P>()
				.Property(p => p.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Posting_CreationDate")));

			#endregion

			#region Remittance

			modelBuilder.Entity<J>()
				.HasMany(j => j.Remittances)
				.WithRequired()
				.HasForeignKey(p => p.JournalID);

			modelBuilder.Entity<R>()
				.Property(r => r.TransactionID)
				.HasColumnAnnotation("Index", new IndexAnnotation(
					new IndexAttribute("IX_Remittance_TransactionID_CreditSystemID", 1) { IsUnique = true }));

			modelBuilder.Entity<R>()
				.Property(r => r.CreditSystemID)
				.HasColumnAnnotation("Index", new IndexAnnotation(
					new IndexAttribute("IX_Remittance_TransactionID_CreditSystemID", 2) { IsUnique = true }));

			modelBuilder.Entity<R>()
				.Property(r => r.BatchID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Remittance_BatchID")));

			modelBuilder.Entity<R>()
				.Property(r => r.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Remittance_CreationDate")));

			#endregion

			#region FundsTransferEvent

			modelBuilder.Entity<FundsTransferEvent>()
				.Property(fte => fte.TraceCode)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferEvent_TraceCode")));

			modelBuilder.Entity<FundsTransferEvent>()
				.Property(fte => fte.ResponseCode)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferEvent_ResponseCode")));

			modelBuilder.Entity<FundsTransferEvent>()
				.Property(fte => fte.Type)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferEvent_Type_ResponseCode", 1)));

			modelBuilder.Entity<FundsTransferEvent>()
				.Property(fte => fte.ResponseCode)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferEvent_Type_ResponseCode", 2)));

			modelBuilder.Entity<FundsTransferEvent>()
				.Property(fte => fte.Date)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferEvent_Date")));

			#endregion

			#region FundsTransferRequest

			modelBuilder.Entity<FundsTransferRequest>()
				.Property(ftr => ftr.BatchID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferRequest_BatchID")));

			modelBuilder.Entity<FundsTransferRequest>()
				.Property(ftr => ftr.TransactionID)
				.HasColumnAnnotation("Index", new IndexAnnotation(
					new IndexAttribute("IX_FundsTransferRequest_TransactionID_CreditSystemID", 1) { IsUnique = true }));

			modelBuilder.Entity<FundsTransferRequest>()
				.Property(ftr => ftr.CreditSystemID)
				.HasColumnAnnotation("Index", new IndexAnnotation(
					new IndexAttribute("IX_FundsTransferRequest_TransactionID_CreditSystemID", 2) { IsUnique = true }));

			modelBuilder.Entity<FundsTransferRequest>()
				.Property(ftr => ftr.State)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_FundsTransferRequest_State")));

			#endregion
		}

		#endregion
	}
}
