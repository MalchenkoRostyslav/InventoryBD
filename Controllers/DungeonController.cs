using Dapper;
using InventoryGame.Data;
using InventoryGame.Models;
using InventoryGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryGame.Controllers;

[Route("api/dungeon")]
public class DungeonController(Db db, CurrencyService cur) : GameController(db, cur)
{
    [HttpGet]
    public Task<IActionResult> Get() => Run(async () =>
    {
        using var c = Db.Open();
        var rows = await c.QueryAsync<DungeonRow>(
            @"SELECT d.DungeonId, d.Name, d.RequiredGearScore, d.MinReward, d.MaxReward, d.CooldownSeconds,
                     CASE WHEN pd.LastCompletedAt IS NULL THEN 0
                          WHEN DATEADD(SECOND, d.CooldownSeconds, pd.LastCompletedAt) <= SYSDATETIME() THEN 0
                          ELSE DATEDIFF(SECOND, SYSDATETIME(), DATEADD(SECOND, d.CooldownSeconds, pd.LastCompletedAt)) END AS SecondsLeft
              FROM dbo.Dungeons d
              LEFT JOIN dbo.PlayerDungeons pd ON pd.DungeonId = d.DungeonId AND pd.PlayerId = @PlayerId
              ORDER BY d.RequiredGearScore", new { PlayerId });
        return Ok(rows.Select(d => new
        {
            d.DungeonId, d.Name, d.RequiredGearScore, d.CooldownSeconds, d.SecondsLeft,
            rewardText = $"{Cur.Text(d.MinReward)} — {Cur.Text(d.MaxReward)}"
        }));
    });

    [HttpPost("enter")]
    public Task<IActionResult> Enter(EnterRequest r) => Run(async () =>
    {
        using var c = Db.Open();
        var res = await c.QuerySingleAsync<DungeonResult>("dbo.sp_CompleteDungeon", new { PlayerId, r.DungeonId }, commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new
        {
            reward = res.Reward, rewardText = Cur.Text(res.Reward),
            wealth = res.NewWealth, currency = Cur.Split(res.NewWealth), res.CooldownSeconds
        });
    });
}
