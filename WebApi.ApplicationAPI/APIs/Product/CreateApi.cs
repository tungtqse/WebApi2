using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Core.Helper;
using WebApi.Core.UnitOfWork;

namespace WebApi.ApplicationAPI.APIs.Product
{
    public class CreateApi
    {
        public class Command : IRequest
        {
            public NestedModel.ProductModel ProductModel { get; set; }
        }

        public class NestedModel
        {
            public class ProductModel
            {
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
                    //.ForMember(m => m.ModifiedBy, o => o.MapFrom(f => HttpContextManager.GetCurrentUserEmail()))
                    ;
            }
        }

        #endregion

        public class CommandValidator : AbstractValidator<NestedModel.ProductModel>
        {
            public CommandValidator()
            {
                RuleFor(f => f.Name).NotNull().NotEmpty().WithGlobalMessage("Name is required");
            }
        }

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command>
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

            public void Handle(Command message)
            {
                using (var unit = _unitOfWork.Create())
                {
                    // Validate
                    var isValid = false;
                    var itemValidator = _validatorFactory.GetValidator<NestedModel.ProductModel>();
                    var validationResult = itemValidator.Validate(message.ProductModel);

                    if (!validationResult.IsValid)
                    {
                        var remark = string.Join(";",
                            validationResult.Errors.Select(s => s.ErrorMessage).Distinct().ToList());
                    }
                    else
                    {
                        isValid = true;
                    }

                    if (isValid)
                    {
                        var product = Mapper.Map<DataAccess.Model.Product>(message.ProductModel);

                        unit.Repository<DataAccess.Model.Product>().Add(product);

                        unit.SaveChanges();
                    }
                }
            }
        }

        #endregion
    }
}
