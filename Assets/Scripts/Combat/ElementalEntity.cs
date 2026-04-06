using UnityEngine;

/// <summary>
/// Attach to any entity (player, enemy, boss) to give it an elemental type.
/// This element determines its resistances and weaknesses when receiving damage.
/// </summary>
public class ElementalEntity : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] private Element _element = Element.None;

    public Element EntityElement => _element;

    public void SetElement(Element element)
    {
        _element = element;
    }

    /// <summary>
    /// Calculate final damage after applying elemental multiplier.
    /// </summary>
    public int CalculateIncomingDamage(int baseDamage, Element attackElement)
    {
        float multiplier = ElementalSystem.GetMultiplier(attackElement, _element);
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * multiplier));
    }

    /// <summary>
    /// Returns whether a status effect should be applied based on element matchup.
    /// </summary>
    public bool RollStatusEffect(Element attackElement)
    {
        float chance = ElementalSystem.GetStatusChance(attackElement, _element);
        return Random.value < chance;
    }
}
