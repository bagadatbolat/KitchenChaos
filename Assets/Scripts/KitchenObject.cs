using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectHolder kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO so, IKitchenObjectHolder parent)
    {
        KitchenObject obj = Instantiate(so.prefab);
        obj.SetKitchenObjectParent(parent);
        return obj;
    }

    private void LateUpdate()
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
