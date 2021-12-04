using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    #region Attributes
    //Each possible animator attribute is specified so there are no spelling errors and such.
    public enum triggers
    {
        Attack,
        Idle,
        DefaultMove,
        Charge,
        Jump,
        Death,
        Teleport,
        Fall,
        Levitate,
        Telegraph
    }

    public enum floats
    {
        AttackNumber
    }
    
    private Animator anim;

    private List<string> validParameters = new List<string>();
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        //All the existing attributes that the enemy's animator has is collected.
        for(int i = 0; i < anim.parameters.Length; i++)
        {
            validParameters.Add(anim.parameters[i].name);
        }  
    }
    #endregion

    #region Normal Methods
    public void SetTrigger(triggers name)
    {
        //If the enemy has such a parameter.
        if(validParameters.Contains(name.ToString()))
        {
            //It resets any other parameter.
            foreach(AnimatorControllerParameter parameter in anim.parameters)
            {
                if(parameter.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.ResetTrigger(parameter.name);
                }
            }

            //Then sets the one that was passed to the method.
            anim.SetTrigger(name.ToString());
        }
    }
    
    public void SetFloat(floats name, float nr)
    {
        //If the enemy's animator has such a parameter, it sets it.
        if(validParameters.Contains(name.ToString()))
        {
            anim.SetFloat(name.ToString(), nr);
        }
    }
    #endregion
}
