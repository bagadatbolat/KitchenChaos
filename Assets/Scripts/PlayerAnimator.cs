using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController player;

    private void Update()
    {
        animator.SetBool(IsWalkingHash, player.IsWalking());
    }
}
