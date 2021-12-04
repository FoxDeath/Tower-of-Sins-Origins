using UnityEngine;

public class Gate : MonoBehaviour
{
    #region Attributes
    private BoxCollider2D boxCollider;
    private Animator animator;

    private AudioManager audioManager;

    private bool isOpening;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        
        animator = GetComponent<Animator>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        isOpening = false;
    }
    #endregion

    #region Normal Methods
    public void OpenGate()
    {
        isOpening = true;

        boxCollider.isTrigger = true;

        audioManager.Play("Gate");
            
        animator.SetBool("IsOn", isOpening);
    }

    public void CloseGate()
    {
        isOpening = false;

        boxCollider.isTrigger = false;

        audioManager.Play("Gate");
            
        animator.SetBool("IsOn", isOpening);
    }

    public bool GetIsOpen()
    {
        return isOpening;
    }
    #endregion
}
