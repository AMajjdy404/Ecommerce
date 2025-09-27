using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllTypesQueryHandler : IRequestHandler<GetAllTypesQuery, IList<TypeResponseDto>>
    {
        private readonly ITypeRepository _typeRepository;
        private readonly IMapper _mapper;

        public GetAllTypesQueryHandler(ITypeRepository typeRepository,
            IMapper mapper)
        {
            _typeRepository = typeRepository;
            _mapper = mapper;
        }
        public async Task<IList<TypeResponseDto>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            var typesList = await _typeRepository.GetAllTypes();
            var mappedTypes = _mapper.Map<IList<ProductType>,IList<TypeResponseDto>>(typesList.ToList());
            return mappedTypes;
        }
    }
}
