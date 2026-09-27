using Application.Handlers.ClientHandlers;
using Application.Handlers.CommonHandlers;
using Domain.Entites;
using Domain.Interfaces;
using InvoiceHub.Application.Handlers.CommonHandlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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

        services.RegisterClosedCommonHandlers();

        return services;
    }

    private static void RegisterClosedCommonHandlers(this IServiceCollection services)
    {
        var entityTypes = typeof(BaseDomainEntity).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(BaseDomainEntity).IsAssignableFrom(t));

        foreach (var entityType in entityTypes)
        {
            services.RegisterHandler(
                typeof(GetAll<>),
                typeof(IEnumerable<>),
                typeof(GetAllHandler<>),
                entityType);

            services.RegisterHandler(
                typeof(GetTEntityByIdRequest<>),
                typeof(ResponseDto<>),
                typeof(GetTEntityByIdHandler<>),
                entityType);

            services.RegisterHandler(
                typeof(CreateRequest<>),
                typeof(ResponseDto<>),
                typeof(CreateHandler<>),
                entityType);
        }
    }

    private static void RegisterHandler(
        this IServiceCollection services,
        Type openRequestType,
        Type openResponseType,
        Type openHandlerType,
        Type entityType)
    {
        var requestType = openRequestType.MakeGenericType(entityType);
        var responseType = openResponseType.MakeGenericType(entityType);
        var handlerType = openHandlerType.MakeGenericType(entityType);
        var serviceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        services.AddTransient(serviceType, handlerType);
    }
}
