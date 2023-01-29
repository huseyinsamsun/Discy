using Discy.Api.Application.Features.Commands.User.Create;
using Discy.Common.ViewModels.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discy.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]LoginUserCommand command)
        {
            var res = await mediator.Send(command);
            return Ok(res);
        }
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var guid = await mediator.Send(command);
            return Ok(guid);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] LoginUserCommand command)
        {
            var guid= await mediator.Send(command);
            return Ok(guid);
        }

    }
}
