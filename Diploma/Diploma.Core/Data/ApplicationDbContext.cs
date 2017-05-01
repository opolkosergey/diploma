using Diploma.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<UserFolder> UserFolders { get; set; }

        public DbSet<Document> Documents { get; set; }
        
        public DbSet<DocumentAccess> DocumentAccesses { get; set; }

        public DbSet<UserKeys> UserKeys { get; set; }

        public DbSet<IncomingSignatureRequest> IncomingSignatureRequests { get; set; }

        public DbSet<AuditEntry> AuditEntries { get; set; }

        public DbSet<SignatureWarrant> SignatureWarrants { get; set; }

        public DbSet<UserTask> UserTasks { get; set; }

        public DbSet<DocumentFolder> DocumentFolders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasMany(x => x.UserFolders).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.ApplicationUserId);

            //builder.Entity<ApplicationUser>().HasOne(x => x.UserKeys).WithOne(x => x.ApplicationUser);

            builder.Entity<DocumentFolder>().HasKey(e => new {e.DocumentId, e.UserFolderId});

            builder.Entity<DocumentFolder>()
                .HasOne(x => x.Document)
                .WithMany(x => x.DocumentFolders)
                .HasForeignKey(x => x.DocumentId);

            builder.Entity<DocumentFolder>()
                .HasOne(x => x.UserFolder)
                .WithMany(x => x.DocumentFolders)
                .HasForeignKey(x => x.UserFolderId);

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-Diploma-c78bd18c-cb2e-4a11-b557-5b00ebc5bc91;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(builder);
        }
    }
}
