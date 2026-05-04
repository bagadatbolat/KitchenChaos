using UnityEngine;

public interface IKitchenObjectHolder
{
    Transform GetKitchenObjectFollowTransform();
    KitchenObject GetKitchenObject();
    void SetKitchenObject(KitchenObject obj);
    void ClearKitchenObject();
    bool HasKitchenObject();
}
