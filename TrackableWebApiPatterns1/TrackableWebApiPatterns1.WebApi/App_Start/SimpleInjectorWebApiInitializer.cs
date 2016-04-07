using System.Web.Http;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using TrackableWebApiPatterns1.WebApi;
//using TrackableWebApiPatterns1.Service.EF.Contexts;
//using TrackableWebApiPatterns1.Service.EF.Repositories;
//using TrackableWebApiPatterns1.Service.EF.UnitsOfWork;
//using TrackableWebApiPatterns1.Service.Persistence.Repositories;
//using TrackableWebApiPatterns1.Service.Persistence.UnitsOfWork;

[assembly: WebActivator.PostApplicationStartMethod(typeof(SimpleInjectorWebApiInitializer), "Initialize")]

namespace TrackableWebApiPatterns1.WebApi
{
    public static class SimpleInjectorWebApiInitializer
    {
        public static void Initialize()
        {
            // Create IoC container
            var container = new Container();

            // Register dependencies
            InitializeContainer(container);
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            // Verify registrations
            container.Verify();

            // Set Web API dependency resolver
            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void InitializeContainer(Container container)
        {
            // TODO: Register context, unit of work and repos with per request lifetime
            //container.RegisterWebApiRequest<INorthwindSlimContext, NorthwindSlimContext>();
            //container.RegisterWebApiRequest<INorthwindUnitOfWork, NorthwindUnitOfWork>();
            //container.RegisterWebApiRequest<ICustomerRepository, CustomerRepository>();
            //container.RegisterWebApiRequest<IOrderRepository, OrderRepository>();
        }
    }
}