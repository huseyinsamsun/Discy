using AutoMapper;
using Discy.Api.Application.Repositories;
using Discy.Api.Domain.Models;
using Discy.Common.Events.User;
using Discy.Common.Infrastructure;
using Discy.Common;
using Discy.Common.Infrastructure.Exceptions;
using Discy.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Api.Application.Features.Commands.User.Update
{
    public class UpdateUserCommandHandler:IRequestHandler<UpdateUserCommand,Guid>
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public UpdateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dbUser = await userRepository.GetByIdAsync(request.Id);
          
            if (dbUser is null)
            {
                throw new DatabaseValidationException("User not found");
            }
            var dbEmailAddress = dbUser.EmailAddress;
            var emailChanged = string
                .CompareOrdinal(dbEmailAddress, request.EmailAddress) != 0;

            // dbUser = mapper.Map<Domain.Models.User>(request); bir tane yaratmak yerine 
            mapper.Map(request, dbUser);//olan datayla işlem yapıyorum
            var rows = await userRepository.UpdateAsync(dbUser);

            //check if email changed

            if (emailChanged&& rows > 0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAddress = null,
                    NewEmailAddress = dbUser.EmailAddress
                };
                QueueFactory.SendMessageToExchange(exchangeName: DiscyConstants.UserExchangeName, obj: @event, queueName: DiscyConstants.UserEmailChangedQueueName, exchangeType: DiscyConstants.DefaultExchangeType);
                dbUser.EmailConfirmed = false;
                await userRepository.UpdateAsync(dbUser);

            }
            return dbUser.Id;
         
        
        }
    }
}
