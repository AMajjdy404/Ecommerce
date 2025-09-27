using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProducrByIdQuery:IRequest<ProductResponseDto>
    {
        public string Id { get; set; }
        public GetProducrByIdQuery(string id)
        {
            Id = id;
        }
    }
}
