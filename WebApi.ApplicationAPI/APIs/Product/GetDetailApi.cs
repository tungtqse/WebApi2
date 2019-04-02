﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common;
using WebApi.Core.UnitOfWork;

namespace WebApi.ApplicationAPI.APIs.Product
{
    public class GetDetailApi
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ShortDescription { get; set; }
            public double? Duration { get; set; }
        }       

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DataAccess.Model.Product, Result>()
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
                var resultQuery = new Result();

                using (var unit = _unitOfWork.Create())
                {
                    var query =
                        unit.Repository<DataAccess.Model.Product>()
                            .Where(w => w.Id == message.Id).ProjectTo<Result>();

                    resultQuery = query.FirstOrDefault();

                    return resultQuery;
                }
            }
        }
    }
}
