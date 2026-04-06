using UnityEngine;

public enum Element
{
    None,
    Fire,
    Water,
    Wind,
    Earth,
    Lightning
}

public enum StatusEffect
{
    None,
    Burn,       // Fire — damage over time
    Slow,       // Water — reduced move speed
    Knockback,  // Wind — pushed away
    Stun,       // Earth — brief freeze
    Shock       // Lightning — reduced attack speed
}

public static class ElementalSystem
{
    // Returns damage multiplier when attacker element hits defender element
    public static float GetMultiplier(Element attackElement, Element defenderElement)
    {
        if (attackElement == Element.None || defenderElement == Element.None)
            return 1f;

        // Strong matchups — 2x damage
        if (IsStrong(attackElement, defenderElement))
            return 2f;

        // Weak matchups — 0.5x damage
        if (IsWeak(attackElement, defenderElement))
            return 0.5f;

        return 1f;
    }

    // Returns the status effect for an element (on successful hit)
    public static StatusEffect GetStatusEffect(Element element)
    {
        switch (element)
        {
            case Element.Fire:      return StatusEffect.Burn;
            case Element.Water:     return StatusEffect.Slow;
            case Element.Wind:      return StatusEffect.Knockback;
            case Element.Earth:     return StatusEffect.Stun;
            case Element.Lightning: return StatusEffect.Shock;
            default:                return StatusEffect.None;
        }
    }

    // Chance to apply status effect (can be modified by resistance)
    public static float GetStatusChance(Element attackElement, Element defenderElement)
    {
        if (attackElement == Element.None) return 0f;
        if (IsStrong(attackElement, defenderElement)) return 0.8f;  // 80% on strong
        if (IsWeak(attackElement, defenderElement)) return 0.1f;    // 10% on weak
        return 0.4f;                                                 // 40% neutral
    }

    public static string GetElementColor(Element element)
    {
        switch (element)
        {
            case Element.Fire:      return "#FF4400";
            case Element.Water:     return "#00AAFF";
            case Element.Wind:      return "#88FF88";
            case Element.Earth:     return "#AA7744";
            case Element.Lightning: return "#FFFF00";
            default:                return "#FFFFFF";
        }
    }

    public static string GetElementEmoji(Element element)
    {
        switch (element)
        {
            case Element.Fire:      return "🔥";
            case Element.Water:     return "💧";
            case Element.Wind:      return "🌪️";
            case Element.Earth:     return "🪨";
            case Element.Lightning: return "⚡";
            default:                return "";
        }
    }

    private static bool IsStrong(Element attacker, Element defender)
    {
        // Fire > Earth
        if (attacker == Element.Fire && defender == Element.Earth) return true;
        // Water > Fire
        if (attacker == Element.Water && defender == Element.Fire) return true;
        // Wind > Earth
        if (attacker == Element.Wind && defender == Element.Earth) return true;
        // Earth > Lightning
        if (attacker == Element.Earth && defender == Element.Lightning) return true;
        // Lightning > Water
        if (attacker == Element.Lightning && defender == Element.Water) return true;

        return false;
    }

    private static bool IsWeak(Element attacker, Element defender)
    {
        // Fire weak vs Water
        if (attacker == Element.Fire && defender == Element.Water) return true;
        // Water weak vs Lightning
        if (attacker == Element.Water && defender == Element.Lightning) return true;
        // Wind weak vs Fire
        if (attacker == Element.Wind && defender == Element.Fire) return true;
        // Earth weak vs Wind
        if (attacker == Element.Earth && defender == Element.Wind) return true;
        // Lightning weak vs Earth
        if (attacker == Element.Lightning && defender == Element.Earth) return true;

        return false;
    }
}
