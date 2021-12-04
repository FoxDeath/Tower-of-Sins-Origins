using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiselCheck : MonoBehaviour
{
    public bool chisel;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("DestructibleObject"))
        {
            chisel = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("DestructibleObject"))
        {
            chisel = false;
        }
    }
}
