using Autofac;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Core.Infrastructure
{
    public class AutofacValidatorFactory : ValidatorFactoryBase
    {
        private readonly ILifetimeScope _scope;

        public AutofacValidatorFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            object validator;
            _scope.TryResolve(validatorType, out validator);
            return validator as IValidator;
        }
    }
}
