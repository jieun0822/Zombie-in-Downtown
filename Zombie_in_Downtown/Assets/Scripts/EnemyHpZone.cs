using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpZone : MonoBehaviour
{
    public GameObject hpBarGroup;
    public Enemy enemy;

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && enemy.curHealth >0)
        {
            hpBarGroup.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && enemy.curHealth > 0)
        {
            hpBarGroup.SetActive(false);
        }
    }
}
