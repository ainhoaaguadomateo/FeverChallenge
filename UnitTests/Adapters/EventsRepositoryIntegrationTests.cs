using Fever.Application.Interfaces;
using Fever.Domain.Features.Events.Entities;
using Fever.Domain.Features.GetEvents.Models;
using Fever.Infraestructure.Postgres;
using Fever.Infraestructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTests.Infraestructure.Repository
{
    public class EventsRepositoryIntegrationTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<IDapperService> _dapperServiceMock;
        private readonly Mock<ILogger<EventsRepository>> _loggerMock;
        private readonly EventsRepository _repository;

        public EventsRepositoryIntegrationTests()
        {
            // Configuración de la base de datos en memoria para pruebas
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "IntegrationTestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // Configuración del DapperService usando Moq
            _dapperServiceMock = new Mock<IDapperService>();

            // Configuración del Logger usando Moq
            _loggerMock = new Mock<ILogger<EventsRepository>>();

            // Configuración del IConfiguration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        {
                            "ConnectionStrings:PostgreSQL",
                            "Host=localhost;Database=test;Username=test;Password=test"
                        },
                    }
                )
                .Build();

            // Inicializar el repositorio
            _repository = new EventsRepository(
                _dbContext,
                _dapperServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task SaveUpdatedEvents_Integrates_Correctly_With_Real_DbContext()
        {
            // Arrange: Configurar datos iniciales
            var eventId = Guid.NewGuid();
            var baseEvents = new List<BaseEvent>
            {
                new BaseEvent
                {
                    Title = "New Event",
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
                                    ZoneId = 1,
                                    Capacity = 100,
                                    Price = 20,
                                    Name = "Zone 1",
                                    Numbered = true,
                                },
                            },
                        },
                    },
                },
            };

            // Act: Guardar los eventos
            await _repository.SaveUpdatedEvents(baseEvents, CancellationToken.None);

            // Assert: Verificar que los eventos se han guardado
            var savedBaseEvent = await _dbContext
                .BaseEvents.Include(be => be.Events)
                .ThenInclude(e => e.Zones)
                .FirstOrDefaultAsync(be => be.BaseEventId == 1);
            Assert.NotNull(savedBaseEvent);
            Assert.Single(savedBaseEvent.Events);
            Assert.Single(savedBaseEvent.Events[0].Zones);
        }

        [Fact]
        public async Task GetEventsAsync_Returns_Correct_Events_From_Db()
        {
            // Arrange: Configurar datos iniciales para Dapper
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
                },
            };

            _dapperServiceMock
                .Setup(d => d.QueryAsync<GetEventListDTO>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(eventList);

            // Act: Obtener eventos
            var result = await _repository.GetEventsAsync(
                DateTime.UtcNow.AddDays(-2),
                DateTime.UtcNow.AddDays(2),
                CancellationToken.None
            );

            // Assert: Verificar los resultados
            Assert.NotNull(result);
            Assert.Single(result.Events);
            Assert.Equal("Test Event", result.Events[0].Title);
        }
    }
}
