using Microsoft.Data.SqlClient;
using System.Dynamic;

await using var conn = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;");

await conn.OpenAsync();

var data = await ExecuteQueryAsync("SELECT * FROM Blogs WHERE BlogId = 1", conn);

foreach (var item in data)
{
    Console.WriteLine(item.BlogId);
    Console.WriteLine(item.Url);
}

async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string sql, SqlConnection connection)
{
    using (var command = new SqlCommand(sql, connection))
    {
        var results = new List<dynamic>();
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var data = new ExpandoObject();
                var dict = (IDictionary<string, object>)data;

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    dict[reader.GetName(i)] = reader[i];
                }

                results.Add(data);
            }
        }

        return results;
    }
}