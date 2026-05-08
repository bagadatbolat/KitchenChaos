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

    public IKitchenObjectHolder GetKitchenObjectParent() => kitchenObjectParent;

    public void SetKitchenObjectParent(IKitchenObjectHolder parent)
    {
        kitchenObjectParent?.ClearKitchenObject();
        kitchenObjectParent = parent;
        parent.SetKitchenObject(this);

        Transform followTransform = parent.GetKitchenObjectFollowTransform();
        transform.SetParent(followTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DestroySelf()
    {
        kitchenObjectParent?.ClearKitchenObject();
        Destroy(gameObject);
    }
}
