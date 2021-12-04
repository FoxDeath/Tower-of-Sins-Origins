using System.Collections;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    #region Attributes
    private GameObject currentObject = null;

    private PlayerRecovery playerRecovery;
    private AudioManager audioManager;

    private bool canPlay;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        playerRecovery = GetComponent<PlayerRecovery>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        canPlay = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("InteractableObject"))
        {
            currentObject = other.gameObject;
        }

        if(other.CompareTag("EnvironmentalDanger"))
        {
            playerRecovery.Recover();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("InteractableObject"))
        {
            if(other.gameObject == currentObject)
            {
                currentObject = null;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("CrumblingPlatform"))
        {
            if(!PlayerState.GetIsWalking() || PlayerState.GetState() == PlayerState.State.Jumping
            || PlayerState.GetState() == PlayerState.State.Sliding || PlayerState.GetIsLanded())
            {
                if(canPlay)
                {
                    canPlay = false;

                    StartCoroutine(CrumblingPlatformBehaviour());
                }
                
                Destroy(other.gameObject, 0.5f);
            }
        }
    }
    #endregion

    #region Normal Methods
    public void Interact()
    {
        if(currentObject)
        {
            currentObject.SendMessage("DoInteraction");
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator CrumblingPlatformBehaviour()
    {
        audioManager.Play("CrumblingPlatform");

        yield return new WaitForSeconds(1f);

        canPlay = true;
    }
    #endregion
}
