using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject doorPivot;
    [SerializeField] GameObject door;
    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource doorClose;
    private BoxCollider doorCollider;
    private BoxCollider doorPivotCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        doorCollider = door.GetComponent<BoxCollider>();
        doorPivotCollider = doorPivot.GetComponent<BoxCollider>();
    }

    public void DoorOpen()
    {
        doorOpen.Play();
    }

    public void DoorClose()
    {
        doorClose.Play();
    }

    public void DisableDoorCollision()
    {
        doorPivotCollider.enabled = false;
        doorCollider.enabled = false;
        animator.SetBool("isClosed", false);
    }

    public void EnableDoorCollision()
    {
        doorPivotCollider.enabled = true;
        doorCollider.enabled = true;
        animator.SetBool("isClosed", true);
    }
}
