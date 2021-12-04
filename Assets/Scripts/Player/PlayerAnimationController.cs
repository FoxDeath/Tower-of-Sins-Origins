using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    #region  Attributes
    private Animator animator;
    #endregion

    #region  MonoBehaviour Methods
    private void Awake() 
    {
        animator = GetComponent<Animator>();    
    }
    #endregion

    #region  Normal Methods
    public void SetTrigger(string name)
    {
        ResetAllTriggers();

        animator.SetTrigger(name);
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger("Run");

        animator.ResetTrigger("Idle");

        animator.ResetTrigger("Fall");

        animator.ResetTrigger("Dash");

        animator.ResetTrigger("WallSlide");
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }

    public float GetFloat(string name)
    {
        return animator.GetFloat(name);
    }

    public void ResetTrigger(string name)
    {
        animator.ResetTrigger(name);
    }
    #endregion
}
