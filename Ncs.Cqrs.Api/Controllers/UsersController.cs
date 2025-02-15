using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Users.Commands;
using Ncs.Cqrs.Application.Features.Users.DTOs;
using Ncs.Cqrs.Application.Features.Users.Queries;
using Ncs.Cqrs.Domain.Constants;
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

        public UsersController(IMediator mediator, IMapper mapper, IHttpContextAccessor httpContextAccessor)
           : base(httpContextAccessor)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary> Retrieve all users. </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetAllUsers()
        {
            var result = await _mediator.Send(new GetUsersByParamQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve a user by ID. </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserById(int id)
        {
            var result = await _mediator.Send(new GetUsersByIdQuery { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve users by role. </summary>
        [HttpGet("roles/{roleId}")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByRole(int roleId)
        {
            var result = await _mediator.Send(new GetUsersByParamQuery { RolesId = roleId });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve users by company. </summary>
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByCompany(int companyId)
        {
            var result = await _mediator.Send(new GetUsersByParamQuery { CompanyId = companyId });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve users by status. </summary>
        [HttpGet("status/{isActive}")]
        public async Task<ActionResult<ResponseDto<IEnumerable<UsersDto>>>> GetUsersByStatus(bool isActive)
        {
            var result = await _mediator.Send(new GetUsersByParamQuery { IsAcvtive = isActive });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve a user by RFID tag. </summary>
        [HttpGet("rfid/{rfidTag}")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByRfidTag(string rfidTag)
        {
            var result = await _mediator.Send(new GetUsersByrfidtTagQuery { RfidTag = rfidTag });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve a user by username. </summary>
        [HttpGet("by-username")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByUsername([FromQuery] string username)
        {
            var result = await _mediator.Send(new GetUsersByUsernameOrEmailQuery { UsernameOrEmail = username });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve a user by email. </summary>
        [HttpGet("by-email")]
        public async Task<ActionResult<ResponseDto<UsersDto>>> GetUserByEmail([FromQuery] string email)
        {
            var result = await _mediator.Send(new GetUsersByUsernameOrEmailQuery { UsernameOrEmail = email });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Create a new user. </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new user", Description = "Registers a new user in the system.")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateUser([FromBody] CreateUsersDto data)
        {
            var result = await _mediator.Send(_mapper.Map<CreateUsersCommand>(data));
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Update an existing user. </summary>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a user", Description = "Updates user information.")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateUser(int id, [FromBody] UpdateUsersDto data)
        {
            var command = _mapper.Map<UpdateUsersCommand>(data);
            command.Id = id;
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Delete a user by ID. </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteUser(int id)
        {
            var result = await _mediator.Send(new DeleteUsersCommand { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
        /// <summary> 
        /// Activates a user by setting their status to active.
        /// </summary>
        /// <param name="id">The ID of the user to activate.</param>
        /// <param name="rfidTag">The RFID tag assigned to the user. Required when activating.</param>
        /// <returns>Returns a success response if the user is activated, otherwise an error response.</returns>
        [HttpPut("activate/{id}")]
        [SwaggerOperation(Summary = "Activate a user", Description = "Sets the user's status to active.")]
        public async Task<ActionResult<ResponseDto<bool>>> ActivateUser(int id, string rfidTag)
        {
            var command = new ChangeUsersStatusCommand { Id = id, RfidTag = rfidTag, IsActive = true };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Deactivate a user. </summary>
        [HttpPut("deactivate/{id}")]
        [SwaggerOperation(Summary = "Deactivate a user", Description = "Sets the user's status to inactive.")]
        public async Task<ActionResult<ResponseDto<bool>>> DeactivateUser(int id)
        {
            var command = new ChangeUsersStatusCommand { Id = id, IsActive = false };
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Changes the user's password. </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult<ResponseDto<bool>>> ChangePassword([FromBody] ChangePasswordDto data)
        {
            var command = _mapper.Map<ChangePasswordCommand>(data);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }
    }
}
