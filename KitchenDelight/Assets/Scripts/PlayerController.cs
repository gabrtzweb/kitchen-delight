using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private string controlScheme = "Player1"; 
    [SerializeField] private Animator animator;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private PlayerControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        controls = new PlayerControls();
        controls.bindingMask = InputBinding.MaskByGroup(controlScheme);

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        
        rb.MovePosition(rb.position + moveDirection * (moveSpeed * Time.fixedDeltaTime));

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
            
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

        rb.angularVelocity = Vector3.zero; 
    }
}