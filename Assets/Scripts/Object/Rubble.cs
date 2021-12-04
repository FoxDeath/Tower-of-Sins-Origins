using UnityEngine;

public class Rubble : MonoBehaviour
{
    private Rigidbody2D rubbleRigidbody;

    private void Awake()
    {
        rubbleRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            rubbleRigidbody.bodyType = RigidbodyType2D.Static;

            gameObject.layer = 6; //ground layer.
        }
    }
}
