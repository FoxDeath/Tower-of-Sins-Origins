using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GuideTutorial : MonoBehaviour
{
    [SerializeField] TMP_Text tutorialText;
    [SerializeField] GameObject tutorialCanvas;

    public bool inone = false;
    bool overone = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("1") && !overone)
        {
            Prompt(1);
        }
    }

    private void Prompt(int i)
    {
        tutorialCanvas.SetActive(true);

        switch(i)
        {
            case 1:
                tutorialText.text = "Press E to transport the City";

                inone = true;
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
