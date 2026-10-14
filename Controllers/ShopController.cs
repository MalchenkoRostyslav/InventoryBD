using Dapper;
using InventoryGame.Data;
using InventoryGame.Models;
using InventoryGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryGame.Controllers;

[Route("api/shop")]
public class ShopController(Db db, CurrencyService cur) : GameController(db, cur)
{
    [HttpGet]
    public Task<IActionResult> Get() => Run(async () =>
    {
        using var c = Db.Open();
        var rows = await c.QueryAsync<ShopRow>(
            @"SELECT i.ItemId, i.Name, i.SlotId, s.Name AS SlotName, i.Power, i.Cost, i.IconPath
              FROM dbo.Items i LEFT JOIN dbo.EquipSlots s ON s.SlotId = i.SlotId
              ORDER BY i.Cost, i.ItemId");
        return Ok(rows.Select(r => new { r.ItemId, r.Name, r.SlotName, r.Power, r.Cost, r.IconPath, costText = Cur.Text(r.Cost) }));
    });

    [HttpPost("buy")]
    public Task<IActionResult> Buy(BuyRequest r) => Run(async () =>
    {
        using var c = Db.Open();
        var res = await c.QuerySingleAsync<WealthResult>("dbo.sp_BuyItem", new { PlayerId, r.ItemId }, commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { wealth = res.NewWealth, currency = Cur.Split(res.NewWealth) });
    });
}
