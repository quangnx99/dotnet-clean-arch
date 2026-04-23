using DotnetCleanArch.Application.Products.Queries.GetProductById;
using DotnetCleanArch.Domain.Products;

namespace DotnetCleanArch.Application.Products.Mappings;

public static class ProductMappings
{
    public static GetProductByIdResponse ToResponse(this Product product) =>
        new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.Price,
            product.CreatedAt,
            product.UpdatedAt);
}
