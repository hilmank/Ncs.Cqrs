using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UsersController(
            IMediator mediator,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MastersController> logger)
           : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve all users", Description = "Gets all users from the system.")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetAllUsers()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByParamQuery()),
                "Error fetching all users"
            );

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve a user by ID", Description = "Gets a specific user by ID.")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserById(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByIdQuery { Id = id }),
                $"Error fetching user with ID {id}"
            );

        [HttpGet("roles/{roleId}")]
        [SwaggerOperation(Summary = "Retrieve users by role", Description = "Gets users associated with a specific role.")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByRole(int roleId)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByParamQuery { RolesId = roleId }),
                $"Error fetching users for role ID {roleId}"
            );

        [HttpGet("company/{companyId}")]
        [SwaggerOperation(Summary = "Retrieve users by company", Description = "Gets users associated with a specific company.")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByCompany(int companyId)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByParamQuery { CompanyId = companyId }),
                $"Error fetching users for company ID {companyId}"
            );

        [HttpGet("status/{isActive}")]
        [SwaggerOperation(Summary = "Retrieve users by status", Description = "Gets active or inactive users.")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByStatus(bool isActive)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByParamQuery { IsAcvtive = isActive }),
                $"Error fetching users with status {isActive}"
            );

        [HttpGet("rfid/{rfidTag}")]
        [SwaggerOperation(Summary = "Retrieve a user by RFID tag", Description = "Gets a user using their RFID tag.")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByRfidTag(string rfidTag)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByrfidtTagQuery { RfidTag = rfidTag }),
                $"Error fetching user with RFID tag {rfidTag}"
            );

        [HttpGet("by-username")]
        [SwaggerOperation(Summary = "Retrieve a user by username", Description = "Gets a user by their username.")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByUsername([FromQuery] string username)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByUsernameOrEmailQuery { UsernameOrEmail = username }),
                $"Error fetching user with username {username}"
            );

        [HttpGet("by-email")]
        [SwaggerOperation(Summary = "Retrieve a user by email", Description = "Gets a user by their email address.")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByEmail([FromQuery] string email)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetUsersByUsernameOrEmailQuery { UsernameOrEmail = email }),
                $"Error fetching user with email {email}"
            );

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Registers a new user in the system.")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateUser([FromBody] CreateUsersDto data)
            => await HandleRequestAsync(
                async () => await _mediator.Send(_mapper.Map<CreateUsersCommand>(data)),
                "Error creating user"
            );

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a user", Description = "Updates user information.")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateUser(int id, [FromBody] UpdateUsersDto data)
            => await HandleRequestAsync(
                async () =>
                {
                    var command = _mapper.Map<UpdateUsersCommand>(data);
                    command.Id = id;
                    return await _mediator.Send(command);
                },
                $"Error updating user with ID {id}"
            );

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a user by ID", Description = "Removes a user from the system.")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteUser(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new DeleteUsersCommand { Id = id }),
                $"Error deleting user with ID {id}"
            );

        [HttpPut("activate/{id}")]
        [SwaggerOperation(Summary = "Activate a user", Description = "Sets the user's status to active.")]
        public async Task<ActionResult<ResponseDto<bool>>> ActivateUser(int id, string rfidTag)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeUsersStatusCommand { Id = id, RfidTag = rfidTag, IsActive = true }),
                $"Error activating user with ID {id}"
            );

        [HttpPut("deactivate/{id}")]
        [SwaggerOperation(Summary = "Deactivate a user", Description = "Sets the user's status to inactive.")]
        public async Task<ActionResult<ResponseDto<bool>>> DeactivateUser(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new ChangeUsersStatusCommand { Id = id, IsActive = false }),
                $"Error deactivating user with ID {id}"
            );

        [HttpPost("change-password")]
        [SwaggerOperation(Summary = "Change a user's password", Description = "Updates a user's password.")]
        public async Task<ActionResult<ResponseDto<bool>>> ChangePassword([FromBody] ChangePasswordDto data)
            => await HandleRequestAsync(
                async () => await _mediator.Send(_mapper.Map<ChangePasswordCommand>(data)),
                "Error changing password"
            );
    }
}
