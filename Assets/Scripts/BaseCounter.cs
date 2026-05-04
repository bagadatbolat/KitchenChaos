using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IKitchenObjectHolder
{
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public abstract void Interact(PlayerController player);

    public virtual void InteractAlternate(PlayerController player) { }

    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;

    public bool HasKitchenObject() => kitchenObject != null;

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void SetKitchenObject(KitchenObject obj) => kitchenObject = obj;

    public void ClearKitchenObject() => kitchenObject = null;
}
