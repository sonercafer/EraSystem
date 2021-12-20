using Era.Core.Event;
using Era.Data;
using Era.Data.Base;
using Era.Data.Entities;
using Era.Service; 
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; 
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Era.Web.Infrastructure.Startup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddController(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddControllersWithViews();
            return serviceCollection;
        }
        public static IServiceCollection AddAutoMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper((typeof(Service.IService)));
            return serviceCollection;
        } 
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan
                .FromAssemblyOf<IRepository>()
                .AddClasses(classes => classes.AssignableTo<IRepository>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            serviceCollection.AddDbContext<EraContext>(ServiceLifetime.Scoped);
            return serviceCollection;
        }

        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan
                .FromAssemblyOf<IService>()
                .AddClasses(classes => classes.AssignableTo<IService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return serviceCollection;
        }

        public static IServiceCollection AddConsumers(this IServiceCollection serviceCollection, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                serviceCollection.AddClassesAsImplementedInterface(assembly, typeof(IConsumer<>), ServiceLifetime.Transient);
            }

            return serviceCollection;
        }

        private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
        {
            var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                                                && !x.IsAbstract
                                                                && x != compareType
                                                                && x.GetInterfaces()
                                                                    .Any(i => i.IsGenericType
                                                                              && i.GetGenericTypeDefinition() == compareType))?.ToList();

            return typeInfoList;
        }

        private static void AddClassesAsImplementedInterface(this IServiceCollection services, Assembly assembly, Type compareType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            assembly.GetTypesAssignableTo(compareType).ForEach((type) =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                    }
                }
            });
        }

    }
}
