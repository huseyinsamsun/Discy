using Discy.Api.Domain.Models;
using Discy.Insfrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Insfrastructure.Persistence.EntryConfigurations
{
    public class EntryCommentFavoriteEntityConfiguration : BaseEntityConfiguration<EntryCommentFavorite>
    {
        public override void Configure(EntityTypeBuilder<EntryCommentFavorite> builder)
        {
            base.Configure(builder);
            builder.ToTable("entrycommentfavorite", DiscyContext.DEFAULT_SCHEMA);

            builder.HasOne(i => i.EntryComment).WithMany(i => i.EntryCommentFavorites).HasForeignKey(i => i.EntryCommentId);
            builder.HasOne(i => i.CreatedUser).WithMany(i => i.EntryCommentFavorites).HasForeignKey(i => i.CreatedById).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
