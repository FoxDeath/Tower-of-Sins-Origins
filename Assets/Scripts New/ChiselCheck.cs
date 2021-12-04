using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiselCheck : MonoBehaviour
{
    public bool chisel = false;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("DestructibleObject") && other.gameObject.layer != LayerMask.NameToLayer("Blockade"))
        {
            chisel = true;
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Blockade"))
        {
            chisel = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("DestructibleObject") && !other.gameObject.layer.Equals("Blockade"))
        {
            chisel = false;
        }
    }
}
