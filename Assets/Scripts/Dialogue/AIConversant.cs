using UnityEngine;
using System.Linq;
using System.Collections;
using TMPro;

namespace TOS.Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        #region Attributes
        [SerializeField] Dialogue dialogue;

        private Dialogue currentDialogue;

        private DialogueNode currentNode = null;

        private DialogueNode[] children;

        private AudioManager audioManager;

        private TextMeshProUGUI dialogueText;

        private bool isDialoguing = false;

        private bool isTyping = false;
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();

            audioManager = FindObjectOfType<AudioManager>();
        }
        #endregion

        #region Normal Methods
        public void RunDialogue()
        {
            if(PlayerState.GetState() != PlayerState.State.Attacking && PlayerState.GetState() != PlayerState.State.Parrying
            && PlayerState.GetState() != PlayerState.State.Jumping && PlayerState.GetState() != PlayerState.State.Falling)
            {
                if(!isTyping)
                {
                    if(!isDialoguing)
                    {
                        isDialoguing = true;

                        StartDialogue(dialogue);
                    }

                    UpdateUI();

                    if(HasNext())
                    {
                        Next();
                    }
                    else
                    {
                        isDialoguing = false;

                        QuitDialogue();

                        dialogueText.text = "";

                        currentDialogue = dialogue;
                    }
                }
            }
        }

        private void StartDialogue(Dialogue newDialogue)
        {
            PlayerInput.GetInputActions().Disable();

            PlayerState.SetState(PlayerState.State.Dialoguing);

            currentDialogue = newDialogue;

            currentNode = currentDialogue.GetRootNode();
        }

        private bool IsActive()
        {
            return currentDialogue != null;
        }

        //Puts the children of the node in an array, which allows us to read the text they have.
        private void Next()
        {
            children = currentDialogue.GetAllChildren(currentNode).ToArray();

            int randomIndex = Random.Range(0, children.Count());

            currentNode = children[randomIndex];
        }

        //Checks if the dialogue node has anymore children.
        private bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        private void QuitDialogue()
        {
            currentDialogue = null;

            currentNode = null;

            PlayerState.SetState(PlayerState.State.Idle);

            PlayerInput.GetInputActions().Enable();
        }

        //Updates the UI text.
        private void UpdateUI()
        {
            dialogueText.text = "";

            if(!isTyping)
            {
                isTyping = true;
                
                StartCoroutine(SoundBehaviour());

                StartCoroutine(TypewriterBehaviour());
            }
        }
        #endregion

        #region Coroutines
        private IEnumerator TypewriterBehaviour()
        {
            foreach(char c in currentNode.GetText())
            {
                dialogueText.text += c;

                yield return new WaitForSeconds(0.005f);
            }

            isTyping = false;
        }

        private IEnumerator SoundBehaviour()
        {
            while(isTyping)
            {
                audioManager.Play("Dialogue");

                yield return new WaitForSeconds(0.05f);
            }

            audioManager.Stop("Dialogue");
        }
        #endregion
    }
}
