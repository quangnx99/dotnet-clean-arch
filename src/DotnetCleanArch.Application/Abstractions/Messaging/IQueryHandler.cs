using DotnetCleanArch.Domain.Common;
using Mediator;

namespace DotnetCleanArch.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
