using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Health playerHealth;
    public Slider slider;

    void Update()
    {
        if (playerHealth == null || slider == null)
            return;

        slider.value = (float)playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }
}