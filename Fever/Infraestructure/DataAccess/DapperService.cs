using System.Data;
using Dapper;
using Fever.Application.Interfaces;

namespace Fever.Infraestructure.DataAccess;

public class DapperService : IDapperService
{
    private readonly IDbConnection _connection;

    public DapperService(IDbConnection connection)
    {
        _connection = connection;
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
    {
        return _connection.QueryAsync<T>(sql, param);
    }
}
