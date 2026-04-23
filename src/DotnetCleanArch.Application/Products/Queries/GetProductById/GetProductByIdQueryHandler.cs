using DotnetCleanArch.Application.Abstractions.Data;
using DotnetCleanArch.Application.Abstractions.Messaging;
using DotnetCleanArch.Application.Products.Mappings;
using DotnetCleanArch.Domain.Common;
using DotnetCleanArch.Domain.Products;
using DotnetCleanArch.Domain.Products.Errors;
using Microsoft.EntityFrameworkCore;

namespace DotnetCleanArch.Application.Products.Queries.GetProductById;

internal sealed class GetProductByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    public async ValueTask<Result<GetProductByIdResponse>> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(query.ProductId);
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
        {
            return ProductErrors.NotFound;
        }

        return product.ToResponse();
    }
}
