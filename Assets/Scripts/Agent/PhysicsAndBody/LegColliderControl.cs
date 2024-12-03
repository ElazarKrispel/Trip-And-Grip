using UnityEngine;

public class LegColliderControl : MonoBehaviour
{
    public Collider[] legColliders;  // Array of leg colliders
    public Animator animator;        // Animator that controls the leg animations

    void Start()
    {
        // Ensure all leg colliders are disabled at the start of the game
        SetLegCollidersActive(false);
    }

    void Update()
    {
        // Check if the leg opening animation is off
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("anim_close") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("closed_Roll_Loop") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("anim_open_GoToRoll"))
        {
            SetLegCollidersActive(false);
        }
        else
        {
            SetLegCollidersActive(true);
        }
    }

    void SetLegCollidersActive(bool isActive)
    {
        foreach (Collider collider in legColliders)
        {
            collider.enabled = isActive;
        }
    }
}
