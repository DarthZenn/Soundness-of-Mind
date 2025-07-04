using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject zombie;
    private FadeScreen fadeScreen;
    private Animator playerAnimator;
    private Animator zombieAnimator;
    [SerializeField] private AudioSource buttonSound;

    void Start()
    {
        fadeScreen = GetComponent<FadeScreen>();
        playerAnimator = player.GetComponent<Animator>();
        zombieAnimator = zombie.GetComponent<Animator>();
    }

    void Update()
    {
        StartCoroutine(Dancing());
    }

    public void StartButton()
    {
        buttonSound.Play();
        GlobalControl.Instance.destinationSpawnID = "mainmenu_area0";
        StartCoroutine(WaitForFadeScreen());
    }

    public void SettingsButton()
    {
        buttonSound.Play();
    }

    public void ExitButton()
    {
        buttonSound.Play();
        Application.Quit();
    }

    IEnumerator WaitForFadeScreen()
    {
        StartCoroutine(fadeScreen.FadeScreenOut());
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(1);
    }

    IEnumerator Dancing()
    {
        yield return new WaitForSeconds(1);
        playerAnimator.SetTrigger("Dance");
        zombieAnimator.SetTrigger("Dance");
        player.transform.Rotate(0f, 50f * Time.deltaTime, 0f, Space.Self);
        zombie.transform.Rotate(0f, -50f * Time.deltaTime, 0f, Space.Self);
    }
}
