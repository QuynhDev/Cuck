using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(CameraController))]
public class PlayerBehavioralController : MonoBehaviour
{
    public PlayerStats PlayerStats { get; private set; } 
    public CameraController CameraController { get; private set; }

    private PlayerMovementStateMachine playerMovementStateMachine;

    private void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
        CameraController = GetComponent<CameraController>(); 

        playerMovementStateMachine = new PlayerMovementStateMachine(this); 
    }
}
