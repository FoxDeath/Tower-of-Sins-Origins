using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    #region Attributes
    [SerializeField] GameObject objectToSpawn;

    private Collider2D myCollider;

    private Transform spawnPos;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();

        spawnPos = transform.Find("SpawnPos");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        myCollider.enabled = false;

        Instantiate(objectToSpawn, spawnPos.position, Quaternion.identity);
    }
    #endregion

    #region Normal Methods

    #endregion
}
