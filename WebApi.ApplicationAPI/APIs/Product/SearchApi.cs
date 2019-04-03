using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common;
using WebApi.Core.Helper;
using WebApi.Core.Models;
using WebApi.Core.UnitOfWork;
using Z.EntityFramework.Plus;

namespace WebApi.ApplicationAPI.APIs.Product
{
    public class SearchApi
    {
        public class Query : KendoGridPagingModel, IRequest<Result>
        {
            public string Name { get; set; }
        }

        public class Result : ISearchResult<NestedModel.ProductModel>, IWebApiResponse
        {
            public IList<NestedModel.ProductModel> SearchResultItems { get; set; }
            public int Count { get; set; }
            public Result()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        public class NestedModel
        {
            public class ProductModel : DataAccess.Model.EntityBase
            {
                public string Name { get; set; }
                public string Description { get; set; }
                public string ShortDescription { get; set; }
                public double? Duration { get; set; }
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DataAccess.Model.Product, NestedModel.ProductModel>()
                    ;
            }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IUnitOfWorkFactory<UnitOfWork> _unitOfWork;

            public QueryHandler(IUnitOfWorkFactory<UnitOfWork> unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Result Handle(Query message)
            {
                using (var unit = _unitOfWork.Create())
                {
                    var query =
                        unit.Repository<DataAccess.Model.Product>()
                            .Where(w => w.StatusId == Constant.StatusId.Active);

                    if (!string.IsNullOrEmpty(message.Name))
                    {
                        query = query.Where(f => f.Name.Contains(message.Name));
                    }

                    var result = query.ProjectTo<NestedModel.ProductModel>();

                    var futureCount = result.DeferredCount().FutureValue();
                    var futureItems = result.SkipTakeAndSort(message, f => f.CreatedDate.Value, false).Future();

                    return new Result
                    {
                        Count = futureCount.Value,
                        SearchResultItems = futureItems.ToList()
                    };
                }
            }
        }
    }
}
