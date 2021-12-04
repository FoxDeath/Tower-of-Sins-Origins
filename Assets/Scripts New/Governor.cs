using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Governor : MonoBehaviour
{
    public bool idle = true;

    private Rigidbody2D rb;
    
    public bool hasDagger = false;

    public int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(idle)
        {
            return;
        }

        rb.velocity = new Vector2(40 * direction, 0);
    }

    public void StopIdle()
    {
        StartCoroutine(StopIdleBehaviour());
    }

    private IEnumerator StopIdleBehaviour()
    {
        yield return new WaitForSeconds(2);
        {
            idle = false;
        }
    }
}