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
        animator.SetTrigger("Break");

        Destroy(gameObject.GetComponent<BoxCollider2D>());

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
