using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grammophone.DataAccess;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;

namespace Grammophone.Domos.DataAccess.EntityFramework
{
	/// <summary>
	/// Entity Framework implementation of a Domos repository,
	/// containing users, roles, managers and permissions.
	/// </summary>
	/// <typeparam name="U">
	/// The type of users. Must be derived from <see cref="User"/>.
	/// </typeparam>
	/// <typeparam name="BST">
	/// The base type of the system's state transitions, derived from <see cref="StateTransition{U}"/>.
	/// </typeparam>
	/// <remarks>
	/// The global cascade delete convention is turned off. When needed, please enable
	/// cascade delete on a per entity basis by overriding <see cref="OnModelCreating(DbModelBuilder)"/>.
	/// </remarks>
	public class EFWorkflowUsersDomainContainer<U, BST> 
		: EFUsersDomainContainer<U>, IWorkflowUsersDomainContainer<U, BST>
		where U : User
		where BST : StateTransition<U>
	{
		#region Construction

		/// <summary>
		/// Constructs a new container instance using conventions to 
		/// create the name of the database to which a connection will be made.
		/// The by-convention name is the full name (namespace + class name)
		/// of the derived container class.
		/// The <see cref="TransactionMode"/> is set to <see cref="TransactionMode.Real"/>.
		/// </summary>
		public EFWorkflowUsersDomainContainer()
		{
		}

		/// <summary>
		/// Constructs a new container instance using conventions to 
		/// create the name of the database to which a connection will be made.
		/// The by-convention name is the full name (namespace + class name)
		/// of the derived container class.
		/// </summary>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFWorkflowUsersDomainContainer(TransactionMode transactionMode)
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
		public EFWorkflowUsersDomainContainer(string nameOrConnectionString)
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
		public EFWorkflowUsersDomainContainer(string nameOrConnectionString, TransactionMode transactionMode)
			: base(nameOrConnectionString, transactionMode)
		{
		}

		/// <summary>
		/// Constructs a new container instance using a given connection.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="ownTheConnection">If true, hand over connection ownership to the container.</param>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFWorkflowUsersDomainContainer(System.Data.Common.DbConnection connection, bool ownTheConnection, TransactionMode transactionMode)
			: base(connection, ownTheConnection, transactionMode)
		{
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Entity set of workflow states in the system.
		/// </summary>
		public IDbSet<State> States { get; set; }

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
		public IDbSet<BST> StateTransitions { get; set; }

		/// <summary>
		/// Entity set of workflow graphs in the system.
		/// </summary>
		public IDbSet<WorkflowGraph> WorkflowGraphs { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Add indexes and other non-implied database artifacts.
		/// </summary>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region WorkflowGraph

			modelBuilder.Entity<WorkflowGraph>()
				.HasIndex(wg => wg.CodeName)
				.IsUnique(true);

			#endregion

			#region StateGroup

			//modelBuilder.Entity<StateGroup>()
			//	.HasIndex(sg => new { sg.WorkflowGraphID, sg.CodeName })
			//	.IsUnique(true);

			modelBuilder.Entity<StateGroup>()
				.HasIndex(sg => sg.CodeName);

			#endregion

			#region State

			//modelBuilder.Entity<State>()
			//	.HasIndex(s => new { s.GroupID, s.CodeName })
			//	.IsUnique(true);

			modelBuilder.Entity<State>()
				.HasIndex(s => s.CodeName);

			#endregion

			#region StatePath

			modelBuilder.Entity<StatePath>()
				.Property(sp => sp.CodeName)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_StatePath_CodeName") { IsUnique = true }));

			#endregion
		}

		#endregion
	}
}
