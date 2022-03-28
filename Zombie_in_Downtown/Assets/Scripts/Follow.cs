using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //public Transform target;
    //public Vector3 offset;

    [Header("카메라기본속성")]
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;
    //private Transform LookAtPos = null;
    [Header("3인칭 카메라")]
    public float Distance = 5.0f; // 타겟으로부터 떨어진 거리.
    public float Height = 1.5f; // 타겟의 위치보다 더 추가적인 높이.
    public float HeightDamping = 2.0f;
    public float RotationDamping = 3.0f;

    //public float speed;
    //float hAxis;
    //float vAxis;
    //Vector3 moveVec;
    

    void Start()
    {
        if (Target == null)
            Target = GameObject.FindWithTag("Player");

        myTransform = GetComponent<Transform>();
        if (Target != null)
        {
            targetTransform = Target.transform;
        }
    }

    void ThirdView()
    {
        Player player = Target.GetComponent<Player>();
        bool isFireReady = player.GetIsFireReady();
        if (isFireReady)
        {
            float wantedRotationAngle = targetTransform.eulerAngles.y; //현재 타겟의 y축 각도 값.
            float wantedHeight = targetTransform.position.y + Height; //현재 타겟의 높이 + 우리가 추가로 높이고 싶은 높이.

            float currentRotationAngle = myTransform.eulerAngles.y; //현재 카메라의 y축 각도 값.
            float currentHeight = myTransform.position.y;//현재 카메라의 높이값.

            //현재 각도에서 원하는 각도로 댐핑값을 얻게 됩니다.
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle,
                                                    RotationDamping * Time.deltaTime);
            //현재 높이에서 원하는 높이로 댐핑값을 얻습니다.
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight,
                                        HeightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

            myTransform.position = targetTransform.position;
            myTransform.position -= currentRotation * Vector3.forward * Distance;
           
            myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);

            myTransform.LookAt(targetTransform);
        }
    }

    // update함수 후에 호출되는 업데이트.
    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }
        if (targetTransform == null)
        {
            targetTransform = Target.transform;
        }

        ThirdView();

        /*if (Input.GetKey("a"))
            transform.rotation=Quaternion.Euler(160, 90, 180);
        else if (Input.GetKey("d"))
            transform.rotation=Quaternion.Euler(160, 270, 180);
        else if (Input.GetKey("w"))
            transform.rotation=Quaternion.Euler(160, 180, 180);
        else if (Input.GetKey("s"))
            transform.rotation=Quaternion.Euler(160, 0, 180);
        */



       // transform.position = target.position + offset;
    }
}
