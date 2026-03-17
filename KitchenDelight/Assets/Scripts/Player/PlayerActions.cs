using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private bool logActions;

    public void Tick(PlayerInputReader input)
    {
        if (input.InteractPressedThisFrame)
        {
            OnInteract();
        }

        if (input.UsePressedThisFrame)
        {
            OnUse();
        }
    }

    private void OnInteract()
    {
        if (logActions)
        {
            Debug.Log($"{name}: Interact");
        }
    }

    private void OnUse()
    {
        if (logActions)
        {
            Debug.Log($"{name}: Use");
        }
    }
}
