namespace InventoryGame.Models;

public class PlayerRow
{
    public int PlayerId { get; set; }
    public string Nickname { get; set; } = "";
    public long Wealth { get; set; }
    public int TotalGearScore { get; set; }
    public int ItemsCount { get; set; }
    public int EquippedCount { get; set; }
}

public class ItemRow
{
    public int InventoryId { get; set; }
    public int ItemId { get; set; }
    public int GridIndex { get; set; }
    public bool IsEquipped { get; set; }
    public int RarityId { get; set; }
    public string ItemName { get; set; } = "";
    public int? SlotId { get; set; }
    public string? SlotName { get; set; }
    public int BasePower { get; set; }
    public long Cost { get; set; }
    public string? IconPath { get; set; }
    public string RarityName { get; set; } = "";
    public string ColorHex { get; set; } = "#9d9d9d";
    public decimal StatMultiplier { get; set; }
    public int EffectivePower { get; set; }
}

public class SlotRow { public int SlotId { get; set; } public string Name { get; set; } = ""; }

public class ShopRow
{
    public int ItemId { get; set; }
    public string Name { get; set; } = "";
    public int? SlotId { get; set; }
    public string? SlotName { get; set; }
    public int Power { get; set; }
    public long Cost { get; set; }
    public string? IconPath { get; set; }
}

public class DungeonRow
{
    public int DungeonId { get; set; }
    public string Name { get; set; } = "";
    public int RequiredGearScore { get; set; }
    public long MinReward { get; set; }
    public long MaxReward { get; set; }
    public int CooldownSeconds { get; set; }
    public int SecondsLeft { get; set; }
}

public class WealthResult { public long NewWealth { get; set; } }
public class DungeonResult { public long Reward { get; set; } public long NewWealth { get; set; } public int CooldownSeconds { get; set; } }

public record LoginRequest(string Nickname);
public record BuyRequest(int ItemId);
public record EquipRequest(int InventoryId, bool Equip);
public record MoveRequest(int InventoryId, int TargetIndex);
public record EnterRequest(int DungeonId);
