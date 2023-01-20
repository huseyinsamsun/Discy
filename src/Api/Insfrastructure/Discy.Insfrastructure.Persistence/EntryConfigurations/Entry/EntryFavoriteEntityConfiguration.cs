using Discy.Api.Domain.Models;
using Discy.Insfrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Insfrastructure.Persistence.EntryConfigurations.Entry
{
    public class EntryFavoriteEntityConfiguration : BaseEntityConfiguration<EntryFavorite>
    {
        public override void Configure(EntityTypeBuilder<EntryFavorite> builder)
        {
            base.Configure(builder);
            builder.ToTable("entryfavorite", DiscyContext.DEFAULT_SCHEMA);

            builder.HasOne(i => i.Entry).WithMany(i => i.EntryFavorites).HasForeignKey(i => i.EntryId);
			builder.HasOne(i => i.CreatedUser).WithMany(i => i.EntryFavorites).HasForeignKey(i => i.CreatedById).OnDelete(DeleteBehavior.Restrict);
		}
    }
}
