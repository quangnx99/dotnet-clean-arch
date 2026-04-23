using DotnetCleanArch.Application.Abstractions.Data;
using DotnetCleanArch.Application.Products.Commands.CreateProduct;
using DotnetCleanArch.Domain.Products;
using FluentAssertions;
using NSubstitute;

namespace DotnetCleanArch.UnitTests.Application;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnProductId()
    {
        await using var context = TestDbContextFactory.Create();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        var handler = new CreateProductCommandHandler(context, unitOfWork);
        var command = new CreateProductCommand("Widget", "A widget", 9.99m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().NotBeEmpty();
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidPrice_ShouldReturnFailure()
    {
        await using var context = TestDbContextFactory.Create();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new CreateProductCommandHandler(context, unitOfWork);
        var command = new CreateProductCommand("Widget", "A widget", -1m);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
