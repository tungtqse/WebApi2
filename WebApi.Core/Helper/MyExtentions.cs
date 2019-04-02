using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Common;
using WebApi.DataAccess.Model;

namespace WebApi.Core.Helper
{
    public static class MyExtentions
    {
        public static IRuleBuilderOptions<T, TProperty> WithGlobalMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage)
        {
            foreach (var item in (rule as RuleBuilder<T, TProperty>).Rule.Validators)
                item.Options.ErrorMessageSource = new StaticStringSource(errorMessage);

            return rule;
        }

        public static IQueryable<T> WithActive<T>(this IQueryable<T> query) where T : EntityBase
        {
            return query.Where(w => w.StatusId == Constant.StatusId.Active);
        }

        public static IQueryable<T> FindById<T>(this IQueryable<T> query, Guid id) where T : EntityBase
        {
            return query.Where(w => w.Id == id);
        }

        public static bool IsNullOrEmpty(this Guid? value)
        {
            return value == null || value == Guid.Empty;
        }
    }
}
