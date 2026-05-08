using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObjectSO", menuName = "ScriptableObjects/KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public string objectName;
    public Sprite sprite;
    public KitchenObject prefab;
}
