using UnityEngine;

public class RubbleSpawn : MonoBehaviour
{
    [SerializeField] GameObject fallingRubble;
    private Transform spawnPoint;
    private Collider2D myCollider;

    private void Awake()
    {
        spawnPoint = transform.Find("SpawnPoint");

        myCollider = GetComponent<Collider2D>();
    }

    private void SpawnItem()
    {
        Instantiate(fallingRubble, spawnPoint.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            SpawnItem();

            myCollider.enabled = false;
        }
    }
}
