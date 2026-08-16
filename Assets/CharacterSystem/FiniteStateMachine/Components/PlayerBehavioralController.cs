using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(CharacterController))]
public class PlayerBehavioralController : MonoBehaviour
{
    public PlayerStats PlayerStats { get; private set; } 
    public CameraController CameraController { get; private set; }
    public CharacterController CharacterController { get; private set; }

    private PlayerMovementStateMachine playerMovementStateMachine;

    private void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
        CameraController = GetComponent<CameraController>(); 
        CharacterController = GetComponent<CharacterController>(); 

        playerMovementStateMachine = new PlayerMovementStateMachine(this); 
    }
}
