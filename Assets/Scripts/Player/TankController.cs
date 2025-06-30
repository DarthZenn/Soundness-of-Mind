using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject player;
    private Animator anim;

    public float walkSpeed;
    public float runSpeed;
    public float rotationSpeed;

    private float verticalInput;
    private float horizontalInput;

    private bool isRunning = false;
    private bool isMoving = false;

    void Start()
    {
        anim = player.GetComponent<Animator>();
    }

    void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;

        HandleMovement();
        HandleAnimation();
    }

    void HandleMovement()
    {
        isMoving = verticalInput != 0 || horizontalInput != 0;

        float moveSpeed = isRunning ? runSpeed : walkSpeed;
        float moveAmount = verticalInput * moveSpeed * Time.deltaTime;
        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;

        player.transform.Rotate(0, rotationAmount, 0);

        player.transform.Translate(0, 0, moveAmount);
    }

    void HandleAnimation()
    {
        if (!isMoving)
        {
            anim.Play("Idle");
        }
        else
        {
            if (verticalInput < 0)
            {
                anim.Play("WalkBackward");
            }
            else if (isRunning)
            {
                anim.Play("Run");
            }
            else
            {
                anim.Play("Walk");
            }
        }
    }
}
