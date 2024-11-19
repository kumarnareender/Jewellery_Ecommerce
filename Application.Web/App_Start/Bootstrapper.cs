using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using System.Reflection;
using Application.Data.Repository;
using Application.Data.Infrastructure;
using Application.Service;
using Application.Common.Authentication;
using Application.Web.Mappings;

namespace Application.Web
{
    public static class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
            AutoMapperConfiguration.Configure();
        }
        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerHttpRequest();
            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerHttpRequest();

            builder.RegisterAssemblyTypes(typeof(UserRepository).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces().InstancePerHttpRequest();

            builder.RegisterAssemblyTypes(typeof(UserService).Assembly)
           .Where(t => t.Name.EndsWith("Service"))
           .AsImplementedInterfaces().InstancePerHttpRequest();

            builder.RegisterAssemblyTypes(typeof(DefaultFormsAuthentication).Assembly)
             .Where(t => t.Name.EndsWith("Authentication"))
             .AsImplementedInterfaces().InstancePerHttpRequest();

            builder.RegisterFilterProvider();
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));            
        }
    }
}