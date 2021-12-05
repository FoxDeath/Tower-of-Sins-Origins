using System.Collections;
using UnityEngine;
using TMPro;

public class ShitDialogue : MonoBehaviour
{
    [SerializeField] TMP_Text dialogueText;

    //Storyteller white, Naive blue, Eager green, Bored gray
    private void Dialogue(int i)
    {
        switch(i)
        {
            #region Prologue
            case 1:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("An endless blanket of darkness covers our chaotic lands."));
            break;

            case 2:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("The Cardinal Sins take hold of people, luring them into madness and transforming them, until they are nothing but mindless monsters, praying on their former human allies."));

                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour("I'll become a mighty warrior and slay all the monsters. You'll see!"));
            break;

            case 3:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Thankfully that won't be necessary. The Age of Chaos ended a long long time ago, thanks to three mighty spirits."));

                dialogueText.text = "";
                dialogueText.color = Color.green;
                StartCoroutine(TypewriterBehaviour("Those statues, there!"));

                dialogueText.text = "";
                dialogueText.color = Color.gray;
                StartCoroutine(TypewriterBehaviour("Ahh... For Height's sake! Not again!"));
            break;
            #endregion
            #region Myth 1
            case 4:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Those statues indeed. Except their real counterparts were far more... otherworldly."));

                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Out of the three, the Devoted One was the first to appear."));
            break;

            case 5:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("They used their remarkable strength to set us free from the shackles of temptation."));
            break;

            case 6:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("As if compelled by some ethereal selflessness, they came to aid and protect us in our time of need."));
            break;

            case 7:
                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour("Why did they help us?"));
            break;

            case 8:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Perhaps they saw something in us. Or they just pitied us, like we pity a bird with a broken wing."));
            break;

            case 9:
                dialogueText.text = "";
                dialogueText.color = Color.gray;
                StartCoroutine(TypewriterBehaviour("Nonetheless, they helped us. Let us move on!"));
            break;
            #endregion
            #region Myth 2
            case 10:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("The next arrival was the..."));

                dialogueText.text = "";
                dialogueText.color = Color.green;
                StartCoroutine(TypewriterBehaviour("...the Empowering One. Who helped humanity realise their own potential."));

                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Precisely, little one. It's a joy to recite these tales for such an eager audience."));
            break;

            case 11:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("The Empowering One gathered all the handiwork of humans and created countless weapons for the eradication of the chaos that plagued the lands."));
            break;

            case 12:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("They presented the humans with these weapons forged out of their own creations. Arming them for what was to come."));
            break;

            case 13:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("Being compelled by the Empowering One, the humans were ready to take the fight to the Cardinal Sins."));
            break;

            case 14:
                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour("Sooooo... they taught us how to fight?"));
            break;

            case 15:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour("They gave us a chance, rather. Illuminating a path out of the darkness."));
            break;
            #endregion
            #region Myth 3
            case 16:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;

            case 17:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;

            case 18:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;

            case 19:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;

            case 20:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;

            case 21:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(""));
            break;
            #endregion
            #region Epilogue
            #endregion
        }
    }

    private IEnumerator TypewriterBehaviour(string text)
        {
            foreach(char c in text)
            {
                dialogueText.text += c;

                yield return new WaitForSeconds(0.05f);
            }
        }
}
