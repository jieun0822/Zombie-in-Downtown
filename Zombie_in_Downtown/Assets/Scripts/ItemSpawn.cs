using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public Transform[] itemZones;
    public GameObject[] itemObj;
    int spawnNum;
    // Start is called before the first frame update
    void Start()
    {
        spawnNum = 5;
    }

    // Update is called once per frame
    void Update()
    {
        while (spawnNum > 0)
        {
            CreateItem();
            spawnNum--;
        }
    }

    void CreateItem()
    {
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                   + Vector3.forward * Random.Range(-3, 3);
        ranVec.y = 0f;

        int ranZone = Random.Range(0, 4);
        int ranItem = Random.Range(0, 3);
        Instantiate(itemObj[ranItem], itemZones[ranZone].position + ranVec, itemZones[ranZone].rotation);
    }
}
