using Autofac;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.App_Start
{
    public class AutoMapperInitializer
    {
        public static void Initialize(IContainer container)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.ConstructServicesUsing(container.Resolve);
                cfg.AddProfiles(typeof(AutoMapperInitializer));
                cfg.AddProfiles(typeof(ApplicationAPI.AutoMapperInitializer));
            });
        }
    }
}