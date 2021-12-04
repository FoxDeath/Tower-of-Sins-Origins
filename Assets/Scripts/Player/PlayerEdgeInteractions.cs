using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput))]
public class PlayerEdgeInteractions : MonoBehaviour
{
    #region Attributes
    private PlayerInput playerInput;

    private GameObject currentPlatform;
    private Vector3 currentTilePos;

    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    #endregion

    #region MonoBehaviour Methods
    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();    
    }
    #endregion

    #region  Normal Methods
    public void EdgeClimb()
    {
        PlayerInput.SetIsRecievingInput(false);
    }
    
    //Finishes the climbing by setting the posstition on the edge of the platform and then either goes idle or starts moving
    public void FinishClimb()
    {
        transform.SetParent(null);

        if(currentTilePos != Vector3.zero)
        {
            if(!PlayerState.GetIsFacingRight())
            {
                transform.position = new Vector2(currentTilePos.x - xOffset, currentTilePos.y + yOffset);
            }
            else
            {
                transform.position = new Vector2(currentTilePos.x + xOffset, currentTilePos.y + yOffset);
            }
        }
        else
        {
            if(!PlayerState.GetIsFacingRight())
            {
                transform.position = new Vector2(currentPlatform.GetComponent<Renderer>().bounds.max.x - xOffset, currentPlatform.GetComponent<Renderer>().bounds.max.y + yOffset - 3f);
            }
            else
            {
                transform.position = new Vector2(currentPlatform.GetComponent<Renderer>().bounds.min.x + xOffset, currentPlatform.GetComponent<Renderer>().bounds.max.y + yOffset - 3f);
            }
        }

        PlayerInput.SetIsRecievingInput(true);

        PlayerState.SetIsGrounded(true);

        playerInput.MovePerformedGameplay();
    }

    public void EdgeHang()
    {
        PlayerMovement.StopJumping(true);

        currentPlatform = PlayerPhysicsCalculations.hitHangDown.collider.gameObject;
        
        if(currentPlatform.GetComponent<Tilemap>())
        {
            currentTilePos = GetTilePos(currentPlatform.GetComponent<Tilemap>(), transform.position);
        }
        else
        {
            currentTilePos = Vector3.zero;
        }

        if(currentPlatform.tag.Equals("MovingPlatform"))
        {
            transform.SetParent(currentPlatform.transform);
        }

        playerInput.MoveInput();
    }

    private Vector3 GetTilePos(Tilemap tilemap,Vector3 pos)
    {
        pos = new Vector3(pos.x + (5f * PlayerMovement.facing), pos.y , pos.z);
        
        Vector3Int tilePos = tilemap.WorldToCell(pos);

        return tilemap.GetCellCenterWorld(tilePos);
    }
    #endregion=
}