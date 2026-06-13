using IS.Core.Mediator.Interfaces;
using IS.Core.Mediator.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace IS.Core.Mediator.Configuration
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(services);

            if (assemblies is null || assemblies.Length == 0)
                throw new ArgumentException("Informe ao menos um assembly para o Mediator.", nameof(assemblies));

            Assembly[] distinctAssemblies = assemblies
                .Where(assembly => assembly is not null)
                .Distinct()
                .ToArray();

            services.TryAddScoped<IMediator, MediatorImp>();

            List<HandlerRegistration> handlerRegistrations = distinctAssemblies
                .SelectMany(GetLoadableTypes)
                .Where(type => type is { IsClass: true, IsAbstract: false })
                .Where(type => type.ContainsGenericParameters is false)
                .SelectMany(GetHandlerRegistrations)
                .ToList();

            ValidateDuplicatedHandlers(handlerRegistrations);

            foreach (HandlerRegistration registration in handlerRegistrations)
                services.AddScoped(registration.ServiceType, registration.ImplementationType);

            ValidateRequestsHaveHandlers(distinctAssemblies, handlerRegistrations);

            return services;
        }

        private static IEnumerable<HandlerRegistration> GetHandlerRegistrations(
            Type implementationType)
        {
            Type handlerInterfaceType = typeof(IRequestHandler<,>);

            return implementationType
                .GetInterfaces()
                .Where(type => type.IsGenericType)
                .Where(type => type.GetGenericTypeDefinition() == handlerInterfaceType)
                .Select(serviceType => new HandlerRegistration(serviceType, implementationType));
        }

        private static void ValidateDuplicatedHandlers(List<HandlerRegistration> handlerRegistrations)
        {
            var duplicatedHandlers = handlerRegistrations
                .GroupBy(registration => registration.ServiceType)
                .Where(group => group.Count() > 1)
                .ToList();

            if (duplicatedHandlers.Count == 0)
                return;

            string message = string.Join(
                Environment.NewLine,
                duplicatedHandlers.Select(group =>
                {
                    string implementations = string.Join(", ",group.Select(x => x.ImplementationType.FullName));

                    return $"{group.Key.FullName} possui múltiplos handlers: {implementations}";
                }));

            throw new InvalidOperationException($"Foram encontrados handlers duplicados:{Environment.NewLine}{message}");
        }

        private static void ValidateRequestsHaveHandlers(Assembly[] assemblies, List<HandlerRegistration> handlerRegistrations)
        {
            HashSet<Type> registeredHandlerServiceTypes = handlerRegistrations
                .Select(x => x.ServiceType)
                .ToHashSet();

            List<string> requestsWithoutHandler = assemblies
                .SelectMany(GetLoadableTypes)
                .Where(type => type is { IsClass: true, IsAbstract: false })
                .Where(type => type.ContainsGenericParameters is false)
                .SelectMany(GetExpectedHandlerTypes)
                .Where(expectedHandlerType => registeredHandlerServiceTypes.Contains(expectedHandlerType) is false)
                .Select(type => type.FullName ?? type.Name)
                .Distinct()
                .ToList();

            if (requestsWithoutHandler.Count == 0)
                return;

            string message = string.Join(Environment.NewLine, requestsWithoutHandler);

            throw new InvalidOperationException($"Existem commands/queries sem handler registrado:{Environment.NewLine}{message}");
        }

        private static IEnumerable<Type> GetExpectedHandlerTypes(Type requestType)
        {
            Type requestInterfaceType = typeof(IRequest<>);

            return requestType
                .GetInterfaces()
                .Where(type => type.IsGenericType)
                .Where(type => type.GetGenericTypeDefinition() == requestInterfaceType)
                .Select(requestInterface =>
                {
                    Type responseType = requestInterface.GetGenericArguments()[0];

                    return typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                });
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type is not null)!;
            }
        }

        private sealed record HandlerRegistration(Type ServiceType, Type ImplementationType);
    }
}
