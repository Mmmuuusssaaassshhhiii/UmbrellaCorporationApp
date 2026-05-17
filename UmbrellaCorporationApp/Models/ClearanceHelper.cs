namespace UmbrellaCorp.Helpers;

using UmbrellaCorp.Models.Enums;

public static class ClearanceHelper
{
    public static string GetName(
        ClearanceLevel level)
    {
        return level switch
        {
            ClearanceLevel.Level1 =>
                "Уровень 1 — Обслуживание",

            ClearanceLevel.Level2 =>
                "Уровень 2 — Охрана",

            ClearanceLevel.Level3 =>
                "Уровень 3 — Младший ученый",

            ClearanceLevel.Level4 =>
                "Уровень 4 — Исследователь",

            ClearanceLevel.Level5 =>
                "Уровень 5 — Вирусолог",

            ClearanceLevel.Level6 =>
                "Уровень 6 — U.S.S.",

            ClearanceLevel.Level7 =>
                "Уровень 7 — Глава отдела",

            ClearanceLevel.Level8 =>
                "Уровень 8 — Директор",

            ClearanceLevel.Level9 =>
                "Уровень 9 — Совет Umbrella",

            ClearanceLevel.Level10 =>
                "Уровень 10 — RED QUEEN",

            _ => "UNKNOWN"
        };
    }
}