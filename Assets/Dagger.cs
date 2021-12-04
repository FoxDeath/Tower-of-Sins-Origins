using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
public Transform center;
 public Vector3 axis = Vector3.up;
 public float radius = 2.0f;
 public float radiusSpeed = 0.5f;
 public float rotationSpeed = 80.0f; 

    // Start is called before the first frame update
    void Start()
    {
        center = GameObject.FindGameObjectWithTag("Player").transform;

        FindObjectOfType<DaggerCollider>().daggers.Add(this);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround (center.localPosition, axis, rotationSpeed);
        var desiredPosition = (transform.position - center.localPosition).normalized * radius + center.localPosition;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("DestructibleObject"))
        {
                other.GetComponent<ObjectDestructible>().DestroyObject();

                FindObjectOfType<DaggerCollider>().daggers.Remove(this);

                Destroy(gameObject);
        }
    }
}
