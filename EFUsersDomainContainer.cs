using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grammophone.DataAccess;
using Grammophone.DataAccess.EntityFramework;
using Grammophone.Domos.Domain;
using Grammophone.Domos.Domain.Files;

namespace Grammophone.Domos.DataAccess.EntityFramework
{
	/// <summary>
	/// Entity Framework implementation of a Domos repository,
	/// containing users, roles, managers and permissions.
	/// </summary>
	/// <typeparam name="U">
	/// The type of users. Must be derived from <see cref="User"/>.
	/// </typeparam>
	/// <remarks>
	/// The global cascade delete convention is turned off. When needed, please enable
	/// cascade delete on a per entity basis by overriding <see cref="OnModelCreating(DbModelBuilder)"/>.
	/// </remarks>
	public class EFUsersDomainContainer<U> : EFDomainContainer, IUsersDomainContainer<U>
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
		public EFUsersDomainContainer()
		{
		}

		/// <summary>
		/// Constructs a new container instance using conventions to 
		/// create the name of the database to which a connection will be made.
		/// The by-convention name is the full name (namespace + class name)
		/// of the derived container class.
		/// </summary>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFUsersDomainContainer(TransactionMode transactionMode)
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
		public EFUsersDomainContainer(string nameOrConnectionString)
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
		public EFUsersDomainContainer(string nameOrConnectionString, TransactionMode transactionMode)
			: base(nameOrConnectionString, transactionMode)
		{
		}

		/// <summary>
		/// Constructs a new container instance using a given connection.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <param name="ownTheConnection">If true, hand over connection ownership to the container.</param>
		/// <param name="transactionMode">The transaction behavior.</param>
		public EFUsersDomainContainer(System.Data.Common.DbConnection connection, bool ownTheConnection, TransactionMode transactionMode)
			: base(connection, ownTheConnection, transactionMode)
		{
		}

		#endregion

		#region Public properties

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
		/// Entity set of dispositions in the system.
		/// </summary>
		public IDbSet<Disposition> Dispositions { get; set; }

		/// <summary>
		/// The MIME content types in the system.
		/// </summary>
		public IDbSet<ContentType> ContentTypes { get; set; }

		/// <summary>
		/// The disposition types in the system.
		/// </summary>
		public IDbSet<DispositionType> DispositionTypes { get; set; }

		#endregion

		#region Protected methods

		/// <summary>
		/// Add indexes and other non-implied database artifacts.
		/// </summary>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			#region Global conventions

			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			#endregion

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

			modelBuilder.Entity<User>()
				.HasMany(u => u.Roles)
				.WithMany()
				.Map(m => m.ToTable("UsersToRoles"));

			modelBuilder.Entity<User>()
				.HasMany(u => u.Dispositions)
				.WithRequired(d => d.OwningUser);

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

			#region ContentType

			modelBuilder.Entity<ContentType>()
				.Property(ct => ct.MIME)
				.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_ContentType_MIME") { IsUnique = true }));

			#endregion

			#region File

			// File is abstract; derive from it and define your own entity set.
			modelBuilder.Ignore<File>();

			#endregion
		}

		#endregion
	}
}
