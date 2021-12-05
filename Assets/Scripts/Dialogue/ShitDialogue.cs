using System.Collections;
using UnityEngine;
using TMPro;

public class ShitDialogue : MonoBehaviour
{
    [SerializeField] TMP_Text dialogueText;

    //Storyteller white, Naive blue, Eager green, Bored red
    public void Dialogue(int i)
    {
        switch(i)
        {
            #region Prologue
            case 1:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "An endless blanket of darkness covers our chaotic lands."));
            break;

            case 2:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.blue, Color.white,
                    "The Cardinal Sins take hold of people, luring them into madness and transforming them to their liking.",
                    "They feed on humans until they are nothing but mindless monsters that prey on their former human allies.",
                    "I'll become a mighty warrior and slay all the monsters. You'll see!"));
            break;

            case 3:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.green, Color.red, Color.white,
                "Thankfully that won't be necessary. The Age of Chaos ended a long long time ago, thanks to three mighty spirits.",
                "Those statues, there!", 
                "Ahh... For Height's sake! Not again!"));
            break;
            #endregion
            #region Myth 1
            case 4:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Those statues indeed. Except their real counterparts were far more... otherworldly.",
                "Out of the three, the Devoted One was the first to appear."));
            break;

            case 5:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "They used their remarkable strength to set us free from the shackles of temptation."));
            break;

            case 6:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "As if compelled by some ethereal selflessness, they came to aid and protect us in our time of need."));
            break;

            case 7:
                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Why did they help us?"));
            break;

            case 8:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Perhaps they saw something in us. Or they just pitied us, like we pity a bird with a broken wing."));
            break;

            case 9:
                dialogueText.text = "";
                dialogueText.color = Color.red;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Nonetheless, they helped us. Let us move on!"));
            break;
            #endregion
            #region Myth 2
            case 10:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.green, Color.white, Color.white,
                "The next arrival was the...",
                "...the Empowering One. Who helped humanity realise their own potential.",
                "Precisely, little one. It's a joy to recite these tales for such an eager audience."));
            break;

            case 11:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "The Empowering One gathered all the handiwork of humans and formed weapons out of it.",
                "Weapons, created specifically for the eradication of the chaos that plagued the lands."));
            break;

            case 12:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "They presented the humans with these weapons forged out of their own creations. Arming them for what was to come."));
            break;

            case 13:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Being compelled by the Empowering One, the humans were ready to take the fight to the Cardinal Sins."));
            break;

            case 14:
                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Sooooo... they taught us how to fight?"));
            break;

            case 15:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "They gave us a chance, rather. Illuminating a path out of the darkness."));
            break;
            #endregion
            #region Myth 3
            case 16:
                dialogueText.text = "";
                dialogueText.color = Color.red;
                StartCoroutine(TypewriterBehaviour(Color.red, Color.white, Color.white,
                "And finally the focal point of the night.",
                "The Guide who led the charge against the forces of chaos, surrounded by the legions of determined humans.",
                "It seems, despite our best efforts to ignore them, some things just can’t cease to linger in our minds.",
                "Even if it's just a silly myth. Isn't that correct?"));
            break;

            case 17:
                dialogueText.text = "";
                dialogueText.color = Color.red;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "I warn you old man, don't you get on my nerves!",
                "You did play your part in the past, Firstborn, so I'll grant your wish."));
            break;

            case 18:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "The Guide provided us with an ideal. An image to live up to.",
                "That day on the battlefield, each human heart kindled a new flame, all burning with the same desire of becoming the masters of their own destiny."));
            break;

            case 19:
                dialogueText.text = "";
                dialogueText.color = Color.white;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "They introduced us to the power of virtue, bringing balance to our Breath and allowing us to end the era of Chaos."));
            break;
            #endregion
            #region Epilogue
            

            case 20:
                dialogueText.text = "";
                dialogueText.color = Color.blue;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Could people not breathe in the era of Chaos?",
                "Not that kind of breath, silly. I meant the source of our life force. The Breath, from which all life originates in our Universe."));
            break;

            case 21:
                dialogueText.text = "";
                dialogueText.color = Color.red;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Are you not gonna tell them about the Heights, old man?"));
            break;
            

            case 22:
                dialogueText.text = "";
                dialogueText.color = Color.green;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "At the moment of humanity's triumph over the darkness, the Heights emerged."));
            break;

            case 23:
                dialogueText.text = "";
                dialogueText.color = Color.green;
                StartCoroutine(TypewriterBehaviour(Color.white, Color.white, Color.white,
                "Now, the tower serves as a reminder of human potential, and a memory of our dark past.",
                "The Spirits would be proud of you, little one."));
            break;
            #endregion
        }
    }

    private IEnumerator TypewriterBehaviour(Color text2Color , Color text3Color, Color text4Color, string text, string text2 = "", string text3 = "", string text4 = "")
        {
            foreach(char c in text)
            {
                dialogueText.text += c;

                yield return new WaitForSeconds(0.05f);
            }
            
            yield return new WaitForSeconds(1f);

            dialogueText.text = "";

            if(!text2.Equals(""))
            {
                dialogueText.text = "";

                dialogueText.color = text2Color;
                
                foreach(char c in text2)
                {
                    dialogueText.text += c;

                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(1f);
            }
            
            if(!text3.Equals(""))
            {
                dialogueText.text = "";

                dialogueText.color = text3Color;

                foreach(char c in text3)
                {
                    dialogueText.text += c;

                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(1f);
            }
            
            if(!text4.Equals(""))
            {
                dialogueText.text = "";

                dialogueText.color = text4Color;

                foreach(char c in text4)
                {
                    dialogueText.text += c;

                    yield return new WaitForSeconds(0.05f);
                }

                yield return new WaitForSeconds(1f);
            }
        }
}
