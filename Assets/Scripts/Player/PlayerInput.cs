using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerWallInteraction))]
[RequireComponent(typeof(PauseMenuController))]
[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(PlayerInteract))]
[RequireComponent(typeof(Teleport))]

public class PlayerInput : MonoBehaviour
{
    #region Attributes

    static private InputActionAsset inputActions;

    public PlayerControls controls;

    private PlayerMovement playerMovement;

    private PlayerCombat playerCombat;

    private PlayerWallInteraction playerWallInteraction;

    private PlayerInteract playerInteract;

    private PauseMenuController pauseMenuController;
    
    private PlayerCameraController playerCameraController;

    private Teleport playerPortalController;

    private ControlOverlay controlOverlay;

    private static float moveInput;
    private static float attackInput;

    private static bool isRecievingInput = true;

    #endregion
    
    #region MonoBehaviour Methods
    private void Awake() 
    {
        controls = new PlayerControls() ;

        playerMovement = GetComponent<PlayerMovement>();

        playerCombat = GetComponent<PlayerCombat>();

        playerWallInteraction = GetComponent<PlayerWallInteraction>();
        
        playerInteract = GetComponent<PlayerInteract>();

        pauseMenuController = GetComponentInChildren<PauseMenuController>();

        playerCameraController = GetComponent<PlayerCameraController>();

        inputActions = GetComponent<UnityEngine.InputSystem.PlayerInput>().actions;

        controlOverlay = FindObjectOfType<ControlOverlay>();

        playerPortalController = GetComponent<Teleport>();
    }

    private void Start()
    {
        controls.Enable();
    }
    #endregion

    #region  Normal Methods
    public void AttackPerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            if(GetComponentInChildren<EmpoweringOneTutorial>())
        if(context.action.phase == InputActionPhase.Started && (GetComponentInChildren<EmpoweringOneTutorial>().inone || GetComponentInChildren<EmpoweringOneTutorial>().intwo))
            {
                GetComponentInChildren<EmpoweringOneTutorial>().EndPrompt();
                
                moveInput = 0;

                AttackPerformed(context);
            }

