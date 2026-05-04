using UnityEngine;

public class PlayerController : MonoBehaviour, IKitchenObjectHolder
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Start()
    {
        GameInput.Instance.OnInteract += Interact;
        GameInput.Instance.OnInteractAlternate += InteractAlternate;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnInteract -= Interact;
        GameInput.Instance.OnInteractAlternate -= InteractAlternate;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() => isWalking;

    private void HandleMovement()
    {
        Vector2 input = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;

        isWalking = moveDir != Vector3.zero;

        if (!isWalking)
            return;

        float moveDistance = moveSpeed * Time.deltaTime;
        Vector3 capsuleBottom = transform.position;
        Vector3 capsuleTop = transform.position + Vector3.up * playerHeight;

        bool canMove = !Physics.CapsuleCast(capsuleBottom, capsuleTop, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            Vector3 slideX = new Vector3(moveDir.x, 0f, 0f);
            Vector3 slideZ = new Vector3(0f, 0f, moveDir.z);

            if (CanSlide(capsuleBottom, capsuleTop, slideX, moveDistance))
            {
                moveDir = slideX.normalized;
                canMove = true;
            }
            else if (CanSlide(capsuleBottom, capsuleTop, slideZ, moveDistance))
            {
                moveDir = slideZ.normalized;
                canMove = true;
            }
        }

        if (canMove)
            transform.position += moveDir * moveDistance;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateSpeed * Time.deltaTime);
    }

    private bool CanSlide(Vector3 capsuleBottom, Vector3 capsuleTop, Vector3 dir, float distance)
    {
        if (dir == Vector3.zero) return false;
        return !Physics.CapsuleCast(capsuleBottom, capsuleTop, playerRadius, dir.normalized, distance);
    }

    private void HandleInteractions()
    {
        Vector2 input = GameInput.Instance.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;

        if (moveDir != Vector3.zero)
            lastInteractDir = moveDir;

        BaseCounter counter = null;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit hit, interactDistance, countersLayerMask))
            hit.transform.TryGetComponent(out counter);

        if (counter != selectedCounter)
            SetSelectedCounter(counter);
    }

    private void Interact()
    {
        selectedCounter?.Interact(this);
    }

    private void InteractAlternate()
    {
        selectedCounter?.InteractAlternate(this);
    }

    private void SetSelectedCounter(BaseCounter counter)
    {
        selectedCounter = counter;
    }

    // ── KitchenObject holding ──────────────────────────────────────────────

    public bool HasKitchenObject() => kitchenObject != null;

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void SetKitchenObject(KitchenObject obj) => kitchenObject = obj;

    public void ClearKitchenObject() => kitchenObject = null;

    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;
}
