namespace IS.Core.Mediator.Interfaces
{
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
