using DotnetCleanArch.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace DotnetCleanArch.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
