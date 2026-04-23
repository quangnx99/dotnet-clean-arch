using DotnetCleanArch.Domain.Common;
using Mediator;

namespace DotnetCleanArch.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
