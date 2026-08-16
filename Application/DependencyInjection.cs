using Microsoft.Extensions.DependencyInjection;
using Domain.Interfaces;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
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