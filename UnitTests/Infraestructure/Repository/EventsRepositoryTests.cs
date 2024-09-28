using Fever.Application.Interfaces;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Models;
using Fever.Infraestructure.Postgres;
using Fever.Infraestructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Infraestructure.Repository;

public class EventsRepositoryTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ApplicationDbContext _mockDbContext;
    private readonly Mock<IDapperService> _mockDapperService;
    private readonly Mock<ILogger<EventsRepository>> _mockLogger;
    private readonly EventsRepository _repository;

    public EventsRepositoryTests()
    {
        _mockDapperService = new Mock<IDapperService>();
        _mockLogger = new Mock<ILogger<EventsRepository>>();

        var mockConfigurationSection = new Mock<IConfigurationSection>();
        mockConfigurationSection
            .Setup(a => a.Value)
            .Returns("Host=localhost;Database=test;Username=test;Password=test");

        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration
            .Setup(config => config.GetSection("ConnectionStrings"))
            .Returns(mockConfigurationSection.Object);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _mockDbContext = new ApplicationDbContext(options);

        _repository = new EventsRepository(
            _mockDbContext,
            _mockDapperService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task SaveUpdatedEvents_Adds_New_BaseEvent_If_Not_Exists()
    {
        var eventId = Guid.NewGuid();
        var baseEvents = new List<BaseEvent>
        {
            new BaseEvent
            {
                Title = "New Base Event",
                BaseEventId = 1,
                SellMode = "online",
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        EventId = 1,
                        BaseEventId = 1,
                        Zones = new List<Zone>
                        {
                            new Zone
                            {
                                Price = 10,
                                Capacity = 50,
                                Name = "Nombre",
                                EventId = eventId,
                            },
                        },
                    },
                },
            },
        };

        await _repository.SaveUpdatedEvents(baseEvents, CancellationToken.None);

        var addedEvent = _mockDbContext.BaseEvents.SingleOrDefault(be => be.BaseEventId == 1);
        Assert.NotNull(addedEvent);
        Assert.Equal("online", addedEvent.SellMode);
        Assert.Single(addedEvent.Events);
    }

    [Fact]
    public async Task SaveUpdatedEvents_Updates_Existing_BaseEvent()
    {
        var existingBaseEvent = new BaseEvent
        {
            Title = "New Base Event modified",
            BaseEventId = 5,
            SellMode = "offline",
            Events = new List<Event>
            {
                new Event { EventId = 33, BaseEventId = 5 },
            },
        };

        _mockDbContext.BaseEvents.Add(existingBaseEvent);
        await _mockDbContext.SaveChangesAsync();

        var baseEvents = new List<BaseEvent>
        {
            new BaseEvent
            {
                Title = "New Base Event modified",
                BaseEventId = 5,
                SellMode = "online",
                Events = new List<Event>
                {
                    new Event { EventId = 33, BaseEventId = 5 },
                },
            },
        };

        await _repository.SaveUpdatedEvents(baseEvents, CancellationToken.None);

        var updatedBaseEvent = await _mockDbContext.BaseEvents.FindAsync(5);
        Assert.NotNull(updatedBaseEvent);
        Assert.Equal("online", updatedBaseEvent.SellMode);
    }

    private Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> elements)
        where T : class
    {
        var queryable = elements.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet
            .As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(queryable.GetEnumerator());
        mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(elements.ToList().Add);
        mockSet
            .Setup(m => m.Update(It.IsAny<T>()))
            .Callback<T>(entity =>
            {
                var existing = elements.SingleOrDefault(e => e.Equals(entity));
                if (existing != null)
                {
                    elements.ToList().Remove(existing);
                    elements.ToList().Add(entity);
                }
            });

        return mockSet;
    }

    [Fact]
    public async Task GetEventsAsync_Returns_Events_When_Found()
    {
        var startsAt = DateTime.UtcNow.AddDays(-1);
        var endsAt = DateTime.UtcNow.AddDays(1);

        var eventList = new List<GetEventListDTO>
        {
            new GetEventListDTO
            {
                Id = Guid.NewGuid(),
                Title = "Test Event",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                MinPrice = 10,
                MaxPrice = 50,
            },
        };

        _mockDapperService
            .Setup(d => d.QueryAsync<GetEventListDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(eventList);

        var result = await _repository.GetEventsAsync(startsAt, endsAt, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Events);
    }

    [Fact]
    public async Task GetEventsAsync_Throws_Exception_On_Error()
    {
        _mockDapperService
            .Setup(d => d.QueryAsync<GetEventListDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(new Exception("Test exception"));

        await Assert.ThrowsAsync<Exception>(
            () => _repository.GetEventsAsync(null, null, CancellationToken.None)
        );
    }

    [Fact]
    public async Task SaveUpdatedEvents_Adds_Event_With_No_Zones()
    {
        var eventId = Guid.NewGuid();
        var baseEvents = new List<BaseEvent>
        {
            new BaseEvent
            {
                Title = "Event with No Zones",
                BaseEventId = 2,
                SellMode = "online",
                Events = new List<Event>
                {
                    new Event
                    {
                        Id = eventId,
                        EventId = 2,
                        BaseEventId = 2,
                        Zones = new List<Zone>(),
                    },
                },
            },
        };

        await _repository.SaveUpdatedEvents(baseEvents, CancellationToken.None);

        var addedEvent = await _mockDbContext.Events.SingleOrDefaultAsync(e => e.EventId == 2);
        Assert.NotNull(addedEvent);
        Assert.Empty(addedEvent.Zones);
    }

    [Fact]
    public async Task SaveUpdatedEvents_Updates_Event_Zones()
    {
        var existingEvent = new Event
        {
            Id = Guid.NewGuid(),
            EventId = 5,
            BaseEventId = 5,
            Zones = new List<Zone>
            {
                new Zone
                {
                    ZoneId = 1,
                    Capacity = 10,
                    Price = 5,
                    Name = "Old Zone",
                    Numbered = false,
                },
            },
        };

        var existingBaseEvent = new BaseEvent
        {
            Title = "Base Event with Old Zone",
            BaseEventId = 5,
            SellMode = "online",
            Events = new List<Event> { existingEvent },
        };

        _mockDbContext.BaseEvents.Add(existingBaseEvent);
        await _mockDbContext.SaveChangesAsync();

        var updatedBaseEvent = new BaseEvent
        {
            Title = "Base Event with Updated Zone",
            BaseEventId = 5,
            SellMode = "online",
            Events = new List<Event>
            {
                new Event
                {
                    EventId = 5,
                    BaseEventId = 5,
                    Zones = new List<Zone>
                    {
                        new Zone
                        {
                            ZoneId = 1,
                            Capacity = 20,
                            Price = 10,
                            Name = "Updated Zone",
                            Numbered = true,
                        },
                    },
                },
            },
        };

        await _repository.SaveUpdatedEvents(
            new List<BaseEvent> { updatedBaseEvent },
            CancellationToken.None
        );

        var updatedEvent = await _mockDbContext
            .Events.Include(e => e.Zones)
            .SingleOrDefaultAsync(e => e.EventId == 5);
        Assert.NotNull(updatedEvent);
        var updatedZone = updatedEvent.Zones.SingleOrDefault(z => z.ZoneId == 1);
        Assert.NotNull(updatedZone);
        Assert.Equal(20, updatedZone.Capacity);
        Assert.Equal(10, updatedZone.Price);
        Assert.Equal("Updated Zone", updatedZone.Name);
        Assert.True(updatedZone.Numbered);
    }

    [Fact]
    public async Task SaveUpdatedEvents_Removes_Old_Zones()
    {
        var eventId = Guid.NewGuid();
        var zoneId1 = Guid.NewGuid();
        var zoneId2 = Guid.NewGuid();
        var existingEvent = new Event
        {
            Id = eventId,
            EventId = 6,
            BaseEventId = 6,
            Zones = new List<Zone>
            {
                new Zone
                {
                    Id = zoneId1,
                    ZoneId = 1,
                    Capacity = 10,
                    Price = 5,
                    Name = "Zone To Be Removed",
                    Numbered = false,
                },
                new Zone
                {
                    Id = zoneId2,
                    ZoneId = 2,
                    Capacity = 20,
                    Price = 10,
                    Name = "Zone To Keep",
                    Numbered = true,
                },
            },
        };

        var existingBaseEvent = new BaseEvent
        {
            Title = "Base Event with Zones",
            BaseEventId = 6,
            SellMode = "online",
            Events = new List<Event> { existingEvent },
        };

        _mockDbContext.BaseEvents.Add(existingBaseEvent);
        await _mockDbContext.SaveChangesAsync();

        var updatedBaseEvent = new BaseEvent
        {
            Title = "Base Event with Updated Zones",
            BaseEventId = 6,
            SellMode = "online",
            Events = new List<Event>
            {
                new Event
                {
                    EventId = 6,
                    BaseEventId = 6,
                    Zones = new List<Zone>
                    {
                        new Zone
                        {
                            ZoneId = 2,
                            Capacity = 20,
                            Price = 10,
                            Name = "Zone To Keep",
                            Numbered = true,
                        },
                    },
                },
            },
        };

        await _repository.SaveUpdatedEvents(
            new List<BaseEvent> { updatedBaseEvent },
            CancellationToken.None
        );

        var remainingEvent = await _mockDbContext
            .Events.Include(e => e.Zones)
            .SingleOrDefaultAsync(e => e.EventId == 6);
        Assert.NotNull(remainingEvent);

        var removedZone = await _mockDbContext.Zones.FindAsync(zoneId1);
        Assert.Null(removedZone);

        var keptZone = await _mockDbContext.Zones.FindAsync(zoneId2);
        Assert.NotNull(keptZone);
    }

    [Fact]
    public async Task GetEventsAsync_Logs_Error_On_Exception()
    {
        var exception = new Exception("Test exception");
        _mockDapperService
            .Setup(d => d.QueryAsync<GetEventListDTO>(It.IsAny<string>(), It.IsAny<object>()))
            .ThrowsAsync(exception);

        var ex = await Assert.ThrowsAsync<Exception>(
            () =>
                _repository.GetEventsAsync(
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(1),
                    CancellationToken.None
                )
        );

        Assert.Equal(exception.Message, ex.Message);
    }
}
