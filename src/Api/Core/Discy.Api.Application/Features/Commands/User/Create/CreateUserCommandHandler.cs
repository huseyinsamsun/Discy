using AutoMapper;
using Discy.Api.Application.Repositories;
using Discy.Api.Domain.Models;
using Discy.Common;
using Discy.Common.Events.User;
using Discy.Common.Infrastructure;
using Discy.Common.Infrastructure.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discy.Api.Application.Features.Commands.User.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetAll();
            var existsUser = await userRepository.GetSingleAsync(i => i.EmailAddress == request.EmailAddress);
            if (existsUser is not null)
            {
                throw new DatabaseValidationException("User already exists!");

            }
            var dbUser = mapper.Map<Domain.Models.User>(request);
            var rows = await userRepository.AddAsync(dbUser);
            if(rows>0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAddress = null,
                    NewEmailAddress = dbUser.EmailAddress
                };
                QueueFactory.SendMessageToExchange(exchangeName: DiscyConstants.UserExchangeName, obj: @event, queueName:DiscyConstants.UserEmailChangedQueueName, exchangeType: DiscyConstants.DefaultExchangeType);
            }
            //Email Changed/Created

            return dbUser.Id;



        }
    }
}
