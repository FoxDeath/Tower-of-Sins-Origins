using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region Attributes
    [SerializeField] Transform[] points;
    private Transform target;

    [SerializeField] float speed = 5f;

    private int currentPoint;

    private bool forward;

    private enum types
    {
        Circle,
        Linear
    }

    [SerializeField] types currentType;
    #endregion

    #region MonoBehaviour Methods
    private void Start()
    {
        target = points[0];

        forward = true;

        currentPoint = 0;
    }
    private void Update()
    {
        if(points.Length != 0)
        {
            if(Vector2.Distance(transform.position, target.position) <= 0.1f)
            {
                switch(currentType)
                {
                    case types.Circle:
                        CircleBehaviour();
                    break;

                    case types.Linear:
                        if(forward)
                        {
                            LinearForwardBehaviour();
                        }
                        else
                        {
                            LinearBackwardsBehaviour();
                        }
                    break;
                }
            }

           transform.position = Vector2.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Player") && collision.collider.GetType() == typeof(PolygonCollider2D))
        {
            PlayerState.SetIsOnMovingPlatform(true);

            collision.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag.Equals("Player") && collision.collider.GetType() == typeof(PolygonCollider2D))
        {
            PlayerState.SetIsOnMovingPlatform(false);

            collision.gameObject.transform.SetParent(null);
        }
    }
    #endregion

    #region Normal Methods
    private void CircleBehaviour()
    {
        if(currentPoint + 1 < points.Length)
        {
            currentPoint += 1;
        }
        else
        {
            currentPoint = 0;
        }

        target = points[currentPoint];
    }

    private void LinearForwardBehaviour()
    {
        if(!forward)
        {
            forward = !forward;
        }

        if(currentPoint + 1 < points.Length)
        {
            currentPoint += 1;
        }
        else
        {
            LinearBackwardsBehaviour();

            return;
        }

        target = points[currentPoint];
    }

    private void LinearBackwardsBehaviour()
    {
        if(forward)
        {
            forward = !forward;
        }

        if(currentPoint - 1 >= 0)
        {
            currentPoint -= 1;
        }
        else
        {
            LinearForwardBehaviour();

            return;
        }

        target = points[currentPoint];
    }
    #endregion
}
