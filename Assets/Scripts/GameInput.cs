using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action OnInteract;
    public event Action OnInteractAlternate;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb.eKey.wasPressedThisFrame) OnInteract?.Invoke();
        if (kb.fKey.wasPressedThisFrame) OnInteractAlternate?.Invoke();
    }

    /// <summary>Returns normalized movement input vector.</summary>
    public Vector2 GetMovementVectorNormalized()
    {
        var kb = Keyboard.current;
        Vector2 v = Vector2.zero;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v.y -= 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  v.x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) v.x += 1f;
        return Vector2.ClampMagnitude(v, 1f);
    }
}
