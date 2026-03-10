using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _auctionRepo;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly AuctionController _controller;
    private readonly IMapper _mapper;

    public AuctionControllerTests()
    {
        _auctionRepo = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _fixture = new Fixture();

        var mockMapperProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(AuctionController).Assembly);
        }).CreateMapper().ConfigurationProvider;

        _mapper = new Mapper(mockMapperProvider);
        _controller = new AuctionController(_auctionRepo.Object, _mapper, _publishEndpoint.Object); 
    }

    [Fact]
    public async Task GetAllAuctions_WithNoParams_Returns10Auctions()
    {
        // Arrange
        var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
        _auctionRepo.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        // Act
        var result = await _controller.GetAllAuctions(null);

        // Assert
        var okResult = Assert.IsType<ActionResult<List<AuctionDto>>>(result);
        var returnValue = Assert.IsType<List<AuctionDto>>(okResult.Value);
        Assert.Equal(10, returnValue.Count);
    }
}
