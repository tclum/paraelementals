using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [SerializeField] private Health _playerHealth;
    [SerializeField] private InventoryManager _inventory;

    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _inventoryText;

    private void Update()
    {
        if (_playerHealth != null)
        {
            _healthText.text =
                "Health: " +
                _playerHealth.CurrentHealth +
                " / " +
                _playerHealth.MaxHealth;
        }

        if (_inventory != null)
        {
            int wood = _inventory.GetItemCount("wood");
            int slime = _inventory.GetItemCount("slime_gel");

            _inventoryText.text =
                "Wood: " + wood +
                "\nSlime Gel: " + slime;
        }
    }
}