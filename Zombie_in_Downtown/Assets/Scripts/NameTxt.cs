using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTxt : MonoBehaviour
{
    public GameObject nameTxt;

    // Update is called once per frame
    void Update()
    {
        if (nameTxt != null)
        {
            nameTxt.transform.LookAt(Camera.main.transform.position);
            nameTxt.transform.Rotate(0, 180, 0);
        }
    }
}
