using Autofac;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.ApplicationAPI
{
    public class AutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region IMediator
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t =>
                {
                    object o;
                    return c.TryResolve(t, out o) ? o : null;
                };
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });
            #endregion

            var mediatrOpenTypes = new[]
            {
                typeof(INotificationHandler<>),
                typeof(IAsyncNotificationHandler<>),
                typeof(IRequestHandler<,>),
                typeof(IRequestHandler<>),
                typeof(IAsyncRequestHandler<,>),
                typeof(IAsyncRequestHandler<>),
                typeof(IValidator<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes)
            {
                builder
                    .RegisterAssemblyTypes(typeof(AutofacModule).Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            var mappingActions = typeof(AutofacModule).Assembly.GetTypes()
                .Where(c => !c.IsInterface &&
                            c.GetInterfaces().Any(x => x.IsGenericType &&
                                                       (x.GetGenericTypeDefinition() == typeof(IValueResolver<,,>) ||
                                                        x.GetGenericTypeDefinition() == typeof(IMappingAction<,>))))
                .ToList();

            foreach (var t in mappingActions)
            {
                builder.RegisterType(t).AsSelf();
            }
        }
    }
}
