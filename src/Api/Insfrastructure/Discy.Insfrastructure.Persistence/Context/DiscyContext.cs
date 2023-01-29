using Discy.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Insfrastructure.Persistence.Context
{
    public class DiscyContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "dbo";
        private readonly IConfiguration _configuration;
        public DiscyContext()
        {

        }
        public DiscyContext(DbContextOptions options) : base(options)
        {

        }



        public DbSet<User> Users { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<EntryComment> EntryComments { get; set; }
        public DbSet<EntryFavorite> EntryFavorites { get; set; }
        public DbSet<EntryVote> EntryVotes { get; set; }
        public DbSet<EntryCommentVote> EntryCommentVotes { get; set; }
        public DbSet<EntryCommentFavorite> EntryCommentFavorites { get; set; }
        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                

                var connStr = "Server=localhost\\SQLEXPRESS;Database=Discy;Integrated Security=true";
                optionsBuilder.UseSqlServer(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
        public override int SaveChanges()
        {
            OnBeforeSave();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        private void OnBeforeSave()
        {
            var addedEntites = ChangeTracker.Entries().Where(i => i.State == EntityState.Added).Select(i => (BaseEntity)i.Entity);
            PrepareAddedEntities(addedEntites);

        }
        private void PrepareAddedEntities(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.CreateDate == DateTime.MinValue)
                    entity.CreateDate = DateTime.Now;
            }
        }



    }
}
