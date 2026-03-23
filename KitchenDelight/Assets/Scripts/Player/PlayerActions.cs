using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Carry")]
    [SerializeField] private Transform holdPoint;
    [SerializeField, Min(0.2f)] private float pickupRadius = 1.4f;
    [SerializeField] private LayerMask pickupLayers = ~0;

    [Header("Throw")]
    [SerializeField, Min(0.1f)] private float throwForce = 8f;
    [SerializeField, Min(0f)] private float throwUpwardForce = 1.2f;

    [SerializeField] private bool logActions;

    private PickupItem carriedItem;
    private PlayerMovement movement;
    private Rigidbody playerRigidbody;
    private Collider[] playerColliders;

    public bool IsCarrying => carriedItem != null;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerColliders = GetComponentsInChildren<Collider>(true);

        if (holdPoint == null)
        {
            var holdAnchor = new GameObject("HoldPoint");
            holdAnchor.transform.SetParent(transform, false);
            holdAnchor.transform.localPosition = new Vector3(0f, 1f, 0.8f);
            holdPoint = holdAnchor.transform;
        }
    }

    public void Tick(PlayerInputReader input)
    {
        if (input.InteractPressedThisFrame)
        {
            OnInteract(input);
        }

        if (input.ThrowPressedThisFrame)
        {
            OnThrow();
        }

        if (input.UsePressedThisFrame)
        {
            OnUse();
        }
    }

    private void OnInteract(PlayerInputReader input)
    {
        if (carriedItem != null)
        {
            DropCarried(input);
            return;
        }

        TryPickupNearest();

        if (logActions)
        {
            Debug.Log($"{name}: Interact");
        }
    }

    private void OnThrow()
    {
        if (carriedItem == null)
        {
            return;
        }

        Vector3 facing = movement != null ? movement.FacingDirection : transform.forward;
        facing.y = 0f;
        if (facing.sqrMagnitude < 0.0001f)
        {
            facing = transform.forward;
            facing.y = 0f;
        }
        facing.Normalize();

        Vector3 inheritedVelocity = playerRigidbody != null ? playerRigidbody.linearVelocity : Vector3.zero;
        Vector3 throwVelocity = facing * throwForce + Vector3.up * throwUpwardForce;
        carriedItem.Throw(throwVelocity, inheritedVelocity, playerColliders);
        carriedItem = null;
    }

    private void OnUse()
    {
        if (logActions)
        {
            Debug.Log($"{name}: Use");
        }
    }

    private void TryPickupNearest()
    {
        Collider[] overlaps = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayers, QueryTriggerInteraction.Collide);
        PickupItem bestItem = null;
        float bestDistanceSqr = float.MaxValue;

        for (int i = 0; i < overlaps.Length; i++)
        {
            PickupItem item = overlaps[i].GetComponentInParent<PickupItem>();
            if (item == null)
            {
                continue;
            }

            if (!item.CanBePickedUp)
            {
                continue;
            }

            Vector3 delta = item.transform.position - transform.position;
            float distanceSqr = delta.sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestItem = item;
            }
        }

        if (bestItem == null)
        {
            return;
        }

        carriedItem = bestItem;
        carriedItem.PickUp(holdPoint, playerColliders);
    }

    private void DropCarried(PlayerInputReader input)
    {
        Vector3 dropVelocity = playerRigidbody != null ? playerRigidbody.linearVelocity : Vector3.zero;
        Vector3 moveInfluence = new Vector3(input.Move.x, 0f, input.Move.y) * 1.5f;
        carriedItem.Drop(dropVelocity + moveInfluence, playerColliders);
        carriedItem = null;
    }
}
