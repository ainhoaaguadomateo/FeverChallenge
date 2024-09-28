using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Models;
using Fever.Infraestructure.Postgres;
using Fever.Application.Interfaces;
using Fever.Infraestructure.Postgres.Repositories;

public class EventsRepositoryIntegrationTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly EventsRepository _repository;
    private readonly Mock<IDapperService> _dapperServiceMock;
    private readonly Mock<ILogger<EventsRepository>> _loggerMock;

    public EventsRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _dbContext = new ApplicationDbContext(options);

        _dapperServiceMock = new Mock<IDapperService>();
        _loggerMock = new Mock<ILogger<EventsRepository>>();

        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection.Setup(x => x.Value).Returns("Host=localhost;Database=test;Username=test;Password=test");

        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(c => c.GetSection("ConnectionStrings")).Returns(mockConfigurationSection.Object);
        configurationMock.Setup(c => c.GetSection("ConnectionStrings:PostgreSQL")).Returns(mockConfigurationSection.Object);

        _repository = new EventsRepository(_dbContext, _dapperServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SaveUpdatedEvents_AddsNewBaseEvent_AndCreatesResumedEvents()
    {
        var eventId = Guid.NewGuid();
        var baseEvents = new List<BaseEvent>
        {
            new BaseEvent
            {
                BaseEventId = 1,
                Title = "New Event",
                SellMode = "online",
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        EventId = 1,
                        EventStartDate = DateTime.UtcNow,
                        EventEndDate = DateTime.UtcNow.AddHours(2),
                        Zones = new List<Zone>
                        {
                            new Zone { Name = "Zone 1", ZoneId = 1, Capacity = 100, Price = 50 }
                        }
                    }
                }
            }
        };

        await _repository.SaveUpdatedEvents(baseEvents, CancellationToken.None);

        var savedBaseEvent = await _dbContext.BaseEvents.Include(be => be.Events).ThenInclude(e => e.Zones).FirstOrDefaultAsync(be => be.BaseEventId == 1);
        Assert.NotNull(savedBaseEvent);
        Assert.Single(savedBaseEvent.Events);
        Assert.Single(savedBaseEvent.Events[0].Zones);

        var resumedEvent = await _dbContext.ResumedEvents.FirstOrDefaultAsync(re => re.EventId == eventId);
        Assert.NotNull(resumedEvent);
        Assert.Equal("New Event", resumedEvent.Title);
    }

    [Fact]
    public async Task GetEventsAsync_ReturnsCorrectEvents()
    {
        var eventList = new List<GetEventListDTO>
        {
            new GetEventListDTO
            {
                Id = Guid.NewGuid(),
                Title = "Test Event",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                MinPrice = 10,
                MaxPrice = 50,
            }
        };

        _dapperServiceMock
            .Setup(d => d.QueryAsync<GetEventListDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(eventList);

        var result = await _repository.GetEventsAsync(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Events);
        Assert.Equal("Test Event", result.Events[0].Title);
    }

    [Fact]
    public async Task SaveUpdatedEvents_UpdatesExistingBaseEvent()
    {
        var eventId = Guid.NewGuid();
        var existingBaseEvent = new BaseEvent
        {
            BaseEventId = 1,
            Title = "Existing Event",
            SellMode = "offline",
            Events = new List<Event>
            {
                new Event
                {
                    Id = eventId,
                    EventId = 1,
                    EventStartDate = DateTime.UtcNow,
                    EventEndDate = DateTime.UtcNow.AddHours(2),
                    Zones = new List<Zone>
                    {
                        new Zone { Name = "Zone 1", ZoneId = 1, Capacity = 100, Price = 50 }
                    }
                }
            }
        };
        _dbContext.BaseEvents.Add(existingBaseEvent);
        await _dbContext.SaveChangesAsync();

        var updatedBaseEvents = new List<BaseEvent>
        {
            new BaseEvent
            {
                BaseEventId = 1,
                Title = "Updated Event",
                SellMode = "online",
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        BaseEventId = 1,
                        EventId = 1,
                        EventStartDate = DateTime.UtcNow,
                        EventEndDate = DateTime.UtcNow.AddHours(3),
                        Zones = new List<Zone>
                        {
                            new Zone { Name = "New Zone 1", ZoneId = 1, Capacity = 200, Price = 75 }
                        }
                    }
                }
            }
        };

        await _repository.SaveUpdatedEvents(updatedBaseEvents, CancellationToken.None);

        var updatedBaseEvent = await _dbContext.BaseEvents.Include(be => be.Events).ThenInclude(e => e.Zones).FirstOrDefaultAsync(be => be.BaseEventId == 1);
        Assert.NotNull(updatedBaseEvent);
        Assert.Equal("Updated Event", updatedBaseEvent.Title);
        Assert.Equal("online", updatedBaseEvent.SellMode);
        Assert.Equal(200, updatedBaseEvent.Events.First().Zones.First().Capacity);
    }
}