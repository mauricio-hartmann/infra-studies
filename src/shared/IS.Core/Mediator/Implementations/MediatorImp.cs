using IS.Core.Mediator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace IS.Core.Mediator.Implementations
{
    public class MediatorImp : IMediator
    {
        private static readonly MethodInfo SendInternalAsyncMethod = typeof(MediatorImp)
            .GetMethod(nameof(SendInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly IServiceProvider _serviceProvider;

        public MediatorImp(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            Type requestType = request.GetType();
            Type responseType = typeof(TResponse);
            MethodInfo method = SendInternalAsyncMethod.MakeGenericMethod(requestType, responseType);

            try
            {
                return (Task<TResponse>)method.Invoke(this, [request, cancellationToken])!;
            }
            catch (TargetInvocationException exception) when (exception.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
                throw;
            }
        }

        private Task<TResponse> SendInternalAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest<TResponse>
        {
            var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            return handler.HandleAsync(request, cancellationToken);
        }
    }
}
