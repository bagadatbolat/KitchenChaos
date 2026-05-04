using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    private IKitchenObjectHolder kitchenObjectParent;

    private void Update()
    {
        if (kitchenObjectParent != null)
            transform.position = kitchenObjectParent.GetKitchenObjectFollowTransform().position;
    }

    public IKitchenObjectHolder GetKitchenObjectParent() => kitchenObjectParent;

    public void SetKitchenObjectParent(IKitchenObjectHolder parent)
    {
        kitchenObjectParent?.ClearKitchenObject();
        kitchenObjectParent = parent;
        parent.SetKitchenObject(this);
        transform.parent = null;
    }

    public void DestroySelf()
    {
        kitchenObjectParent?.ClearKitchenObject();
        Destroy(gameObject);
    }
}
