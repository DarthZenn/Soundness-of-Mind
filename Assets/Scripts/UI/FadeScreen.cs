using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private GameObject fadeScreen;
    private Animator fadeScreenAnimator;

    void Start()
    {
        fadeScreen.SetActive(false);
        fadeScreenAnimator = fadeScreen.GetComponent<Animator>();
        fadeScreenAnimator.enabled = false;

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            StartCoroutine(FadeScreenIn());
        }
    }

    public IEnumerator FadeScreenIn()
    {
        fadeScreen.SetActive(true);
        fadeScreenAnimator.enabled = true;
        fadeScreenAnimator.Play("FadeIn");
        yield return null;
    }

    public IEnumerator FadeScreenOut()
    {
        fadeScreen.SetActive(true);
        fadeScreenAnimator.enabled = true;
        fadeScreenAnimator.Play("FadeOut");
        yield return null;
    }
}
