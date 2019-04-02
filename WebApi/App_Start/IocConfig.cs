using Autofac;
using Autofac.Integration.WebApi;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using WebApi.Common;
using WebApi.Core.UnitOfWork;
using WebApi.Core.Repository;
using WebApi.DataAccess;
using System.Web.Http;
using FluentValidation;
using WebApi.Core.Infrastructure;

namespace WebApi.App_Start
{
    public class IocConfig
    {
        public static void Config()
        {
            var builder = new ContainerBuilder();

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

            builder.RegisterModule(new ApplicationAPI.AutofacModule());            

            builder.Register<Func<IPrincipal>>(c => () => HttpContext.Current.User);

            builder.RegisterType(typeof(UnitOfWorkFactory<UnitOfWork, MainContext>)).As(
                typeof(IUnitOfWorkFactory<UnitOfWork>))
                .SingleInstance().WithParameter("connectionStringName", Constant.ConnectionString);

            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>();
            builder.RegisterGeneric(typeof(ValidationPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidationResolver<,>)).AsSelf();

            #region WebApi

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            #endregion

            var container = builder.Build();

            //AutoMapper Config
            AutoMapperInitializer.Initialize(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}