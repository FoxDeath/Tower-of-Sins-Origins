using UnityEngine;
using TOS.Dialogue;

public class ObjectInteract : MonoBehaviour
{
    #region Attributes
    [SerializeField] InterObj interObj;

    private Lever lever;
    private AIConversant dialogue;
    private AudioManager audioManager;

    public enum InterObj
    {
        Page,
        Lever,
        NPC
    }
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();

        if(interObj == InterObj.Lever)
        {
            lever = GetComponent<Lever>();
        }

        if(interObj == InterObj.NPC)
        {
            dialogue = GetComponent<AIConversant>();
        }
    }
    #endregion

    #region Normal Methods
    private void DoInteraction()
    {
        switch(interObj)
        {
            case InterObj.Page:
                //To Do: Updrages the Player stats.
                
                audioManager.Play("Page");

                Destroy(gameObject);
            break;
                
            case InterObj.Lever:
                if(!lever.GetLeverState())
                {
                    lever.LeverOn();
                }
                else
                {
                    lever.LeverOff();
                }
            break;

            case InterObj.NPC:
                dialogue.RunDialogue();
            break;
        }
    }
    #endregion
}
