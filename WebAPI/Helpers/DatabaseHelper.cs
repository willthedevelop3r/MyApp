using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> ExecuteAsync(string sql, object parameters = null)
    {
        using (IDbConnection conn = new SqliteConnection(_connectionString))
        {
            return await conn.ExecuteAsync(sql, parameters);
        }
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null)
    {
        using (IDbConnection conn = new SqliteConnection(_connectionString))
        {
            return await conn.QueryAsync<T>(sql, parameters);
        }
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object parameters = null)
    {
        using (IDbConnection conn = new SqliteConnection(_connectionString))
        {
            return await conn.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }
    }
}