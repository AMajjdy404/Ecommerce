using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductsByNameQuery:IRequest<IList<ProductResponseDto>>
    {
        public string Name { get; set; }
        public GetProductsByNameQuery(string name)
        {
            Name = name;
        }
    }
}
