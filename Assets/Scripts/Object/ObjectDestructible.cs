using UnityEngine;

public class ObjectDestructible : MonoBehaviour
{
    #region Attributes
    [SerializeField] GameObject objectPrefab;

    private AudioManager audioManager;

    private Animator animator;

    [SerializeField] float timeToBreak;

    [SerializeField] bool shouldDestroy;
    private bool canSpawn;
    private bool destroyed = false;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        animator = GetComponent<Animator>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        if(objectPrefab)
        {
            canSpawn = true;
        }
        else
        {
            canSpawn = false;
        }
    }
    #endregion

    #region Normal Methods
    public void DestroyObject()
    {
        if(destroyed)
        {
            return;
        }

        destroyed = true;

        animator.SetTrigger("Break");

        audioManager.Play("BreakObject");

        if(shouldDestroy)
        {
            Destroy(gameObject, timeToBreak);
        }
        
        if(canSpawn)
        {
            Instantiate(objectPrefab, transform.position, transform.rotation);
        }
    }
    #endregion
}
