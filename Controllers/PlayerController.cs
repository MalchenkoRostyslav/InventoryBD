using Dapper;
using InventoryGame.Data;
using InventoryGame.Models;
using InventoryGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryGame.Controllers;

[Route("api/player")]
public class PlayerController(Db db, CurrencyService cur) : GameController(db, cur)
{
    const long StartWealth = 15_000;  // стартовий баланс нового героя (сирі одиниці)

    [HttpPost("login")]
    public Task<IActionResult> Login(LoginRequest req) => Run(async () =>
    {
        var nick = (req.Nickname ?? "").Trim();
        if (nick.Length < 2 || nick.Length > 20)
            return BadRequest(new { error = "Нікнейм має містити від 2 до 20 символів." });

        using var c = Db.Open();
        var id = await c.ExecuteScalarAsync<int?>("SELECT PlayerId FROM dbo.Players WHERE Nickname = @nick", new { nick });
        id ??= await c.ExecuteScalarAsync<int>(
            "INSERT dbo.Players (Nickname, Wealth, TotalGearScore) OUTPUT INSERTED.PlayerId VALUES (@nick, @w, 0)",
            new { nick, w = StartWealth });

        var p = await c.QuerySingleAsync<PlayerRow>("SELECT * FROM dbo.v_PlayerFullStats WHERE PlayerId = @id", new { id });
        return Ok(Profile(p));
    });

    [HttpGet("me")]
    public Task<IActionResult> Me() => Run(async () =>
    {
        using var c = Db.Open();
        var p = await c.QuerySingleOrDefaultAsync<PlayerRow>("SELECT * FROM dbo.v_PlayerFullStats WHERE PlayerId = @PlayerId", new { PlayerId });
        return p is null ? Unauthorized(new { error = "Гравця не знайдено. Увійди знову." }) : Ok(Profile(p));
    });
}
