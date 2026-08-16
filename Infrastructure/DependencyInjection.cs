namespace Infrastructure;

using Application.Interfaces.Queries;
using Domain.Interfaces;
using Infrastructure.Queries;
using Infrastructure.Queries.QueryBuilderEngine;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
public static class DependencyInjection
{
public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICommonQueries<>), typeof(CommonQueries<>));
        services.AddScoped(typeof(ICommonCommands<>), typeof(CommonCommands<>));
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableTo<IScopedService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<IRegisterAsSelf>())
                .AsSelf()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );

        return services;
    }
}
