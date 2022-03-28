using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameObject managerObj;
    public GameManager manager;

    void Awake()
    {
        if (managerObj == null)
        {
            managerObj = GameObject.FindWithTag("GameManager");
            manager = managerObj.GetComponent<GameManager>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            manager.StageStart();
        }
    }
}
