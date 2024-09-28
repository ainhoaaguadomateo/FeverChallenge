using Fever.Domain.Features.GetEvents.Models;
using Fever.Infraestructure.EventsProviders.Mappers;

namespace UnitTests.Mappers
{
    public class EventListResponseMapperTests
    {
        [Fact]
        public void MapEvents_ShouldReturnEmptyResponse_WhenEventListIsEmpty()
        {
            var eventListDto = new List<GetEventListDTO>();

            var result = eventListDto.MapEvents();

            Assert.NotNull(result);
            Assert.Empty(result.Events);
        }

        [Fact]
        public void MapEvents_ShouldMapAllPropertiesCorrectly()
        {
            var eventListDto = new List<GetEventListDTO>
            {
                new GetEventListDTO
                {
                    Id = Guid.NewGuid(),
                    Title = "Event 1",
                    StartDate = new DateTime(2024, 8, 22, 10, 0, 0),
                    EndDate = new DateTime(2024, 8, 22, 12, 0, 0),
                    MinPrice = 50.00m,
                    MaxPrice = 100.00m,
                },
                new GetEventListDTO
                {
                    Id = Guid.NewGuid(),
                    Title = "Event 2",
                    StartDate = new DateTime(2024, 9, 1, 14, 0, 0),
                    EndDate = new DateTime(2024, 9, 1, 16, 0, 0),
                    MinPrice = 20.00m,
                    MaxPrice = 80.00m,
                },
            };

            var result = eventListDto.MapEvents();

            Assert.NotNull(result);
            Assert.Equal(2, result.Events.Count);

            var firstEvent = result.Events.First();
            Assert.Equal(eventListDto[0].Id, firstEvent.Id);
            Assert.Equal(eventListDto[0].Title, firstEvent.Title);
            Assert.Equal(DateOnly.FromDateTime(eventListDto[0].StartDate), firstEvent.StartDate);
            Assert.Equal(TimeOnly.FromDateTime(eventListDto[0].StartDate), firstEvent.StartTime);
            Assert.Equal(DateOnly.FromDateTime(eventListDto[0].EndDate), firstEvent.EndDate);
            Assert.Equal(TimeOnly.FromDateTime(eventListDto[0].EndDate), firstEvent.EndTime);
            Assert.Equal(eventListDto[0].MinPrice, firstEvent.MinPrice);
            Assert.Equal(eventListDto[0].MaxPrice, firstEvent.MaxPrice);
        }

        [Fact]
        public void MapEvents_ShouldHandleSingleEventMapping()
        {
            var singleEvent = new GetEventListDTO
            {
                Id = Guid.NewGuid(),
                Title = "Single Event",
                StartDate = new DateTime(2024, 10, 5, 18, 30, 0),
                EndDate = new DateTime(2024, 10, 5, 21, 30, 0),
                MinPrice = 30.00m,
                MaxPrice = 75.00m,
            };
            var eventListDto = new List<GetEventListDTO> { singleEvent };

            var result = eventListDto.MapEvents();

            Assert.NotNull(result);
            Assert.Single(result.Events);

            var mappedEvent = result.Events.First();
            Assert.Equal(singleEvent.Id, mappedEvent.Id);
            Assert.Equal(singleEvent.Title, mappedEvent.Title);
            Assert.Equal(DateOnly.FromDateTime(singleEvent.StartDate), mappedEvent.StartDate);
            Assert.Equal(TimeOnly.FromDateTime(singleEvent.StartDate), mappedEvent.StartTime);
            Assert.Equal(DateOnly.FromDateTime(singleEvent.EndDate), mappedEvent.EndDate);
            Assert.Equal(TimeOnly.FromDateTime(singleEvent.EndDate), mappedEvent.EndTime);
            Assert.Equal(singleEvent.MinPrice, mappedEvent.MinPrice);
            Assert.Equal(singleEvent.MaxPrice, mappedEvent.MaxPrice);
        }
    }
}
