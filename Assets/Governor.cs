using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Governor : MonoBehaviour
{
    public bool idle = true;

    private Rigidbody2D rb;

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

        rb.velocity = new Vector2(40, 0);
    }
}