using DotnetCleanArch.Domain.Common;
using Mediator;

namespace DotnetCleanArch.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
