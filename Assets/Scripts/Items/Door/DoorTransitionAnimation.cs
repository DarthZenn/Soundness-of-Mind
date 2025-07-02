using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionAnimation : MonoBehaviour
{
    [SerializeField] AudioSource doorOpenSound;
    [SerializeField] AudioSource doorCreakSound;
    [SerializeField] AudioSource doorCreak1Sound;

    public void OpenDoor()
    {
        doorOpenSound.Play();
    }

    public void DoorCreak()
    {
        doorCreakSound.Play();
    }

    public void DoorCreak1()
    {
        doorCreak1Sound.Play();
    }
}
