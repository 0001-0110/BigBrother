using Npgsql;

namespace BigBrother.Reminders.Repositories;

public class SqlRepository
{
    private readonly string _connectionString;

    // TODO Switch to IOptions rather than a string
    public SqlRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected async Task<TResult> Execute<TResult>(Func<NpgsqlConnection, NpgsqlCommand> buildCommand, Func<NpgsqlDataReader, Task<TResult>> read)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using NpgsqlCommand command = buildCommand(connection);
        using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        return await read(reader);
    }

    protected async Task<int> ExecuteNonQuery(Func<NpgsqlConnection, NpgsqlCommand> buildCommand)
    {
        using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        using NpgsqlCommand command = buildCommand(connection);
        return await command.ExecuteNonQueryAsync();
    }
}
