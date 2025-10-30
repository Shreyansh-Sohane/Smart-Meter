using SharedLibrary.Models;
using Npgsql;

namespace SharedLibrary.Utils
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsertMeterReadingAsync(MeterReading data)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                string query = @"
                    INSERT INTO meterreading (meterid, meterreadingdate, energyconsumed, voltage, current)
                    VALUES (@meterid, @meterreadingdate, @energyconsumed, @voltage, @current);";

                await using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@meterid", data.meterid);
                cmd.Parameters.AddWithValue("@meterreadingdate", DateTime.Parse(data.meterreadingdate));
                cmd.Parameters.AddWithValue("@energyconsumed", data.energyconsumed);
                cmd.Parameters.AddWithValue("@voltage", data.voltage);
                cmd.Parameters.AddWithValue("@current", data.current);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB Error] {ex.Message}");
                throw;
            }
        }
    }

}