using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerCollider : MonoBehaviour
{
    public List<Dagger> daggers = new List<Dagger>();

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Governor"))
        {
            if(!other.GetComponent<Governor>().hasDagger)
            {
            other.GetComponent<Governor>().hasDagger = true;
            
            daggers[0].center = other.transform;

            daggers.Remove(daggers[0]);

            other.GetComponent<Governor>().StopIdle();
            }
        }
    }
}
