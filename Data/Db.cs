using Microsoft.Data.SqlClient;

namespace InventoryGame.Data;

public class Db(IConfiguration cfg)
{
    private readonly string _cs = cfg.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Немає ConnectionStrings:Default");
    public SqlConnection Open() => new(_cs);   // Dapper сам відкриває/закриває з'єднання
}
