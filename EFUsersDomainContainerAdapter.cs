using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFramework;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Files;

namespace Grammophone.Domos.DataAccess.EntityFramework
{
	/// <summary>
	/// Adapts an Entity Framework users domain container to <see cref="IUsersDomainContainer{U}"/>.
	/// </summary>
	public class EFUsersDomainContainerAdapter<U, D> : EFDomainContainerAdapter<D>, IUsersDomainContainer<U>
		where U : User
		where D : EFUsersDomainContainer<U>
	{
		#region Private fields

		private IEntitySet<U> users;

		private IEntitySet<Registration> registrations;

		private IEntitySet<Role> roles;

		private IEntitySet<Disposition> dispositions;

		private IEntitySet<ContentType> contentTypes;

		private IEntitySet<DispositionType> dispositionTypes;

		private IEntitySet<WebAuthnCredential> webAuthnCredentials;

		private IEntitySet<BrowserSession> browserSessions;

		private IEntitySet<ClientIpAddress> clientIpAddresses;

		#endregion

		#region Construction

		/// <summary>
		/// Create.
		/// </summary>
		/// <param name="innerContainer">The adapted EF domain container.</param>
		public EFUsersDomainContainerAdapter(D innerContainer)
			: base(innerContainer)
		{
		}

		#endregion

		#region IUsersDomainContainer implementation

		/// <inheritdoc/>
		public IEntitySet<U> Users => users ??= new EFSet<U>(this.InnerDomainContainer.Users, this);

		/// <inheritdoc/>
		public IEntitySet<Registration> Registrations => registrations ??= new EFSet<Registration>(this.InnerDomainContainer.Registrations, this);

		/// <inheritdoc/>
		public IEntitySet<Role> Roles => roles ??= new EFSet<Role>(this.InnerDomainContainer.Roles, this);

		/// <inheritdoc/>
		public IEntitySet<Disposition> Dispositions => dispositions ??= new EFSet<Disposition>(this.InnerDomainContainer.Dispositions, this);

		/// <inheritdoc/>
		public IEntitySet<ContentType> ContentTypes => contentTypes ??= new EFSet<ContentType>(this.InnerDomainContainer.ContentTypes, this);

		/// <inheritdoc/>
		public IEntitySet<DispositionType> DispositionTypes => dispositionTypes ??= new EFSet<DispositionType>(this.InnerDomainContainer.DispositionTypes, this);

		/// <inheritdoc/>
		public IEntitySet<WebAuthnCredential> WebAuthnCredentials => webAuthnCredentials ??= new EFSet<WebAuthnCredential>(this.InnerDomainContainer.WebAuthnCredentials, this);

		/// <inheritdoc/>
		public IEntitySet<BrowserSession> BrowserSessions => browserSessions ??= new EFSet<BrowserSession>(this.InnerDomainContainer.BrowserSessions, this);

		/// <inheritdoc/>
		public IEntitySet<ClientIpAddress> ClientIpAddresses => clientIpAddresses ??= new EFSet<ClientIpAddress>(this.InnerDomainContainer.ClientIpAddresses, this);

		#endregion
	}
}
