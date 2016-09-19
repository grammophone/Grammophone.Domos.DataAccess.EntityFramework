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
	/// <typeparam name="ST">
	/// The type of state transitions, derived from <see cref="StateTransition{U}"/>.
	/// </typeparam>
	/// <typeparam name="A">The type of accounts, derived from <see cref="Account{U}"/>.</typeparam>
	/// <typeparam name="P">The type of the postings, derived from <see cref="Posting{U, A}"/>.</typeparam>
	/// <typeparam name="R">The type of remittances, derived from <see cref="Remittance{U, A}"/>.</typeparam>
	/// <typeparam name="J">
	/// The type of accounting journals, derived from <see cref="Journal{U, ST, A, P, R}"/>.
	/// </typeparam>
	public abstract class EFDomosDomainContainer<U, ST, A, P, R, J> 
		: EFWorkflowUsersDomainContainer<U, ST>, IDomosDomainContainer<U, ST, A, P, R, J>
		where U : User
		where ST : StateTransition<U>
		where A : Account<U>
		where P : Posting<U, A>
		where R : Remittance<U, A>
		where J : Journal<U, ST, A, P, R>
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
		public IDbSet<A> Accounts { get; set; }

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

		#endregion

		#region Protected methods

		/// <summary>
		/// Add indexes and other non-implied database artifacts.
		/// </summary>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Account

			modelBuilder.Entity<A>()
				.HasMany(a => a.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("AccountsToOwners"));

			modelBuilder.Entity<A>()
				.Property(a => a.LastModificationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Account_LastModificationDate")));

			#endregion

			#region CreditSystem

			modelBuilder.Entity<CreditSystem>()
				.Property(cs => cs.CodeName)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_CreditSystem_CodeName") { IsUnique = true }));

			#endregion

			#region Journal

			modelBuilder.Entity<J>()
				.Property(p => p.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Journal_CreationDate")));

			modelBuilder.Entity<J>()
				.HasMany(j => j.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("JournalsToOwners"));

			#endregion

			#region Posting

			modelBuilder.Entity<P>()
				.Property(p => p.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Posting_CreationDate")));

			modelBuilder.Entity<P>()
				.HasMany(p => p.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("PostingsToOwners"));

			#endregion

			#region Remittance

			modelBuilder.Entity<R>()
				.Property(r => r.TransactionID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Remittance_TransactionID_LineID", 1) { IsUnique = true }));

			modelBuilder.Entity<R>()
				.Property(r => r.LineID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Remittance_TransactionID_LineID", 2) { IsUnique = true }));

			modelBuilder.Entity<R>()
				.Property(r => r.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Remittance_CreationDate")));

			modelBuilder.Entity<R>()
				.HasMany(r => r.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("RemittancesToOwners"));

			#endregion
		}

		#endregion
	}
}
