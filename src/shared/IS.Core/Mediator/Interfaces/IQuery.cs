namespace IS.Core.Mediator.Interfaces
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}
