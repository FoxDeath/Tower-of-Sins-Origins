using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerCollider : MonoBehaviour
{
    public List<Dagger> daggers = new List<Dagger>();

    public GameObject daggerAttackCenter;

    bool attacking = false;

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

    public IEnumerator Attack()
    {
        if(!attacking)
        {
            attacking = true;

            daggers[0].center = Instantiate(daggerAttackCenter, daggerAttackCenter.transform.position, Quaternion.identity).transform;

            daggers[0].axis = Vector3.zero;
            daggers[0].radiusSpeed = 5;

            daggers[0].GetComponent<CircleCollider2D>().enabled = true;


            yield return new WaitForSeconds(.5f);

            attacking = false;

            if(daggers.Count > 0)
            {
            daggers[0].GetComponent<CircleCollider2D>().enabled = false;

            daggers[0].center = transform.parent.parent;

            daggers[0].axis = Vector3.forward;

            daggers[0].radiusSpeed = 1;
            }
        }
        
        yield return null;
    }
}
