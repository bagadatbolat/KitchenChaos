using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(PlayerController player)
    {
        if (!player.HasKitchenObject())
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
    }
}
