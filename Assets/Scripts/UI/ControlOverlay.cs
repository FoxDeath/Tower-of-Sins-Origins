using UnityEngine;
using UnityEngine.UI;

public class ControlOverlay : MonoBehaviour
{ 
    #region Attributes
    [SerializeField] public Image attackImage;
    [SerializeField] public Image parryImage;
    [SerializeField] public Image jumpImage;
    [SerializeField] public Image dashImage;
    [SerializeField] public Image leftImage;
    [SerializeField] public Image rightImage;

    private Color transparent;
    #endregion

    #region MonoBehavior Methods
    private void Start()
    {
        TransparentStart();
    }

    private void Update()
    {
        LeftRightController();
    }
    #endregion

    #region Normal Methods
    //Changes the transparency depending of the state.
    public void SetImageState(bool isPressed, Image buttonImage)
    {
        transparent = buttonImage.color;
        
        if(isPressed)
        {
            transparent.a = 1f;

            buttonImage.color = transparent;
        }
        else
        {
            transparent.a = 0.5f;

            buttonImage.color = transparent;
        }
    }

    //Controlls the transparency of the left and right buttons.
    private void LeftRightController()
    {
        if(PlayerInput.GetMoveInput() < 0f)
        {
            //Left is pressed.
            transparent = leftImage.color;

            transparent.a = 1f;

            leftImage.color = transparent;
        }
        else if(PlayerInput.GetMoveInput() > 0f)
        {
            //Right is pressed.
            transparent = rightImage.color;

            transparent.a = 1f;

            rightImage.color = transparent;
        }
        else
        {
            //Left is not pressed.
            transparent = leftImage.color;

            transparent.a = 0.5f;

            leftImage.color = transparent;

            //Right is not pressed.
            transparent = rightImage.color;

            transparent.a = 0.5f;

            rightImage.color = transparent;
        }
    }

    //Makes all the images transparent from the start.
    private void TransparentStart()
    {
        transparent = attackImage.color;

        transparent.a = 0.5f;

        attackImage.color = transparent;

        transparent = parryImage.color;

        transparent.a = 0.5f;

        parryImage.color = transparent;

        transparent = jumpImage.color;

        transparent.a = 0.5f;

        jumpImage.color = transparent;

        transparent = dashImage.color;

        transparent.a = 0.5f;

        dashImage.color = transparent;

        transparent = leftImage.color;

        transparent.a = 0.5f;

        leftImage.color = transparent;

        transparent = rightImage.color;

        transparent.a = 0.5f;

        rightImage.color = transparent;
    }
    #endregion
}
