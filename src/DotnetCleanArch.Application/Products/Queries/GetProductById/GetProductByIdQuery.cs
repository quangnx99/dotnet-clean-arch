using DotnetCleanArch.Application.Abstractions.Messaging;
using DotnetCleanArch.Application.Products.Queries.GetProductById;

namespace DotnetCleanArch.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<GetProductByIdResponse>;
