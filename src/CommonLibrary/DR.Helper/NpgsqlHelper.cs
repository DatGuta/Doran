using System.Data;
using Npgsql;

namespace DR.Helper;

public static class NpgsqlHelper {

    public static async Task ExecuteReaderAsync(string connectionString, string sql, Action<NpgsqlDataReader> action, CancellationToken cancellationToken) {
        using (var conn = new NpgsqlConnection(connectionString))
        using (var cmd = conn.CreateCommand()) {
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 60;

            if (conn.State != ConnectionState.Open) {
                await conn.OpenAsync(cancellationToken);
            }

            using (var reader = await cmd.ExecuteReaderAsync(cancellationToken)) {
                while (reader.Read()) action(reader);
            }
        }
    }

    public static async Task<object?> ExecuteScalarAsync(string connectionString, string sql, CancellationToken cancellationToken) {
        using (var conn = new NpgsqlConnection(connectionString))
        using (var cmd = conn.CreateCommand()) {
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 60;

            if (conn.State != ConnectionState.Open) {
                await conn.OpenAsync(cancellationToken);
            }

            return await cmd.ExecuteScalarAsync(cancellationToken);
        }
    }

    public static async Task<T> ExecuteScalarAsync<T>(string connectionString, string sql, CancellationToken cancellationToken) {
        return (T)(await ExecuteScalarAsync(connectionString, sql, cancellationToken))!;
    }
}
