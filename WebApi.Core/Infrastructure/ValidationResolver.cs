using FluentValidation;
using System;
using System.Linq;
using WebApi.Core.Models;
using AutoMapper;

namespace WebApi.Core.Infrastructure
{
    public class ValidationResolver<TSource, TDestination> : IValueResolver<TSource, ModelContainer<TDestination>, TDestination>
            where TDestination : IValidationModel
    {
        private readonly IValidatorFactory _validatorFactory;

        public ValidationResolver(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public TDestination Resolve(TSource source, ModelContainer<TDestination> destination, TDestination destMember, ResolutionContext context)
        {
            var validator = _validatorFactory.GetValidator<TSource>();

            destMember = Mapper.Map<TDestination>(source);

            if (validator != null)
            {
                var validationResult = validator.Validate(source);

                if (!validationResult.IsValid)
                {
                    destMember.IsValid = false;
                    destMember.Messages = validationResult.Errors.Select(s => s.ErrorMessage).Distinct().ToList();
                }
            }

            return destMember;
        }
    }
}
