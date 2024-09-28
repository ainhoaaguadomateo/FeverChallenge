using System.Data;
using Dapper;

namespace Fever.Application.Interfaces;

public interface IDapperService
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null!);
}
