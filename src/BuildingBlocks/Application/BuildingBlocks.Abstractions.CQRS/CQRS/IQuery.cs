using MediatR;

namespace BuildingBlocks.Abstractions.CQRS.CQRS;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}
