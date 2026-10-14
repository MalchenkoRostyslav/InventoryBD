using Dapper;
using InventoryGame.Data;
using InventoryGame.Models;
using InventoryGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryGame.Controllers;

[Route("api/inventory")]
public class InventoryController(Db db, CurrencyService cur) : GameController(db, cur)
{
    [HttpGet]
    public Task<IActionResult> Get() => Run(async () =>
    {
        using var c = Db.Open();
        var items = await c.QueryAsync<ItemRow>("SELECT * FROM dbo.v_PlayerItems WHERE PlayerId = @PlayerId ORDER BY GridIndex", new { PlayerId });
        var slots = await c.QueryAsync<SlotRow>("SELECT SlotId, Name FROM dbo.EquipSlots ORDER BY SlotId");
        return Ok(new { capacity = 30, items, slots });
    });

    [HttpPost("equip")]
    public Task<IActionResult> Equip(EquipRequest r) => Run(async () =>
    {
        using var c = Db.Open();
        await c.ExecuteAsync("dbo.sp_EquipItem", new { PlayerId, r.InventoryId, r.Equip }, commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { ok = true });
    });

    [HttpPost("move")]
    public Task<IActionResult> Move(MoveRequest r) => Run(async () =>
    {
        using var c = Db.Open();
        await c.ExecuteAsync("dbo.sp_MoveItem", new { PlayerId, r.InventoryId, r.TargetIndex }, commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { ok = true });
    });
}
