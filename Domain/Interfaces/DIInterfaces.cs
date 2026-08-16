namespace Domain.Interfaces;

/// <summary>
/// Marks a service to be registered with a Scoped lifetime.
/// A new instance is created for each HTTP request/scope.
/// </summary>
public interface IScopedService { }

/// <summary>
/// Marks a service to be registered with a Transient lifetime.
/// A new instance is created each time the service is requested.
/// </summary>
public interface ITransientService { }

/// <summary>
/// Marks a service to be registered with a Singleton lifetime.
/// A single instance is shared for the application's lifetime.
/// </summary>
public interface ISingletonService { }

/// <summary>
/// Marks a type to be registered using its concrete type.
/// Used when the type should be injected directly rather than through an interface.
/// </summary>
public interface IRegisterAsSelf { }