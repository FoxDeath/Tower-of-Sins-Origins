using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A placeholder class that makes the eye move depending on where the player holds the up and down button
public class PlayerEyeController : MonoBehaviour
{
    [SerializeField] Transform EyeUp;
    [SerializeField] Transform EyeDown;
    
    private Vector2 originalPossition;

    private void Start()
    {
        originalPossition = transform.localPosition;
    }

    private void Update()
    {
        if(PlayerInput.GetAttackInput() > 0f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, EyeUp.localPosition, 0.1f);
        }
        else if(PlayerInput.GetAttackInput() < 0f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, EyeDown.localPosition, 0.1f);

        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, originalPossition, 0.1f);
        }
    }
}
