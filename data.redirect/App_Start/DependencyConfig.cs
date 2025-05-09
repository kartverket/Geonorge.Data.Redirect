using System.Web.Mvc;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Integration.Mvc;
using System.Reflection;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Configuration;
using Geonorge.AuthLib.NetFull;
using data.redirect;

namespace data.redirect.App_Start
{
    public class DependencyConfig
    {
        public static IContainer Configure(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            builder.RegisterModule(new AutofacWebTypesModule());
            ConfigureAppDependencies(builder);
            var container = builder.Build();

            // dependency resolver for MVC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }

        // the order of component registration is significant. must wire up dependencies in other packages before types in this project.
        private static void ConfigureAppDependencies(ContainerBuilder builder)
        {
            builder.RegisterModule<GeonorgeAuthenticationModule>();
        }
    }
}