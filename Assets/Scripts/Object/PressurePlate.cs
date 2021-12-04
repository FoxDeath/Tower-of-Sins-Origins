using System.Collections;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    #region Attribues
    [SerializeField] GameObject gateObject;
    [SerializeField] GameObject leverObject;

    private Animator animator;

    private Gate gate;
    private Lever lever;

    private Coroutine pressurePlateCoroutine;

    [SerializeField] float openTime;

    private bool canActivate;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        gate = gateObject.GetComponent<Gate>();
        
        animator = GetComponent<Animator>();

        if(leverObject)
        {
            lever = leverObject.GetComponent<Lever>();
        }
    }

    private void Start()
    {
        canActivate = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") || collider.CompareTag("Enemy") && gate.GetIsOpen())
        {
            if(canActivate)
            {
                gate.OpenGate();

                canActivate = false;
            }

            animator.SetBool("IsOn", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Player") || collider.CompareTag("Enemy") && gate.GetIsOpen())
        {
            if(pressurePlateCoroutine != null)
            {
                StopCoroutine(pressurePlateCoroutine);
            }

            pressurePlateCoroutine = StartCoroutine(PressurePlateBehaviour());
            
            animator.SetBool("IsOn", false);
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator PressurePlateBehaviour()
    {
        yield return new WaitForSeconds(openTime);

        if(leverObject)
        {
            if(!lever.GetLeverState() && gate.GetIsOpen())
            {
                gate.CloseGate();
            }
        }
        else if(gate.GetIsOpen())
        {
            gate.CloseGate();
        }

        canActivate = true;
    }
    #endregion
}
