using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public Animator animator;
    public GameObject scopeOverlay;
    public bool isScoped = false;
    public Camera scopeCamera;
    public Player player;

    // Update is called once per frame
    void Update()
    {
        if (isScoped)
        {
            StartCoroutine(OnScoped());
            Turn();
        }
        else OnUnScoped();
    }

    void OnUnScoped()
    {
        scopeOverlay.SetActive(false);
        scopeCamera.gameObject.SetActive(false);
    }

    IEnumerator OnScoped()
    {
        yield return new WaitForSeconds(.15f);
        scopeOverlay.SetActive(true);
        scopeCamera.gameObject.SetActive(true);
    }

    void Turn()
    {
        //2.마우스에 의한 회전
       
            Ray ray = scopeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - player.transform.position;
                //CameraTransform.LookAt(transform.position + nextVec);
                nextVec.y = 0.0f;
                player.transform.forward = Vector3.Lerp(player.transform.forward, nextVec,
                    0.5f * Time.deltaTime);
            }
        
    }
}
