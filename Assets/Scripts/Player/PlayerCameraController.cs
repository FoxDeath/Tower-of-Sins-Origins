using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    #region Attributes
    [SerializeField] Transform cameraFollow;

    private Camera mainCamera;

    [SerializeField] float clampX = 5f;
    [SerializeField] float clampY = 5f;
    private float cameraPositionOffsetX;
    private float cameraPositionOffsetY;

    private int lookX;
    private int lookY;

    private bool cameraReseting;
    #endregion

    #region MonoBehaviour Methods
    private void Awake() 
    {
        mainCamera = FindObjectOfType<Camera>();    
    }

    private void FixedUpdate()
    {
        if(PlayerState.GetState() != PlayerState.State.Idle)
        {
            return;
        }

        CameraPositionOffsetCalculations();

        cameraFollow.localPosition = new Vector2(Mathf.Clamp(cameraFollow.localPosition.x, -clampX + cameraPositionOffsetX, clampX - cameraPositionOffsetX)
        , Mathf.Clamp(cameraFollow.localPosition.y, -clampY - cameraPositionOffsetY, clampY + cameraPositionOffsetY));

        MoveLookToStart();

        MoveLook();
    }
    #endregion

    #region Normal Methods

    //Calcucaltes the extra distance the cameraFollow needs to move based on the possition of the camera and the cameraFollow
    private void CameraPositionOffsetCalculations()
    {
        if (cameraFollow.localPosition == Vector3.zero)
        {
            PlayerState.SetIsLookingAround(false);

            cameraPositionOffsetX = (mainCamera.transform.position.x - cameraFollow.position.x) / 4f;
            
            cameraPositionOffsetY = (mainCamera.transform.position.y - cameraFollow.position.y) / 4f;
        }
        else
        {
            PlayerState.SetIsLookingAround(true);
        }
    }

    //Moves the cameraFollow object back to Vector3.zero
    private void MoveLookToStart()
    {
        if(cameraReseting)
        {
            cameraFollow.localPosition = Vector3.MoveTowards(cameraFollow.localPosition, Vector3.zero, 1f);

            if(cameraFollow.localPosition.Equals(Vector3.zero))
            {
                cameraReseting = false;
            }

            return;
        }
    }

    //Moves the cameraFollow object coresponding to the looxX and lookY
    private void MoveLook()
    {
        if(lookX == 1)
        {
            cameraFollow.localPosition += Vector3.right;
        }
        else if(lookX == -1)
        {
            cameraFollow.localPosition += Vector3.left;
        }

        if(lookY == 1)
        {
            cameraFollow.localPosition += Vector3.up;
        }
        else if(lookY == -1)
        {
            cameraFollow.localPosition += Vector3.down;
        }
    }
    
    //Sets the lookY and lookX in order to know where to move
    public void SetLook(Vector2 vectorValue)
    {
        ResetLook();

        if(Mathf.Abs(vectorValue.x) > Mathf.Abs(vectorValue.y))
        {
            lookY = 0;

            if(vectorValue.x > 0f)
            {
                lookX = 1;
            }
            else if(vectorValue.x < 0f)
            {
                lookX = -1;
            }
        }
        else if(Mathf.Abs(vectorValue.x) < Mathf.Abs(vectorValue.y))
        {
            lookX = 0;

            if(vectorValue.y > 0f)
            {
                lookY = 1;
            }
            else if(vectorValue.y < 0f)
            {
                lookY = -1;
            }
        }
    }

    //Resets lookY and lookX everytime the imput is called and then the cameraFollow object goes to Vector3.zero
    private void ResetLook()
    {
        lookY = 0;

        lookX = 0;

        cameraReseting = true;
    }
    #endregion
}
