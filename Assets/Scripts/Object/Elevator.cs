using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    #region Attributes
    private Vector2 positionA;
    private Vector2 positionB;
    private Vector2 nextPosition;

    [SerializeField] Transform platform;
    [SerializeField] Transform destination;

    [SerializeField] float speed;

    private bool destinationReached;
    #endregion

    #region MonoBehavior Methods
    private void Start()
    {
        positionA = platform.localPosition;

        positionB = destination.localPosition;

        nextPosition = positionB;

        destinationReached = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            PlayerState.SetIsOnMovingPlatform(true);

            collision.gameObject.transform.SetParent(gameObject.transform);

            destinationReached = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(ElevatorBehaviour());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            PlayerState.SetIsOnMovingPlatform(false);

            collision.gameObject.transform.SetParent(null);
        }
    }
    #endregion

    #region Normal Methods
    private void ChangeDestination()
    {
        if(nextPosition != positionA)
        {
            nextPosition = positionA;
        }
        else
        {
            nextPosition = positionB;
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator ElevatorBehaviour()
    {
        if(!destinationReached)
        {
            platform.localPosition = Vector2.MoveTowards(platform.localPosition, nextPosition, speed * Time.deltaTime);
        }

        if(Vector3.Distance(platform.localPosition, nextPosition) <= 0.1f)
        {
            destinationReached = true;
            
            ChangeDestination();
        }

        yield return new WaitForSeconds(1f);
    }
    #endregion
}
