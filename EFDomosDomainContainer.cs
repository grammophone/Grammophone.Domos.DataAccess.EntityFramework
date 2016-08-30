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
	public abstract class EFDomosDomainContainer<U> : EFDomainContainer, IDomosDomainContainer<U>
		where U : User
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

		#region Public repository properties

		#region Users, segregations, dispositions and security

		/// <summary>
		/// Entity set of users in the system.
		/// </summary>
		public IDbSet<U> Users { get; set; }

		/// <summary>
		/// Entity set of registrations in the system.
		/// </summary>
		public IDbSet<Registration> Registrations { get; set; }

		/// <summary>
		/// Entity set of roles in the system.
		/// </summary>
		public IDbSet<Role> Roles { get; set; }

		/// <summary>
		/// Entity set of entity accesses in the system,
		/// only used if defined in database.
		/// </summary>
		public IDbSet<EntityAccess> EntityAccesses { get; set; }

		/// <summary>
		/// Entity set of manager accesses in the system,
		/// only used if defined in database.
		/// </summary>
		public IDbSet<ManagerAccess> ManagerAccesses { get; set; }

		/// <summary>
		/// Entity set of permissions in the system,
		/// only used if defined in database.
		/// </summary>
		public IDbSet<Permission> Permissions { get; set; }

		/// <summary>
		/// Entity set of segregations in the system.
		/// </summary>
		public IDbSet<Segregation<U>> Segregations { get; set; }

		/// <summary>
		/// Entity set of dispositions in the system.
		/// </summary>
		public IDbSet<Disposition<U>> Dispositions { get; set; }

		#endregion

		#region Accounting

		/// <summary>
		/// Entity set of accounts in the system.
		/// </summary>
		public IDbSet<Account<U>> Accounts { get; set; }

		/// <summary>
		/// Entity set of accounting batches in the system.
		/// </summary>
		public IDbSet<Batch<U>> Batches { get; set; }

		/// <summary>
		/// Entity set of remittances belonging to a batch in the system.
		/// </summary>
		public IDbSet<BatchRemittance<U>> BatchRemittances { get; set; }

		/// <summary>
		/// Entity set of credit systems in the system.
		/// </summary>
		public IDbSet<CreditSystem> CreditSystems { get; set; }

		/// <summary>
		/// Entity set of accounting journals in the system.
		/// </summary>
		public IDbSet<Journal<U>> Journals { get; set; }

		/// <summary>
		/// Entity set of accounting journal lines in the system.
		/// </summary>
		public IDbSet<JournalLine<U>> JournalLines { get; set; }

		#endregion

		#region Workflow

		/// <summary>
		/// Entity set of state transition attachments in the system.
		/// </summary>
		public IDbSet<Attachment<U>> Attachments { get; set; }

		/// <summary>
		/// Entity set of contents of state transition attachments in the system.
		/// </summary>
		public IDbSet<AttachmentContent<U>> AttachmentContents { get; set; }

		/// <summary>
		/// Entity set of workflow states in the system.
		/// </summary>
		public IDbSet<State> State { get; set; }

		/// <summary>
		/// Entity set of workflow state groups in the system.
		/// </summary>
		public IDbSet<StateGroup> StateGroups { get; set; }

		/// <summary>
		/// Entity set of workflow state paths in the system.
		/// </summary>
		public IDbSet<StatePath> StatePaths { get; set; }

		/// <summary>
		/// Entity set of transitions occurred between workflow states in the system.
		/// </summary>
		public IDbSet<StateTransition<U>> StateTransitions { get; set; }

		/// <summary>
		/// Entity set of workflow graphs in the system.
		/// </summary>
		public IDbSet<WorkflowGraph> WorkflowGraphs { get; set; }

		#endregion

		#endregion

		#region Protected methods

		/// <summary>
		/// Add indexes and other non-implied database artifacts.
		/// </summary>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Main entities setup

			#region User

			modelBuilder.Entity<U>()
				.Property(u => u.Email)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_User_Email") { IsUnique = true }));

			modelBuilder.Entity<U>()
				.Property(u => u.UserName)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_User_UserName") { IsUnique = true }));

			modelBuilder.Entity<U>()
				.Property(u => u.CreationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_User_CreationDate")));

			modelBuilder.Entity<U>()
				.HasMany(u => u.Dispositions)
				.WithRequired()
				.HasForeignKey(d => d.UserID);

			#endregion

			#region Registration

			modelBuilder.Entity<Registration>()
				.Property(r => r.Provider)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Registration_Provider_ProviderKey", 1) { IsUnique = true }));

			modelBuilder.Entity<Registration>()
				.Property(r => r.ProviderKey)
				.HasMaxLength(128)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Registration_Provider_ProviderKey", 2) { IsUnique = true }));

			#endregion

			#region Disposition

			modelBuilder.Entity<Disposition>()
				.Property(d => d.UserID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Disposition_User")));

			#endregion

			#endregion

			#region Accounting entitites setup

			#region Account

			modelBuilder.Entity<UserGroupAccount<U>>()
				.HasMany(a => a.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedAccounts"));

			#endregion

			#region Batch

			modelBuilder.Entity<Batch<U>>()
				.HasMany(b => b.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedBatches"));

			#endregion

			#region BatchRemittance

			modelBuilder.Entity<BatchRemittance<U>>()
				.HasMany(br => br.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedBatchRemittances"));

			#endregion

			#region Journal

			modelBuilder.Entity<Journal<U>>()
				.HasMany(j => j.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedJournals"));

			modelBuilder.Entity<Journal<U>>()
				.Property(j => j.LastModificationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Journal_LastModificationDate")));

			modelBuilder.Entity<Journal<U>>()
				.Property(j => j.Type)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Journal_Type_LastModificationDate", 1)));

			modelBuilder.Entity<Journal<U>>()
				.Property(j => j.LastModificationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Journal_Type_LastModificationDate", 2)));

			#endregion

			#region JournalLine

			modelBuilder.Entity<JournalLine<U>>()
				.HasMany(jl => jl.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedJournalLines"));

			modelBuilder.Entity<JournalLine<U>>()
				.Property(jl => jl.LastModificationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_JournalLine_LastModificationDate")));

			modelBuilder.Entity<JournalLine<U>>()
				.Property(jl => jl.Type)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_JournalLine_Type_LastModificationDate", 1)));

			modelBuilder.Entity<JournalLine<U>>()
				.Property(jl => jl.LastModificationDate)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_JournalLine_Type_LastModificationDate", 2)));

			#endregion

			#endregion

			#region Workflow entities setup

			#region Attachment

			modelBuilder.Entity<StateTransition<U>>()
				.HasMany(st => st.OwningUsers)
				.WithMany()
				.Map(m => m.ToTable("OwnedStateTransitions"));

			modelBuilder.Entity<StateTransition<U>>()
				.Property(st => st.PathID)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_StateTransition_Path")));

			#endregion

			#endregion

		}

		#endregion
	}
}
