using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessController : MonoBehaviour
{
    private PostProcessVolume pp;
    private Vignette ppVignette;
    private Color ppVignetteDefaultColor;
    private float ppVignetteDefaultIntensity;

    private int playerHealth;

    void Start()
    {
        pp = FindObjectOfType<PostProcessVolume>();
        ppVignette = pp.profile.GetSetting<Vignette>();
        ppVignetteDefaultColor = ppVignette.color;
        ppVignetteDefaultIntensity = ppVignette.intensity;
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerStats>().GetCurrentHealth();
        }

        OverridePP();
    }

    void OverridePP()
    {
        if (playerHealth > 50)
        {
            ppVignette.color.Override(ppVignetteDefaultColor);
            ppVignette.intensity.Override(ppVignetteDefaultIntensity);
        }
        else if (playerHealth < 49 && playerHealth > 20)
        {
            ppVignette.color.Override(Color.red);
            ppVignette.intensity.Override(0.3f);
        }
        else
        {
            ppVignette.color.Override(Color.red);
            ppVignette.intensity.Override(0.5f);
        }
    }
}
