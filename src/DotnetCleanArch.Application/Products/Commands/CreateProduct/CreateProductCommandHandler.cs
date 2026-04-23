using DotnetCleanArch.Application.Abstractions.Data;
using DotnetCleanArch.Application.Abstractions.Messaging;
using DotnetCleanArch.Domain.Common;
using DotnetCleanArch.Domain.Products;

namespace DotnetCleanArch.Application.Products.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateProductCommand, ProductId>
{
    public async ValueTask<Result<ProductId>> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = Product.Create(command.Name, command.Description, command.Price);

        if (result.IsFailure)
        {
            return Result<ProductId>.Failure(result.Error);
        }

        var product = result.Value;
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ProductId>.Success(product.Id);
    }
}
