using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DevotedTutorial : MonoBehaviour
{
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] GameObject tutorialCanvas;

    public bool inone = false;
    bool overone = false;

    public bool intwo = false;
    bool overtwo = false;

    public bool inthree = false;
    bool overthree = false;

    public bool infour = false;
    bool overfour = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("1") && !overone)
        {
            Prompt(1);
        }
        else if(other.CompareTag("2") && !overtwo)
        {
            Prompt(2);
        }
        else if(other.CompareTag("3") && !overthree)
        {
            Prompt(3);
        }
        else if(other.CompareTag("4") && !overfour)
        {
            Prompt(4);
        }
    }

    private void Prompt(int i)
    {
        tutorialCanvas.SetActive(true);

        switch(i)
        {
            case 1:
                tutorialText.text = "Move using A and D";

                inone = true;
            break;

            case 2:
                tutorialText.text = "Hold SPACE to jump over the rubble";

                intwo = true;
            break;

            case 3:
                tutorialText.text = "Press SHIFT destroy the rubble and save the citizens";

                inthree = true;
            break;
            
            case 4:
                tutorialText.text = "Press SHIFT to charge into the Shadows and kill them";

                infour = true;
            break;
        }

        TimeStop();
    }

    public void EndPrompt()
    {
        TimeStart();

        if(inone)
        {
            inone = false;
            overone = true;
        }
        else if(intwo)
        {
            intwo = false;
            overtwo = true;
        }
        else if(inthree)
        {
            inthree = false;
            overthree = true;
        }
        else if(infour)
        {
            infour = false;
            overfour = true;
        }

        tutorialCanvas.SetActive(false);
        tutorialText.text = "";
    }

    private void TimeStop()
    {
        PlayerInput.SetIsRecievingInput(false);

        Time.timeScale = 0f;
    }

    private void TimeStart()
    {
        PlayerInput.SetIsRecievingInput(true);

        Time.timeScale = 1f;
    }
}
