using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerActions))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private string controlScheme = "Player1";
    [SerializeField] private Animator animator;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerActions actions;

    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");
    private static readonly int InteractHash = Animator.StringToHash("Interact");
    private static readonly int UseHash = Animator.StringToHash("Use");

    private bool hasIsMovingParameter;
    private bool hasIsDashingParameter;
    private bool hasInteractParameter;
    private bool hasUseParameter;

    private void Awake()
    {
        inputReader ??= GetComponent<PlayerInputReader>();
        movement ??= GetComponent<PlayerMovement>();
        actions ??= GetComponent<PlayerActions>();

        inputReader.ConfigureControlScheme(controlScheme);
        movement.Configure(moveSpeed, rotationSpeed);
        CacheAnimatorParameters();
    }

    private void Update()
    {
        movement.Tick(inputReader);
        actions.Tick(inputReader);
        UpdateAnimator();
        inputReader.ConsumeFrameFlags();
    }

    private void FixedUpdate()
    {
        movement.FixedTick(inputReader);
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        if (hasIsMovingParameter)
        {
            animator.SetBool(IsMovingHash, movement.CurrentPlanarSpeed > 0.05f);
        }

        if (hasIsDashingParameter)
        {
            animator.SetBool(IsDashingHash, movement.IsDashing);
        }

        if (inputReader.InteractPressedThisFrame && hasInteractParameter)
        {
            animator.SetTrigger(InteractHash);
        }

        if (inputReader.UsePressedThisFrame && hasUseParameter)
        {
            animator.SetTrigger(UseHash);
        }
    }

    private void CacheAnimatorParameters()
    {
        if (animator == null)
        {
            hasIsMovingParameter = false;
            hasInteractParameter = false;
            hasUseParameter = false;
            return;
        }

        hasIsMovingParameter = HasParameter(IsMovingHash, AnimatorControllerParameterType.Bool);
        hasIsDashingParameter = HasParameter(IsDashingHash, AnimatorControllerParameterType.Bool);
        hasInteractParameter = HasParameter(InteractHash, AnimatorControllerParameterType.Trigger);
        hasUseParameter = HasParameter(UseHash, AnimatorControllerParameterType.Trigger);
    }

    private bool HasParameter(int hash, AnimatorControllerParameterType type)
    {
        var parameters = animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            if (parameters[i].nameHash == hash && parameters[i].type == type)
            {
                return true;
            }
        }

        return false;
    }
}