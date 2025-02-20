using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Features.Masters.Commands;
using Ncs.Cqrs.Application.Features.Masters.DTOs;
using Ncs.Cqrs.Application.Features.Masters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ncs.Cqrs.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/masters")]
    [ApiVersion("1.0")]
    public class MastersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MastersController(IMediator mediator, IHttpContextAccessor httpContextAccessor, ILogger<MastersController> logger)
            : base(httpContextAccessor, logger)
        {
            _mediator = mediator;
        }

        /// <summary> Retrieve all companies. </summary>
        [HttpGet("companies")]
        public async Task<ActionResult<ResponseDto<IEnumerable<CompaniesDto>>>> GetCompanies()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetCompaniesAllQuery()),
                "Error fetching companies"
            );

        /// <summary> Retrieve all personal types. </summary>
        [HttpGet("personal-types")]
        public async Task<ActionResult<ResponseDto<IEnumerable<PersonalIdTypeDto>>>> GetPersonalTypes()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetPersonalTypeAllQuery()),
                "Error fetching personal types"
            );

        /// <summary> Retrieve all reservation statuses. </summary>
        [HttpGet("reservation-status")]
        public async Task<ActionResult<ResponseDto<IEnumerable<ReservationsStatusDto>>>> GetReservationStatuses()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetReservationsStatusAllQuery()),
                "Error fetching reservation statuses"
            );

        /// <summary> Retrieve all roles. </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<ResponseDto<IEnumerable<RolesDto>>>> GetRoles()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetRolesAllQuery()),
                "Error fetching roles"
            );

        /// <summary> Retrieve all vendors. </summary>
        [HttpGet("vendors")]
        public async Task<ActionResult<ResponseDto<IEnumerable<VendorsDto>>>> GetVendors()
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetVendorsAllQuery()),
                "Error fetching vendors"
            );

        /// <summary> Retrieve vendor by ID. </summary>
        [HttpGet("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<VendorsDto>>> GetVendorById(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new GetVendorsByIdQuery { VendorId = id }),
                $"Error fetching vendor with ID {id}"
            );

        /// <summary> Add a new vendor. </summary>
        [HttpPost("vendors")]
        public async Task<ActionResult<ResponseDto<bool>>> CreateVendor([FromBody] CreateVendorsDto data)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new CreateVendorsCommand
                {
                    Name = data.Name,
                    ContactInfo = data.ContactInfo,
                    Email = data.Email,
                    Address = data.Address,
                    PhoneNumber = data.PhoneNumber
                }),
                "Error creating vendor"
            );

        /// <summary> Update a vendor. </summary>
        [HttpPut("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> UpdateVendor(int id, [FromBody] UpdateVendorsDto data)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new UpdateVendorsCommand
                {
                    Id = id,
                    Name = data.Name,
                    ContactInfo = data.ContactInfo,
                    Email = data.Email,
                    Address = data.Address,
                    PhoneNumber = data.PhoneNumber
                }),
                $"Error updating vendor with ID {id}"
            );

        /// <summary> Delete a vendor by ID. </summary>
        [HttpDelete("vendors/{id}")]
        public async Task<ActionResult<ResponseDto<bool>>> DeleteVendor(int id)
            => await HandleRequestAsync(
                async () => await _mediator.Send(new DeleteVendorsCommand { Id = id }),
                $"Error deleting vendor with ID {id}"
            );
    }
}
