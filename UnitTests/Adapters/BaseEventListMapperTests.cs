using Fever.Infraestructure.EventsProviders.Mappers;
using Fever.Presentation.Features.Events.DTOs;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Mappers;

public class BaseEventListMapperTests
{
    private readonly Mock<ILogger> _mockLogger;

    public BaseEventListMapperTests()
    {
        _mockLogger = new Mock<ILogger>();
    }

    [Fact]
    public void CreateBaseEvent_ShouldReturnNull_WhenBaseEventDtoIsInvalid()
    {
        var baseEventDto = new BaseEventDTO
        {
            BaseEventId = 0,
            SellMode = null!,
            Title = null!,
            Events = new List<EventDTO>
            {
                new EventDTO
                {
                    EventId = 1,
                    Zones = new List<ZoneDTO>
                    {
                        new ZoneDTO { ZoneId = 3 },
                        new ZoneDTO { ZoneId = 3 },
                    },
                },
            },
        };

        var result = BaseEventListMapper.CreateBaseEvent(baseEventDto, _mockLogger.Object);

        Assert.Null(result);
    }

    [Fact]
    public void CreateBaseEvent_ShouldReturnBaseEvent_WhenBaseEventDtoIsValid()
    {
        var baseEventDto = new BaseEventDTO
        {
            BaseEventId = 1,
            SellMode = "Online",
            Title = "Test Event",
            Events = new List<EventDTO>
            {
                new EventDTO
                {
                    EventId = 1,
                    EventStartDate = "2024-08-22T10:00:00",
                    EventEndDate = "2024-08-22T12:00:00",
                    SellFrom = "2024-08-20T09:00:00",
                    SellTo = "2024-08-22T09:59:00",
                    SoldOut = false,
                    Zones = new List<ZoneDTO>(),
                },
            },
        };

        var result = BaseEventListMapper.CreateBaseEvent(baseEventDto, _mockLogger.Object);

        Assert.NotNull(result);
        Assert.Equal(baseEventDto.BaseEventId, result.BaseEventId);
        Assert.Equal(baseEventDto.SellMode, result.SellMode);
        Assert.Equal(baseEventDto.Title, result.Title);
        Assert.Single(result.Events);
    }

    [Fact]
    public void CreateEvent_ShouldReturnNull_WhenAnyDateTimeIsInvalid()
    {
        var eventDto = new EventDTO
        {
            EventId = 1,
            EventStartDate = "invalid-date",
            EventEndDate = "2024-08-22T12:00:00",
            SellFrom = "2024-08-20T09:00:00",
            SellTo = "2024-08-22T09:59:00",
            SoldOut = false,
            Zones = new List<ZoneDTO>(),
        };

        var result = BaseEventListMapper.CreateEvent(eventDto, 1, _mockLogger.Object);

        Assert.Null(result);
    }

    [Fact]
    public void CreateEvent_ShouldReturnEvent_WhenAllDateTimesAreValid()
    {
        var eventDto = new EventDTO
        {
            EventId = 1,
            EventStartDate = "2024-08-22T10:00:00",
            EventEndDate = "2024-08-22T12:00:00",
            SellFrom = "2024-08-20T09:00:00",
            SellTo = "2024-08-22T09:59:00",
            SoldOut = false,
            Zones = new List<ZoneDTO>(),
        };

        var result = BaseEventListMapper.CreateEvent(eventDto, 1, _mockLogger.Object);

        Assert.NotNull(result);
        Assert.Equal(eventDto.EventId, result.EventId);
        Assert.Equal(1, result.BaseEventId);
        Assert.Equal(DateTime.Parse(eventDto.EventStartDate), result.EventStartDate);
        Assert.Equal(DateTime.Parse(eventDto.EventEndDate), result.EventEndDate);
        Assert.Equal(DateTime.Parse(eventDto.SellFrom), result.SellFrom);
        Assert.Equal(DateTime.Parse(eventDto.SellTo), result.SellTo);
        Assert.False(result.SoldOut);
        Assert.Empty(result.Zones);
    }

    [Fact]
    public void CreateZone_ShouldMapPropertiesCorrectly()
    {
        var zoneDto = new ZoneDTO
        {
            ZoneId = 1,
            Capacity = 100,
            Price = 20.5m,
            Name = "VIP",
            Numbered = true,
        };

        var result = BaseEventListMapper.CreateZone(zoneDto);

        Assert.NotNull(result);
        Assert.Equal(zoneDto.ZoneId, result.ZoneId);
        Assert.Equal(zoneDto.Capacity, result.Capacity);
        Assert.Equal(zoneDto.Price, result.Price);
        Assert.Equal(zoneDto.Name, result.Name);
        Assert.Equal(zoneDto.Numbered, result.Numbered);
    }

    [Fact]
    public void ParseDateTime_ShouldReturnDateTime_WhenInputIsValid()
    {
        var validDateString = "2024-08-22T10:00:00";

        var result = BaseEventListMapper.ParseDateTime(validDateString, _mockLogger.Object);

        Assert.NotNull(result);
        Assert.Equal(DateTime.Parse(validDateString), result);
    }

    [Fact]
    public void ParseDateTime_ShouldReturnNull_WhenInputIsInvalid()
    {
        var invalidDateString = "invalid-date";

        var result = BaseEventListMapper.ParseDateTime(invalidDateString, _mockLogger.Object);

        Assert.Null(result);
        _mockLogger.Verify(
            logger =>
                logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                ),
            Times.Once
        );
    }
}
