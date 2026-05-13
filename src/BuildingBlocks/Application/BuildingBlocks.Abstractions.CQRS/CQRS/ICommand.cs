using MediatR;

namespace BuildingBlocks.Abstractions.CQRS.CQRS;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out T> : IRequest<T>
    where T : notnull
{
}
