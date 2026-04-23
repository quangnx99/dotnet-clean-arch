using DotnetCleanArch.Application.Abstractions.Messaging;
using DotnetCleanArch.Domain.Products;

namespace DotnetCleanArch.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(string Name, string Description, decimal Price)
    : ICommand<ProductId>;
