using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebApi.Core.Models;

namespace WebApi.Core.Helper
{
    public static class LinqHelper
    {
        public static Expression<Func<T, TKey>> OrderExpression<T, TKey>(string memberName)
        {
            ParameterExpression[] typeParams = { Expression.Parameter(typeof(T), "") };

            var orderByExpression = (Expression<Func<T, TKey>>)Expression.Lambda(
                    Expression.Property(typeParams[0], memberName), typeParams);
            return orderByExpression;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> input, string memberName, string sord)
        {
            var pro = typeof(T).GetProperty(memberName);
            var type = pro.PropertyType;

            if (type == typeof(DateTime))
            {
                var expression = OrderExpression<T, DateTime>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(DateTime?))
            {
                var expression = OrderExpression<T, DateTime?>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(int))
            {
                var expression = OrderExpression<T, int>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(int?))
            {
                var expression = OrderExpression<T, int?>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(double))
            {
                var expression = OrderExpression<T, double>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(double?))
            {
                var expression = OrderExpression<T, double?>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(string))
            {
                var expression = OrderExpression<T, string>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            if (type == typeof(bool))
            {
                var expression = OrderExpression<T, bool>(memberName);
                return "ASC".Equals(sord, StringComparison.OrdinalIgnoreCase) ? input.OrderBy(expression) : input.OrderByDescending(expression);
            }
            return input;
        }

        public static IQueryable<T> SkipTakeAndSort<T>(this IQueryable<T> input, KendoGridPagingModel pagingModel,
            Expression<Func<T, string>> defaultSortField, bool isAscending = true)
        {
            if (!string.IsNullOrEmpty(pagingModel.SortField) && !string.IsNullOrEmpty(pagingModel.SortType))
            {
                input =
                    input.OrderBy(pagingModel.SortField, pagingModel.SortType)
                        .Skip(pagingModel.Skip)
                        .Take(pagingModel.Take);
            }
            else
            {
                if (isAscending)
                    input = input.OrderBy(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
                else
                    input = input.OrderByDescending(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
            }

            return input;
        }

        public static IQueryable<T> SkipTakeAndSort<T>(this IQueryable<T> input, KendoGridPagingModel pagingModel,
            Expression<Func<T, DateTime>> defaultSortField, bool isAscending = true)
        {
            if (!string.IsNullOrEmpty(pagingModel.SortField) && !string.IsNullOrEmpty(pagingModel.SortType))
            {
                input =
                    input.OrderBy(pagingModel.SortField, pagingModel.SortType)
                        .Skip(pagingModel.Skip)
                        .Take(pagingModel.Take);
            }
            else
            {
                if (isAscending)
                    input = input.OrderBy(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
                else
                    input = input.OrderByDescending(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
            }

            return input;
        }

        public static IQueryable<T> SkipTakeAndSort<T>(this IQueryable<T> input, KendoGridPagingModel pagingModel,
            Expression<Func<T, int>> defaultSortField, bool isAscending = true)
        {
            if (!string.IsNullOrEmpty(pagingModel.SortField) && !string.IsNullOrEmpty(pagingModel.SortType))
            {
                input =
                    input.OrderBy(pagingModel.SortField, pagingModel.SortType)
                        .Skip(pagingModel.Skip)
                        .Take(pagingModel.Take);
            }
            else
            {
                if (isAscending)
                    input = input.OrderBy(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
                else
                    input = input.OrderByDescending(defaultSortField).Skip(pagingModel.Skip).Take(pagingModel.Take);
            }

            return input;
        }
    }
}
