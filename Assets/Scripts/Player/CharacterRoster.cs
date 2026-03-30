using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterEntry
{
    public string CharacterId;
    public CharacterStats Stats;
    public string UnlockHint;
    public bool IsUnlocked;
}

[CreateAssetMenu(fileName = "CharacterRoster", menuName = "Para-Elementals/Character Roster")]
public class CharacterRoster : ScriptableObject
{
    public List<CharacterEntry> Characters = new List<CharacterEntry>();

    private const string SelectedCharacterKey = "SelectedCharacterId";

    public CharacterEntry GetSelectedCharacter()
    {
        string savedId = PlayerPrefs.GetString(SelectedCharacterKey, "");

        foreach (var c in Characters)
        {
            if (c.CharacterId == savedId && c.IsUnlocked)
                return c;
        }

        foreach (var c in Characters)
        {
            if (c.IsUnlocked)
                return c;
        }

        return null;
    }

    public void SelectCharacter(string characterId)
    {
        PlayerPrefs.SetString(SelectedCharacterKey, characterId);
        PlayerPrefs.Save();
    }

    public void UnlockCharacter(string characterId)
    {
        foreach (var c in Characters)
        {
            if (c.CharacterId == characterId)
            {
                c.IsUnlocked = true;
                return;
            }
        }
    }
}
