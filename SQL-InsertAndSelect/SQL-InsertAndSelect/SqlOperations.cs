using Microsoft.Data.SqlClient;

namespace SQL_InsertAndSelect
{
    public class SqlOperations(string connectionString)
    {
        public async Task<IEnumerable<string>> Select(string? condition = null)
        {
            using var conn = new SqlConnection(connectionString);

            await conn.OpenAsync();

            var cmdText = "SELECT TestColumn FROM TestTable";
            if (condition != null)
            {
                cmdText += $" WHERE {condition}";
            }

            using var cmd = new SqlCommand(cmdText, conn);
            using var reader = cmd.ExecuteReader();

            var results = new List<string>();
            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }

            return results;
        }

        public async Task Insert(string value)
        {
            using var conn = new SqlConnection(connectionString);

            await conn.OpenAsync();

            var cmdText = "INSERT INTO TestTable VALUES (@value)";

            using var cmd = new SqlCommand(cmdText, conn);
            cmd.Parameters.AddWithValue("@value", value);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
