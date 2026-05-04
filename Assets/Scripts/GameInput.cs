using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action OnInteract;
    public event Action OnInteractAlternate;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Interact.performed += _ => OnInteract?.Invoke();
        playerInputActions.Player.InteractAlternate.performed += _ => OnInteractAlternate?.Invoke();
        playerInputActions.Player.Enable();
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }

    /// <summary>Returns normalized movement input vector.</summary>
    public Vector2 GetMovementVectorNormalized()
    {
        return playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
}
