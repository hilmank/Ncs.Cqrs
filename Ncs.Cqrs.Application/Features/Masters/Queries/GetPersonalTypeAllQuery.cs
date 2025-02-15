using System;
using AutoMapper;
using Ncs.Cqrs.Application.Common.DTOs;
using Ncs.Cqrs.Application.Interfaces;
using Ncs.Cqrs.Domain.Constants;
using MediatR;

namespace Ncs.Cqrs.Application.Features.Masters.Queries;

public class GetPersonalTypeAllQuery : IRequest<ResponseDto<IEnumerable<PersonalIdTypeDto>>>
{
}
public class GetPersonalTypeAllQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetPersonalTypeAllQuery, ResponseDto<IEnumerable<PersonalIdTypeDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<ResponseDto<IEnumerable<PersonalIdTypeDto>>> Handle(GetPersonalTypeAllQuery request, CancellationToken cancellationToken)
    {
        var personalIdTypes = await _unitOfWork.Masters.GetPersonalIdTypesAsync();

        if (personalIdTypes == null || !personalIdTypes.Any())
        {
            return ResponseDto<IEnumerable<PersonalIdTypeDto>>.ErrorResponse(ErrorCodes.NotFound, "Personalid Type not found");
        }

        var result = _mapper.Map<IEnumerable<PersonalIdTypeDto>>(personalIdTypes);

        return ResponseDto<IEnumerable<PersonalIdTypeDto>>.SuccessResponse(result, "Data retrieved successfully");
    }
}

