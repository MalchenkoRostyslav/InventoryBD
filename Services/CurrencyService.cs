namespace InventoryGame.Services;

public record CurrencyDto(long Iron, long Wood, long Scrap, string Text);

/// <summary>Конвертація "сирого" числа з БД у ігрову валюту. Курси змінюй тут.</summary>
public class CurrencyService
{
    public const long IronUnit = 10_000;  // 1 Залізо = 10000 сирих одиниць
    public const long WoodUnit = 100;     // 1 Дерево = 100 сирих одиниць (залишок < 100 = Тріски)

    public CurrencyDto Split(long raw)
    {
        raw = Math.Max(raw, 0);
        long iron = raw / IronUnit, wood = raw % IronUnit / WoodUnit, scrap = raw % WoodUnit;
        var parts = new List<string>();
        if (iron > 0) parts.Add($"{iron} Залізо");
        if (wood > 0) parts.Add($"{wood} Дерева");
        if (scrap > 0) parts.Add($"{scrap} Тріски");
        return new(iron, wood, scrap, parts.Count == 0 ? "0 Дерева" : string.Join(", ", parts));
    }

    public string Text(long raw) => Split(raw).Text;
}
