using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
                player.GetKitchenObject().SetKitchenObjectParent(this);
            else
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        }
        else
        {
            if (player.HasKitchenObject())
                return;

            GetKitchenObject().SetKitchenObjectParent(player);
        }
    }
}
