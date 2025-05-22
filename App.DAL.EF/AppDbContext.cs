using App.Domain;
using App.Domain.Identity;
using Base.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole,
    IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<Contact> Contacts { get; set; } = default!;
    public DbSet<ContactType> ContactTypes { get; set; } = default!;
    public DbSet<Person> Persons { get; set; } = default!;
    public DbSet<Department> Departments { get; set; } = default!;
    public DbSet<DepartmentPerson> DepartmentPersons { get; set; } = default!;
    
    public DbSet<Tag> Tags { get; set; } = default!;
    public DbSet<Post> Posts { get; set; } = default!;
    public DbSet<PostTag> PostTags { get; set; } = default!;
    public DbSet<PostDepartment> PostDepartments { get; set; } = default!;
    

    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;

    private readonly IUserNameResolver _userNameResolver;
    private readonly ILogger<AppDbContext> _logger;


    public AppDbContext(DbContextOptions<AppDbContext> options, IUserNameResolver userNameResolver,
        ILogger<AppDbContext> logger)
        : base(options)
    {
        _userNameResolver = userNameResolver;
        _logger = logger;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // remove cascade delete
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        /*
        // TODO - adding primary key to AppUserRole causes issues with RoleManager
        // We have custom UserRole - with separate PK and navigation for Role and User
        // override default Identity EF config
        builder.Entity<AppUserRole>().HasKey(a => new { a.UserId, a.RoleId });
        builder.Entity<AppUserRole>().HasAlternateKey(a => a.Id);
        builder.Entity<AppUserRole>().HasIndex(a => new { a.UserId, a.RoleId }).IsUnique();
        */

        builder.Entity<AppUserRole>()
            .HasOne(a => a.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(a => a.UserId);

        builder.Entity<AppUserRole>()
            .HasOne(a => a.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(a => a.RoleId);
        // Configure Department entity
        builder.Entity<Department>()
            .HasOne(d => d.Manager)
            .WithMany()
            .HasForeignKey(d => d.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure DepartmentPerson entity
        builder.Entity<DepartmentPerson>()
            .HasOne(dp => dp.Department)
            .WithMany(d => d.DepartmentPersons)
            .HasForeignKey(dp => dp.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    
        builder.Entity<DepartmentPerson>()
            .HasOne(dp => dp.Person)
            .WithMany(p => p.DepartmentPersons)
            .HasForeignKey(dp => dp.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Tag>()
            .HasIndex(t => t.Title)
            .IsUnique(); // Ensure unique tag titles

        builder.Entity<PostTag>()
            .HasIndex(pt => new { pt.PostId, pt.TagId })
            .IsUnique();
        
        builder.Entity<PostTag>()
            .HasOne(pt => pt.Post)
            .WithMany(p => p.PostTags)
            .HasForeignKey(pt => pt.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PostTag>()
            .HasOne(pt => pt.Tag)
            .WithMany()
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
        
        
        builder.Entity<PostDepartment>()
            .HasIndex(pd => new { pd.PostId, pd.DepartmentId })
            .IsUnique();
        
        builder.Entity<PostDepartment>()
            .HasOne(pd => pd.Post)
            .WithMany(p => p.PostDepartments)
            .HasForeignKey(pd => pd.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PostDepartment>()
            .HasOne(pd => pd.Department)
            .WithMany()
            .HasForeignKey(pd => pd.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var addedEntries = ChangeTracker.Entries()
            ;
        foreach (var entry in addedEntries)
        {
            if (entry is { Entity: IDomainMeta })
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        (entry.Entity as IDomainMeta)!.CreatedAt = DateTime.UtcNow;
                        (entry.Entity as IDomainMeta)!.CreatedBy = _userNameResolver.CurrentUserName;
                        break;
                    case EntityState.Modified:
                        entry.Property("ChangedAt").IsModified = true;
                        entry.Property("ChangedBy").IsModified = true;
                        (entry.Entity as IDomainMeta)!.ChangedAt = DateTime.UtcNow;
                        (entry.Entity as IDomainMeta)!.ChangedBy = _userNameResolver.CurrentUserName;

                        // Prevent overwriting CreatedBy/CreatedAt on update
                        entry.Property("CreatedAt").IsModified = false;
                        entry.Property("CreatedBy").IsModified = false;
                        break;
                }
            }

            if (entry is { Entity: IDomainUserId, State: EntityState.Modified })
            {
                // do not allow userid modification
                entry.Property("UserId").IsModified = false;
                _logger.LogWarning("UserId modification attempt. Denied!");
            }
        }


        return base.SaveChangesAsync(cancellationToken);
    }
}