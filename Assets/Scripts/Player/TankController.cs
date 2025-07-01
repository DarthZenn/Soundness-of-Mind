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
    private float fireTimer = 0f;
    private int currentAmmo = 0;
    private int reserveAmmo = 0;
    private int maxAmmo = 0;

    [SerializeField] private float playerAttackRange;
    [SerializeField] private float playerFieldOfView;

    void Start()
    {
        anim = player.GetComponent<Animator>();
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

    /*    void HandleAnimation()
        {
            if (isAiming)
            {
                anim.Play("HandgunAim");
                return;
            }
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
        }*/

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

    /*void CheckAimingMode()
    {
        if (handgunHolder.childCount > 0)
        {
            Transform gun = handgunHolder.GetChild(0);
            ItemPickup pickup = gun.GetComponent<ItemPickup>();
            ItemData item = pickup != null ? pickup.item : null;

            if (item != null && item.itemType == ItemType.Handgun)
            {
                isAiming = Input.GetMouseButton(1);

                if (maxAmmo != item.maxAmmo)
                {
                    maxAmmo = item.maxAmmo;
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
                            Debug.Log("Click! Out of ammo, dummy.");
                        }
                    }
                }
            }
        }
        else
        {
            isAiming = false;
        }
    }*/

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
                            Debug.Log("Click! Out of ammo, dummy.");
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
        Debug.Log("Bang! Ammo left: " + currentAmmo);

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
                Debug.Log("Zombie hit! Gave it " + pickup.item.gunDamage + " damage.");
            }
        }
        else
        {
            Debug.Log("You missed. Maybe open your eyes next time, slug.");
        }
    }

    void Reload()
    {
        if (currentAmmo == maxAmmo)
        {
            Debug.Log("Magazine already full, dumbass.");
            return;
        }

        InventoryManager manager = FindObjectOfType<InventoryManager>();
        if (manager == null)
        {
            Debug.LogWarning("InventoryManager not found. You forgot to put it in the scene, didn't you?");
            return;
        }

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
            Debug.Log($"Reloaded {collected} bullets. Ammo now: {currentAmmo}/{maxAmmo}");
        }
        else
        {
            Debug.Log("No spare ammo to reload, loser.");
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