            return;
        }

        if(context.action.phase == InputActionPhase.Started)
        {
            PlayerState.SetState(PlayerState.State.Attacking);

            controlOverlay.SetImageState(true, controlOverlay.attackImage);
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            controlOverlay.SetImageState(false, controlOverlay.attackImage);
        }
    }

    public void ParryPerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            return;
        }

        if(context.action.phase == InputActionPhase.Started)
        {
            PlayerState.SetState(PlayerState.State.Parrying);

            controlOverlay.SetImageState(true, controlOverlay.parryImage);
        }
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            controlOverlay.SetImageState(false, controlOverlay.parryImage);
        }
    }

    public void SpecialAbilityPerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            return;
        }

        if(context.action.phase == InputActionPhase.Started)
        {
            PlayerState.SetState(PlayerState.State.Casting);
        }
    }

    public void JumpPerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            if(GetComponentInChildren<DevotedTutorial>())
        if(context.action.phase == InputActionPhase.Started && GetComponentInChildren<DevotedTutorial>().intwo)
            {
                GetComponentInChildren<DevotedTutorial>().EndPrompt();

            JumpPerformed(context);
            }


            return;
        }

        if(PlayerState.GetState() != PlayerState.State.Dead)
        {
            if(context.action.phase == InputActionPhase.Started)
            {
                controlOverlay.SetImageState(true, controlOverlay.jumpImage);

                if(PlayerState.GetIsGrounded())
                {
                    PlayerState.SetState(PlayerState.State.Jumping);                
                }
                else if(PlayerState.GetState() == PlayerState.State.WallSliding)
                {
                    PlayerState.SetState(PlayerState.State.WallJumping);
                }
            }
            else if(context.action.phase == InputActionPhase.Canceled)
            {
                controlOverlay.SetImageState(false, controlOverlay.jumpImage);

                if(PlayerState.GetState() == PlayerState.State.Jumping)
                {
                    playerMovement.JumpRelease();
                }
            }
        }
    }

    public void MovePerformed(InputAction.CallbackContext context)
    {
        if(PlayerState.GetIsWallStuck() || PlayerState.GetState() == PlayerState.State.Dead
        || PlayerState.GetState() == PlayerState.State.Sliding || PlayerState.GetState() == PlayerState.State.Staggered
        || !isRecievingInput)
        {
        
        if(GetComponentInChildren<DevotedTutorial>())
        if(context.ReadValue<Vector2>().x != 0 && GetComponentInChildren<DevotedTutorial>().inone)
            {
                GetComponentInChildren<DevotedTutorial>().EndPrompt();
            }

            if(GetComponentInChildren<EmpoweringOneTutorial>())
        if(context.ReadValue<Vector2>().x != 0 && (GetComponentInChildren<EmpoweringOneTutorial>().inthree || GetComponentInChildren<EmpoweringOneTutorial>().infour))
            {
                GetComponentInChildren<EmpoweringOneTutorial>().EndPrompt();
            }

            return;
        }

        Vector2 vectorValue = context.ReadValue<Vector2>();

        

        moveInput = vectorValue.x;

        attackInput = vectorValue.y;

        MoveInput();
    }

    public void MovePerformedGameplay()
    {
        if(PlayerState.GetIsWallStuck() || PlayerState.GetState() == PlayerState.State.Dead
        || !isRecievingInput)
        {
            return;
        }
        
        Vector2 vectorValue = controls.Gameplay.Move.ReadValue<Vector2>();

        moveInput = vectorValue.x;

        attackInput = vectorValue.y;
        
        MoveInput();
    }

    public void MoveInput()
    {
        if(!isRecievingInput)
        {
            return;
        }

        if(PlayerState.GetState() == PlayerState.State.Hanging && (int)(PlayerPhysicsCalculations.hitHangDown.normal.x + controls.Gameplay.Move.ReadValue<Vector2>().x) == 0)
        {
            PlayerState.SetState(PlayerState.State.Climbing);
        }
        else if(PlayerState.GetState() == PlayerState.State.Hanging && (int)(PlayerPhysicsCalculations.hitHangDown.normal.x + controls.Gameplay.Move.ReadValue<Vector2>().x) != 0)
        {
            return;
        }

        if(PlayerState.GetState() == PlayerState.State.Attacking || PlayerState.GetState() == PlayerState.State.Parrying)
        {
            return;
        }

        if((Math.Abs(moveInput) > 0.01f))
        {
            PlayerState.SetState(PlayerState.State.Moving);
        }
        else
        {
            PlayerState.SetState(PlayerState.State.Idle);
        }
    }

    public void LookPerformed(InputAction.CallbackContext context)
    {
        if(PlayerState.GetIsWallStuck() || PlayerState.GetState() == PlayerState.State.Dead
        || PlayerState.GetState() == PlayerState.State.Sliding || PlayerState.GetState() == PlayerState.State.Staggered
        || !isRecievingInput)
        {
            return;
        }

        Vector2 vectorValue = context.ReadValue<Vector2>();

        playerCameraController.SetLook(vectorValue);
    }

    public void SlidePerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            if(GetComponentInChildren<DevotedTutorial>())
        if(context.action.phase == InputActionPhase.Started && (GetComponentInChildren<DevotedTutorial>().inthree || GetComponentInChildren<DevotedTutorial>().infour))
            {
                GetComponentInChildren<DevotedTutorial>().EndPrompt();
            }
            return;
        }

        controlOverlay.SetImageState(true, controlOverlay.dashImage);

        if(PlayerState.GetIsGrounded())
        {
            PlayerState.SetState(PlayerState.State.Sliding);
        }
        else
        {
            PlayerState.SetState(PlayerState.State.Dashing);
        }
    }

    public void HubPortPerformed(InputAction.CallbackContext context)
    {
        PlayerState.SetState(PlayerState.State.Teleporting);
    }

    public void InteractPerformed(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            if(GetComponentInChildren<GuideTutorial>())
        if(context.action.phase == InputActionPhase.Started && GetComponentInChildren<GuideTutorial>().inone)
            {
                GetComponentInChildren<GuideTutorial>().EndPrompt();
                
                moveInput = 0;

                InteractPerformed(context);
            }
            return;
        }

        if(PlayerState.GetState() != PlayerState.State.Dead && PlayerState.GetIsGrounded() && context.action.phase == InputActionPhase.Started)
        {
            playerInteract.Interact();
        }
    }

    public void InteractPerformedGameplay()
    {
        if(!isRecievingInput)
        {
            return;
        }

        if(PlayerState.GetState() != PlayerState.State.Dead && PlayerState.GetIsGrounded() && controls.Gameplay.Interact.triggered)
        {
            playerInteract.Interact();
        }
    }

    public void PauseInput(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void RestartScene(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void DeathCheckInput(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started && PlayerState.GetState() == PlayerState.State.Dead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void WalkToggleInput(InputAction.CallbackContext context)
    {
        if(!isRecievingInput)
        {
            return;
        }

        if(context.action.phase == InputActionPhase.Started)
        {
            PlayerState.SetIsWalking(!PlayerState.GetIsWalking());
        }
    }
    #endregion

    #region  Getters
    public static float GetMoveInput()
    {
        if(Mathf.Abs(moveInput) > 0.01f)
        {
            return moveInput;
        }
        else
        {
            return 0f;
        }
    }
    
    public static float GetAttackInput()
    {
        return attackInput;
    }

    public static InputActionAsset GetInputActions()
    {
        return inputActions;
    }

    public static void SetIsRecievingInput(bool state)
    {
        if(isRecievingInput != state)
        {
            isRecievingInput = state;
        }
    }
    #endregion

    #region  Setters
    public static void SetMoveInput(float moveInput)
    {
        PlayerInput.moveInput = moveInput;
    }
    #endregion
}
