using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Masters.Commands;
using Ncs.Cqrs.Application.Features.Masters.DTOs;
using Ncs.Cqrs.Application.Features.Masters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Cqrs.Api.Controllers
{
    [Route("api/masters")]
    public class MastersController : BaseApiController
    {
        private readonly IMediator _mediator;
        public MastersController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _mediator = mediator;
        }

        /// <summary> Retrieve all companies. </summary>
        [HttpGet("companies")]
        public async Task<ActionResult<ResponseDto<IEnumerable<CompaniesDto>>>> GetCompanies()
        {
            var result = await _mediator.Send(new GetCompaniesAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve all personal types. </summary>
        [HttpGet("personal-types")]
        public async Task<ActionResult<ResponseDto<IEnumerable<PersonalIdTypeDto>>>> GetPersonalTypes()
        {
            var result = await _mediator.Send(new GetPersonalTypeAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve all reservation statuses. </summary>
        [HttpGet("reservation-status")]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsStatusDto>>>> GetReservationStatuses()
        {
            var result = await _mediator.Send(new GetReservationsStatusAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve all roles. </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<ResponseDto<IEnumerable<RolesDto>>>> GetRoles()
        {
            var result = await _mediator.Send(new GetRolesAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve all vendors. </summary>
        [HttpGet("vendors")]
        public async Task<ActionResult<ResponseDto<IEnumerable<VendorsDto>>>> GetVendors()
        {
            var result = await _mediator.Send(new GetVendorsAllQuery());
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Retrieve vendor by ID. </summary>
        [HttpGet("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<VendorsDto>>> GetVendorById(int id)
        {
            var result = await _mediator.Send(new GetVendorsByIdQuery { VendorId = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Add a new vendor. </summary>
        [HttpPost("vendors")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateVendor([FromBody] CreateVendorsDto data)
        {
            var result = await _mediator.Send(new CreateVendorsCommand
            {
                Name = data.Name,
                ContactInfo = data.ContactInfo,
                Email = data.Email,
                Address = data.Address,
                PhoneNumber = data.PhoneNumber
            });

            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Update a vendor. </summary>
        [HttpPut("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateVendor(int id, [FromBody] UpdateVendorsDto data)
        {
            var result = await _mediator.Send(new UpdateVendorsCommand
            {
                Id = id,
                Name = data.Name,
                ContactInfo = data.ContactInfo,
                Email = data.Email,
                Address = data.Address,
                PhoneNumber = data.PhoneNumber
            });

            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

        /// <summary> Delete a vendor by ID. </summary>
        [HttpDelete("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteVendor(int id)
        {
            var result = await _mediator.Send(new DeleteVendorsCommand { Id = id });
            return result.Success ? Ok(result) : HandleErrorResponse(result);
        }

    }
}
