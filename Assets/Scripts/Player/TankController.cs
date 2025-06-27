using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject player;
    public bool isMoving;
    public float horizontalMove;
    public float verticalMove;
    public bool isRunning;
    public bool backwardsCheck;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            isMoving = true;
            if (Input.GetButton("Backward"))
            {
                backwardsCheck = true;
                player.GetComponent<Animator>().Play("WalkBackward");
            }
            else
            {
                backwardsCheck = false;
                if (isRunning == false)
                {
                    player.GetComponent<Animator>().Play("Walk");
                }
                else
                {
                    player.GetComponent<Animator>().Play("Run");
                }
            }

            if (isRunning == false)
            {
                verticalMove = Input.GetAxis("Vertical") * Time.deltaTime * 1;
            }
            else if(isRunning == true && backwardsCheck == false)
            {
                verticalMove = Input.GetAxis("Vertical") * Time.deltaTime * 4;
            }

            horizontalMove = Input.GetAxis("Horizontal") * Time.deltaTime * 135;
            player.transform.Rotate(0, horizontalMove, 0);
            player.transform.Translate(0, 0, verticalMove);
        }
        else
        {
            isMoving = false;
            player.GetComponent<Animator>().Play("Idle");
        }
    }
}
