using InventoryGame.Data;
using InventoryGame.Models;
using InventoryGame.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InventoryGame.Controllers;

[ApiController]
public abstract class GameController(Db db, CurrencyService cur) : ControllerBase
{
    protected readonly Db Db = db;
    protected readonly CurrencyService Cur = cur;

    /// <summary>Id гравця з заголовка X-Player-Id (спрощена авторизація).</summary>
    protected int PlayerId => int.TryParse(Request.Headers["X-Player-Id"], out var id) ? id : 0;

    protected object Profile(PlayerRow p) => new
    {
        p.PlayerId, p.Nickname, wealth = p.Wealth, currency = Cur.Split(p.Wealth),
        gearScore = p.TotalGearScore, p.ItemsCount, p.EquippedCount
    };

    /// <summary>Помилки з THROW 5xxxx у процедурах повертаємо як 400 з текстом.</summary>
    protected async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try { return await action(); }
        catch (SqlException ex) when (ex.Number >= 50000) { return BadRequest(new { error = ex.Message }); }
    }
}
