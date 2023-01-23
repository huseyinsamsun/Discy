using Discy.Api.Application.Repositories;
using Discy.Api.Domain.Models;
using Discy.Insfrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Insfrastructure.Persistence.Repositories
{
    public class EntryVoteRepository : GenericRepository<EntryVote>, IEntryVoteRepository
    {
        public EntryVoteRepository(DiscyContext discyContext) : base(discyContext)
        {
        }
    }
}
