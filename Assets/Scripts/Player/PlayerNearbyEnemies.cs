using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNearbyEnemies : MonoBehaviour
{
    #region Attributes
    private List<EnemyHealth> enemies = new List<EnemyHealth>();
    #endregion

    #region MonoBehaviour Methods
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

            if(!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

            enemies.Remove(enemy);
        } 
    }
    #endregion

    #region Normal Methods
    public List<EnemyHealth> GetEnemies()
    {
        return enemies;
    }

    public void RemoveEnemy(EnemyHealth enemy)
    {
        if(enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    #endregion
}
