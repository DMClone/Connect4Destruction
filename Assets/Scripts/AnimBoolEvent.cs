using UnityEngine;

public class AnimBoolEvent : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string boolName;

    public void SetBoolTrue()
    {
        animator.SetBool(boolName, true);
    }

    public void SetBoolFalse()
    {
        animator.SetBool(boolName, false);
    }
}
