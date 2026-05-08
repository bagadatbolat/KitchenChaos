using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] selectedVisualArray;
    [SerializeField] private PlayerController player;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
        player.OnSelectedCounterChanged += OnSelectedCounterChanged;
    }

    private void OnDestroy()
    {
        player.OnSelectedCounterChanged -= OnSelectedCounterChanged;
    }

    private void OnSelectedCounterChanged(BaseCounter selectedCounter)
    {
        foreach(var visual in selectedVisualArray)
            visual.SetActive(selectedCounter == counter);
    }
}
