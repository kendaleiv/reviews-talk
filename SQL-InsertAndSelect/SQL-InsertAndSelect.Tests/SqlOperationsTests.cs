using Microsoft.Data.SqlClient;
using TemporaryDb;

namespace SQL_InsertAndSelect.Tests
{
    public class SqlOperationsTests : IAsyncLifetime
    {
        private readonly TempLocalDb db;
        private readonly SqlOperations sqlOperations;

        public SqlOperationsTests()
        {
            db = new TempLocalDb("testdb");
            sqlOperations = new SqlOperations(db.ConnectionString);
        }

        public async Task InitializeAsync()
        {
            using var conn = new SqlConnection(db.ConnectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("CREATE TABLE TestTable (TestColumn varchar(255))", conn);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DisposeAsync()
        {
            using var conn = new SqlConnection(db.ConnectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("DROP TABLE TestTable", conn);
            await cmd.ExecuteNonQueryAsync();
        }

        [Fact]
        public async Task Select_ShouldRetrieveExpectedItems_WithoutCondition()
        {
            // Arrange
            var expected = new List<string> { "test1", "test2" };

            using (var conn = new SqlConnection(db.ConnectionString))
            {
                await conn.OpenAsync();

                foreach (var item in expected)
                {
                    var cmdText = "INSERT INTO TestTable VALUES (@value)";
                    using var cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@value", item);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act
            var results = await sqlOperations.Select();

            // Assert
            Assert.Equal(expected, results);
        }

        [Fact]
        public async Task Select_ShouldRetrieveExpectedItems_WithCondition()
        {
            // Arrange
            var expected = new List<string> { "test1", "test2" };

            using (var conn = new SqlConnection(db.ConnectionString))
            {
                await conn.OpenAsync();

                foreach (var item in expected)
                {
                    var cmdText = "INSERT INTO TestTable VALUES (@value)";
                    using var cmd = new SqlCommand(cmdText, conn);
                    cmd.Parameters.AddWithValue("@value", item);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act
            var results = await sqlOperations.Select("TestColumn = 'test1'");

            // Assert
            var result = Assert.Single(results);
            Assert.Equal("test1", result);
        }

        [Fact]
        public async Task Insert_ShouldCreateExpectedItem()
        {
            // Arrange

            // Act
            await sqlOperations.Insert("test1");

            // Assert
            using var conn = new SqlConnection(db.ConnectionString);
            await conn.OpenAsync();

            var cmdText = "SELECT TestColumn FROM TestTable";
            using var cmd = new SqlCommand(cmdText, conn);
            using var reader = cmd.ExecuteReader();

            var results = new List<string>();
            while (reader.Read())
            {
                results.Add(reader.GetString(0));
            }

            var result = Assert.Single(results);
            Assert.Equal("test1", result);
        }
    }
}
