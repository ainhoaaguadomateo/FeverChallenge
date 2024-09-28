using Fever.Infraestructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Moq;

public static class MockDbSetExtensions
{
    public static Mock<DbSet<T>> CreateDbSetMock<T>(this IEnumerable<T> elements)
        where T : class
    {
        var queryable = elements.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock
            .As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(queryable.GetEnumerator());

        dbSetMock.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(elements.ToList().Add);

        return dbSetMock;
    }

    public static void ReturnsDbSet<T>(this Mock<DbSet<T>> dbSetMock, IEnumerable<T> elements)
        where T : class
    {
        var queryable = elements.AsQueryable();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock
            .As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(queryable.GetEnumerator());
    }

    public static void ReturnsDbSet<T>(
        this Mock<ApplicationDbContext> dbContextMock,
        IEnumerable<T> elements
    )
        where T : class
    {
        var dbSetMock = elements.CreateDbSetMock();
        dbContextMock.Setup(x => x.Set<T>()).Returns(dbSetMock.Object);
    }
}
