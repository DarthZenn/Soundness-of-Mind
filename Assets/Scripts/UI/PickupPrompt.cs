using TMPro;
using UnityEngine;

public class PickupPrompt : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    private bool isActive = false;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
        else
            Debug.LogError("Prompt UI is not assigned.");

        if (promptText == null)
            Debug.LogError("Prompt Text is not assigned.");
    }

    private void Update()
    {
        if (isActive && Input.GetButtonDown("Interact"))
        {
            ClosePrompt();
        }
    }

    public void ShowPrompt(string itemName, int quantity)
    {
        if (promptUI == null || promptText == null)
        {
            Debug.LogWarning("UI references are missing.");
            return;
        }

        promptText.text = $"You have picked up: <b>{itemName}</b> x{quantity}";
        promptUI.SetActive(true);
        Time.timeScale = 0f;
        isActive = true;
        GlobalControl.Instance.isPrompt = isActive;
    }

    public void ShowPromptFull()
    {
        if (promptUI == null || promptText == null)
        {
            Debug.LogWarning("UI references are missing.");
            return;
        }

        promptText.text = $"Your Inventory is full";
        promptUI.SetActive(true);
        Time.timeScale = 0f;
        isActive = true;
        GlobalControl.Instance.isPrompt = isActive;
    }

    private void ClosePrompt()
    {
        promptUI.SetActive(false);
        Time.timeScale = 1f;
        isActive = false;
        GlobalControl.Instance.isPrompt = isActive;
    }

    public bool IsPromptActive() => isActive;
}
