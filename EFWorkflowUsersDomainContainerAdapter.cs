using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFramework;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Workflow;

namespace Grammophone.Domos.DataAccess.EntityFramework
{
	/// <summary>
	/// Adapts an Entity Framework workflow users domain container to <see cref="IWorkflowUsersDomainContainer{U, BST}"/>.
	/// </summary>
	public class EFWorkflowUsersDomainContainerAdapter<U, BST, D> : EFUsersDomainContainerAdapter<U, D>, IWorkflowUsersDomainContainer<U, BST>
		where U : User
		where BST : StateTransition<U>
		where D : EFWorkflowUsersDomainContainer<U, BST>
	{
		#region Private fields

		private IEntitySet<State> states;

		private IEntitySet<StateGroup> stateGroups;

		private IEntitySet<StatePath> statePaths;

		private IEntitySet<BST> stateTransitions;

		private IEntitySet<WorkflowGraph> workflowGraphs;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF domain container.</param>
		public EFWorkflowUsersDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IWorkflowUsersDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<State> States => states ??= new EFSet<State>(this.InnerDomainContainer.States, this);

		/// <inheritdoc/>
		public IEntitySet<StateGroup> StateGroups => stateGroups ??= new EFSet<StateGroup>(this.InnerDomainContainer.StateGroups, this);

		/// <inheritdoc/>
		public IEntitySet<StatePath> StatePaths => statePaths ??= new EFSet<StatePath>(this.InnerDomainContainer.StatePaths, this);

		/// <inheritdoc/>
		public IEntitySet<BST> StateTransitions => stateTransitions ??= new EFSet<BST>(this.InnerDomainContainer.StateTransitions, this);

		/// <inheritdoc/>
		public IEntitySet<WorkflowGraph> WorkflowGraphs => workflowGraphs ??= new EFSet<WorkflowGraph>(this.InnerDomainContainer.WorkflowGraphs, this);

		#endregion
	}
}
