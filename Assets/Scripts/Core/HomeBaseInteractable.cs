using UnityEngine;
using UnityEngine.InputSystem;

public class HomeBaseInteractable : MonoBehaviour
{
    public enum InteractableType
    {
        DungeonPortal,
        CraftingStation,
        ItemStorage,
        Shop,
        SkillUpgrade,
        CharacterSelect
    }

    [Header("Settings")]
    [SerializeField] private InteractableType _type;
    [SerializeField] private string _promptText = "Press F to interact";
    [SerializeField] private float _interactRadius = 2.5f;

    private Transform _player;
    private bool _playerInRange;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        bool inRange = dist <= _interactRadius;

        if (inRange != _playerInRange)
        {
            _playerInRange = inRange;
            if (HomeBaseManager.Instance != null)
                HomeBaseManager.Instance.ShowInteractPrompt(_promptText, inRange);
        }

        if (_playerInRange && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            Interact();
    }

    private void Interact()
    {
        switch (_type)
        {
            case InteractableType.DungeonPortal:
                if (HomeBaseManager.Instance != null)
                    HomeBaseManager.Instance.EnterDungeon();
                break;
            case InteractableType.CraftingStation:
                if (HomeBaseManager.Instance != null)
                    HomeBaseManager.Instance.OpenCrafting();
                break;
            case InteractableType.ItemStorage:
                if (HomeBaseManager.Instance != null)
                    HomeBaseManager.Instance.OpenStorage();
                break;
            case InteractableType.Shop:
                if (HomeBaseManager.Instance != null)
                    HomeBaseManager.Instance.OpenShop();
                break;
            case InteractableType.SkillUpgrade:
                if (HomeBaseManager.Instance != null)
                    HomeBaseManager.Instance.OpenSkills();
                break;
            case InteractableType.CharacterSelect:
                if (CharacterSelectManager.Instance != null)
                    CharacterSelectManager.Instance.Open();
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}
