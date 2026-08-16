using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterPhysics : MonoBehaviour
{
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -9.81f;
    public float MinGravity = -30;

    [Tooltip("The character's vertical speed will never go below this value")]
    public float TerminalVelocity = 53.0f;

    [Header("Grounded Check")]
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers = 1;

    [Header("Jump")]
    [Tooltip("The height the character can jump, in meters")]
    public float JumpHeight = 1.2f;

    public bool Grounded { get; private set; } = true;
    public float VerticalVelocity { get; private set; }

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGrounded();
        ApplyGravity();

        _controller.Move(new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);
    }

    /// <summary>
    /// Launches the character upward. Called by the movement FSM's jump state;
    /// the required take-off velocity is derived from JumpHeight and Gravity.
    /// </summary>
    public void Jump()
    {
        // v = sqrt(h * -2 * g)
        VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
    }

    private void CheckGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private void ApplyGravity()
    {
        if (Grounded && VerticalVelocity < 0.0f)
        {
            VerticalVelocity = -2f;
        }

        if (VerticalVelocity < TerminalVelocity)
        {
            VerticalVelocity += Gravity * Time.deltaTime;
            VerticalVelocity = Mathf.Max(VerticalVelocity, MinGravity); 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        Gizmos.color = Grounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }
}
