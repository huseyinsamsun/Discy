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
    public class EntryCommentEntityConfiguration : BaseEntityConfiguration<EntryComment>
    {
        public override void Configure(EntityTypeBuilder<EntryComment> builder)
        {
            base.Configure(builder);
            builder.ToTable("entrycomment", DiscyContext.DEFAULT_SCHEMA);

            builder.HasOne(i => i.CreatedBy).WithMany(i => i.EntryComments).HasForeignKey(i => i.CreatedById);
            builder.HasOne(i => i.Entry).WithMany(i => i.EntryComments).HasForeignKey(i => i.EntryId);

        }
    }
}

