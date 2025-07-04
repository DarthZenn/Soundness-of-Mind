using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] private GameObject savePromptUI;

    public void OnClickYesSave()
    {
        GlobalControl.Instance.SaveGame();
        CloseSavePrompt();
    }

    public void OnClickNotNow()
    {
        CloseSavePrompt();
    }

    public void ShowSavePrompt()
    {
        if (savePromptUI == null)
        {
            return;
        }

        savePromptUI.SetActive(true);
        GlobalControl.Instance.isSaving = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSavePrompt()
    {
        if (savePromptUI != null)
            savePromptUI.SetActive(false);

        GlobalControl.Instance.isSaving = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
