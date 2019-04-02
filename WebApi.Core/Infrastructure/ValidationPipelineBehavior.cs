using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Core.Infrastructure.MediatedControllers;

namespace WebApi.Core.Infrastructure
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidationPipelineBehavior(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next)
        {
            var result = await next();

            var validator = _validatorFactory.GetValidator(result.GetType());

            if (validator == null) return result;

            if (result.GetType().GetInterfaces().Contains(typeof(ICommandResponse)))
            {
                ((ICommandResponse)result).ValidationResult = validator.Validate(result);
            }

            return result;
        }
    }
}
