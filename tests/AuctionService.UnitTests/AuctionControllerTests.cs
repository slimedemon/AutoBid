using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        _controller = new AuctionController(_auctionRepo.Object, _mapper, _publishEndpoint.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = Utils.Helpers.GetClaimsPrincipal()
                }
            }
        };
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

    [Fact]
    public async Task GetAuctionById_WithValidId_ReturnsAuction()
    {
        // Arrange
        var auction = _fixture.Create<AuctionDto>();
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(auction.Id)).ReturnsAsync(auction);

        // Act
        var result = await _controller.GetAuctionById(auction.Id);

        // Assert
        var okResult = Assert.IsType<ActionResult<AuctionDto>>(result);
        var returnValue = Assert.IsType<AuctionDto>(okResult.Value);
        Assert.Equal(auction.Make, returnValue.Make);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _auctionRepo.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // Act
        var result = await _controller.GetAuctionById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAction()
    {
        // Arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateAuction(auction);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdAtActionResult.StatusCode);
        var returnValue = Assert.IsType<AuctionDto>(createdAtActionResult.Value);
        Assert.Equal(auction.Make, returnValue.Make);
    }

    [Fact]
    public async Task CreateAuction_WithFailedSave_ReturnsBadRequest()
    {
        // Arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _auctionRepo.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateAuction(auction);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithValidInput_ReturnsAuction()
    {
        // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = _controller.User.Identity.Name;

        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAuction(auction.Id, updateAuctionDto);

        // Assert
        var okResult = Assert.IsType<ActionResult<AuctionDto>>(result);
        var returnValue = Assert.IsType<AuctionDto>(okResult.Value);
        Assert.Equal(updateAuctionDto.Make, returnValue.Make);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_ReturnsForbid()
    {
        // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = "Another user !=" + _controller.User.Identity.Name;

        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAuction(auction.Id, updateAuctionDto);

        // Assert
        var forbidResult = Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // Act
        var result = await _controller.UpdateAuction(Guid.NewGuid(), updateAuctionDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateAuction_WithFailedSave_ReturnsBadRequest()
    {
         // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = _controller.User.Identity.Name;

        var updateAuctionDto = _fixture.Create<UpdateAuctionDto>();

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateAuction(auction.Id, updateAuctionDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAuction_WithValidInput_ReturnsOk()
    {
        // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = _controller.User.Identity.Name;

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAuction(auction.Id);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidUser_ReturnForbid()
    {
        // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = "Another user !=" + _controller.User.Identity.Name;

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAuction(auction.Id);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(value: null);

        // Act
        var result = await _controller.DeleteAuction(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteAuction_WithFailedSave_ReturnsBadRequest()
    {
        // Arrange
        var auction = _fixture.Build<Auction>()
            .Without(a => a.Item)
            .Create();
        var Item = _fixture.Build<Item>()
            .With(i => i.AuctionId, auction.Id)
            .Without(i => i.Auction)
            .Create();

        auction.Item = Item;
        auction.Seller = _controller.User.Identity.Name;

        _auctionRepo.Setup(repo => repo.GetAuctionEntityByIdAsync(auction.Id))
            .ReturnsAsync(auction);
        _auctionRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAuction(auction.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
