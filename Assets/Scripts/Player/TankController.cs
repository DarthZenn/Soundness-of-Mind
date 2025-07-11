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

    public Transform handgunHolder;
    private bool isAiming = false;
    [SerializeField] private AudioSource gunAudio;
    [SerializeField] private AudioSource gunEmptyAudio;
    [SerializeField] private AudioSource gunReloadAudio;
    [SerializeField] private AudioSource footStepAudio;
    private float fireTimer;
    private int currentAmmo;
    private int reserveAmmo;
    private int maxAmmo;

    [SerializeField] private float playerAttackRange;
    [SerializeField] private float playerFieldOfView;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        anim = player.GetComponent<Animator>();

        if (GlobalControl.Instance != null && GlobalControl.Instance.currentAmmo >= 0)
        {
            currentAmmo = GlobalControl.Instance.currentAmmo;
        }
        else
        {
            currentAmmo = maxAmmo;
        }
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
        CheckAimingMode();
        if (isAiming)
        {
            AutoAimNearestZombie();
        }

    }

    void HandleInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        isRunning = Input.GetKey(KeyCode.LeftShift) && verticalInput > 0;

    }

    void HandleMovement()
    {
        isMoving = verticalInput != 0 || horizontalInput != 0;

        float moveSpeed = isRunning ? runSpeed : walkSpeed;
        float moveAmount = verticalInput * moveSpeed * Time.deltaTime;
        float rotationAmount = horizontalInput * rotationSpeed * Time.deltaTime;

        player.transform.Rotate(0, rotationAmount, 0);

        if (!isAiming)
        {
            player.transform.Translate(0, 0, moveAmount);
        }
    }

    void HandleAnimation()
    {
        float speed = 0f;

        if (isAiming)
        {
            anim.SetBool("isAiming", true);
            anim.SetFloat("Speed", 0f);
            anim.SetBool("isWalkingBackward", false);
            return;
        }
        else
        {
            anim.SetBool("isAiming", false);
        }

        if (!isMoving)
        {
            speed = 0f;
            anim.SetBool("isWalkingBackward", false);
        }
        else
        {
            if (verticalInput < 0)
            {
                speed = walkSpeed;
                anim.SetBool("isWalkingBackward", true);
            }
            else
            {
                anim.SetBool("isWalkingBackward", false);
                speed = isRunning ? runSpeed : walkSpeed;
            }
        }

        anim.SetFloat("Speed", speed);
    }

    void CheckAimingMode()
    {
        bool holdingGun = false;

        if (handgunHolder.childCount > 0)
        {
            Transform gun = handgunHolder.GetChild(0);
            ItemPickup pickup = gun.GetComponent<ItemPickup>();
            ItemData item = pickup != null ? pickup.item : null;

            if (item != null && item.itemType == ItemType.Handgun)
            {
                holdingGun = true;
                isAiming = Input.GetMouseButton(1);

                if (maxAmmo != item.maxAmmo)
                {
                    maxAmmo = item.maxAmmo;

                    if (currentAmmo <= 0)
                        currentAmmo = maxAmmo;

                    reserveAmmo = CountAmmoInInventory();
                }


                if (Input.GetButtonDown("Reload") && isAiming)
                {
                    Reload();
                }

                if (isAiming)
                {
                    fireTimer -= Time.deltaTime;

                    if (Input.GetMouseButtonDown(0) && fireTimer <= 0f)
                    {
                        if (currentAmmo > 0)
                        {
                            fireTimer = item.fireRate;
                            FireGun();
                        }
                        else
                        {
                            fireTimer = item.fireRate;
                            gunEmptyAudio.Play();
                        }
                    }
                }
            }
        }

        anim.SetBool("hasHandgun", holdingGun);

        if (!holdingGun)
        {
            isAiming = false;
        }
    }

    void FireGun()
    {
        currentAmmo--;

        if (gunAudio != null)
            gunAudio.Play();

        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, playerAttackRange);
        float closestDistance = Mathf.Infinity;
        ZombieStats targetZombie = null;

        foreach (Collider hit in hitColliders)
        {
            ZombieStats zombie = hit.GetComponent<ZombieStats>();
            if (zombie != null && !zombie.IsDead())
            {
                Vector3 directionToZombie = (hit.transform.position - player.transform.position).normalized;
                float angleToZombie = Vector3.Angle(player.transform.forward, directionToZombie);

                if (angleToZombie <= playerFieldOfView / 2f)
                {
                    float distance = Vector3.Distance(player.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetZombie = zombie;
                    }
                }
            }
        }

        if (targetZombie != null)
        {
            Transform gun = handgunHolder.childCount > 0 ? handgunHolder.GetChild(0) : null;
            ItemPickup pickup = gun != null ? gun.GetComponent<ItemPickup>() : null;

            if (pickup != null && pickup.item != null)
            {
                targetZombie.TakeDamage(pickup.item.gunDamage);
            }
        }
    }

    void Reload()
    {
        if (currentAmmo == maxAmmo)
        {
            return;
        }

        InventoryManager manager = FindObjectOfType<InventoryManager>();
        if (manager == null)
        {
            return;
        }

        if (gunReloadAudio != null)
            gunReloadAudio.Play();

        int needed = maxAmmo - currentAmmo;
        int collected = 0;

        foreach (var slot in manager.slots)
        {
            ItemData data = slot.GetItem();
            if (data != null && data.itemType == ItemType.HandgunAmmo)
            {
                int available = slot.GetQuantity();

                if (available <= 0) continue;

                int take = Mathf.Min(needed - collected, available);

                slot.RemoveQuantity(take);
                collected += take;

                if (collected >= needed) break;
            }
        }

        if (collected > 0)
        {
            currentAmmo += collected;
            currentAmmo = Mathf.Clamp(currentAmmo, 0, maxAmmo);
        }
    }

    int CountAmmoInInventory()
    {
        int total = 0;
        InventoryManager manager = FindObjectOfType<InventoryManager>();
        if (manager == null) return 0;

        foreach (var slot in manager.slots)
        {
            ItemData data = slot.GetItem();
            if (data != null && data.itemType == ItemType.HandgunAmmo)
            {
                total += slot.GetQuantity();
            }
        }

        return total;
    }

    void AutoAimNearestZombie()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, playerAttackRange);
        float closestDistance = Mathf.Infinity;
        Transform closestZombie = null;

        foreach (Collider hit in hitColliders)
        {
            ZombieStats zombie = hit.GetComponent<ZombieStats>();
            if (zombie != null && !zombie.IsDead())
            {
                Vector3 dirToZombie = (hit.transform.position - player.transform.position).normalized;
                float angle = Vector3.Angle(player.transform.forward, dirToZombie);

                if (angle <= playerFieldOfView / 2f)
                {
                    float distance = Vector3.Distance(player.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestZombie = hit.transform;
                    }
                }
            }
        }

        if (closestZombie != null)
        {
            Vector3 targetDir = closestZombie.position - player.transform.position;
            targetDir.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public void PlayFootStepSound()
    {
        if (footStepAudio != null)
        {
            footStepAudio.Play();
        }
    }

    public int GetCurrentAmmo() => currentAmmo;

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(player.transform.position, playerAttackRange);

        Vector3 forward = player.transform.forward * playerAttackRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-playerFieldOfView / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(playerFieldOfView / 2, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(player.transform.position, leftRayDirection);
        Gizmos.DrawRay(player.transform.position, rightRayDirection);
    }
}
