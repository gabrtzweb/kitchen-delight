using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupItem : MonoBehaviour
{
    [SerializeField] private float collisionRestoreDelay = 0.12f;

    private Rigidbody rb;
    private Collider[] itemColliders;

    private Collider[] ignoredPlayerColliders;
    private float restoreCollisionTimeRemaining;
    private Transform carryParent;

    private bool isCarried;

    public bool CanBePickedUp => !isCarried;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemColliders = GetComponentsInChildren<Collider>(true);
    }

    private void Update()
    {
        if (isCarried && carryParent != null)
        {
            transform.position = carryParent.position;
            transform.rotation = carryParent.rotation;
        }

        if (restoreCollisionTimeRemaining > 0f)
        {
            restoreCollisionTimeRemaining -= Time.deltaTime;
            if (restoreCollisionTimeRemaining <= 0f)
            {
                restoreCollisionTimeRemaining = 0f;
                SetCollisionIgnore(false);
            }
        }
    }

    public void PickUp(Transform parent, Collider[] playerColliders)
    {
        isCarried = true;
        carryParent = parent;
        ignoredPlayerColliders = playerColliders;

        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        rb.isKinematic = true;
        rb.detectCollisions = true;

        SetCollisionIgnore(true);
    }

    public void Drop(Vector3 releaseVelocity, Collider[] playerColliders)
    {
        transform.SetParent(null, true);

        isCarried = false;
        carryParent = null;
        ignoredPlayerColliders = playerColliders;

        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.linearVelocity = releaseVelocity;

        restoreCollisionTimeRemaining = collisionRestoreDelay;
    }

    public void Throw(Vector3 throwVelocity, Vector3 inheritedVelocity, Collider[] playerColliders)
    {
        Drop(inheritedVelocity, playerColliders);
        rb.linearVelocity += throwVelocity;
    }

    private void SetCollisionIgnore(bool ignore)
    {
        if (ignoredPlayerColliders == null || ignoredPlayerColliders.Length == 0)
        {
            return;
        }

        for (int i = 0; i < itemColliders.Length; i++)
        {
            Collider itemCollider = itemColliders[i];
            if (itemCollider == null)
            {
                continue;
            }

            for (int j = 0; j < ignoredPlayerColliders.Length; j++)
            {
                Collider playerCollider = ignoredPlayerColliders[j];
                if (playerCollider == null)
                {
                    continue;
                }

                Physics.IgnoreCollision(itemCollider, playerCollider, ignore);
            }
        }
    }
}
