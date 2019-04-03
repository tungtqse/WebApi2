using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Core.Helper;
using WebApi.Core.Models;
using WebApi.Core.UnitOfWork;

namespace WebApi.ApplicationAPI.APIs.Product
{
    public class UpdateApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public NestedModel.ProductModel ProductModel { get; set; }
        }

        public class CommandResponse : IWebApiResponse
        {
            public CommandResponse()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        public class NestedModel
        {
            public class ProductModel
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
                public string ShortDescription { get; set; }
                public double? Duration { get; set; }
            }
        }

        #region Mapping

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<NestedModel.ProductModel, DataAccess.Model.Product>()
                    ;
            }
        }

        #endregion

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(f => f.ProductModel.Name).NotNull().NotEmpty().WithGlobalMessage("Name is required");
            }
        }

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command,CommandResponse>
        {
            private readonly IUnitOfWorkFactory<UnitOfWork> _unitOfWork;
            private readonly IMediator _mediator;
            private readonly IValidatorFactory _validatorFactory;

            public CommandHandler(IUnitOfWorkFactory<UnitOfWork> unitOfWork, IMediator mediator, IValidatorFactory validatorFactory)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
                _validatorFactory = validatorFactory;
            }

            public CommandResponse Handle(Command message)
            {
                var result = new CommandResponse();

                using (var unit = _unitOfWork.Create())
                {
                    // Validate
                    var isValid = true;
                    var itemValidator = _validatorFactory.GetValidator<NestedModel.ProductModel>();
                    var validationResult = itemValidator.Validate(message.ProductModel);

                    if (!validationResult.IsValid)
                    {
                        result.Code = 400;
                        result.Messages.AddRange(validationResult.Errors.Select(s => s.ErrorMessage).Distinct().ToList());
                        isValid = false;
                    }                   

                    if (isValid)
                    {
                        var product = unit.Repository<DataAccess.Model.Product>().FirstOrDefault(f => f.Id == message.ProductModel.Id);
                            
                        if(product != null)
                        {
                            product = Mapper.Map(message.ProductModel,product);

                            unit.SaveChanges();

                            result.Code = 201;
                            result.IsSuccessful = true;
                            result.Messages.Add("Product is updated");
                        }                        
                    }
                }

                return result;
            }
        }

        #endregion
    }
}
