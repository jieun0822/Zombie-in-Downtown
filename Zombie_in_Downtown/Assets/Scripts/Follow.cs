using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    //public Transform target;
    //public Vector3 offset;

    [Header("ī�޶�⺻�Ӽ�")]
    private Transform myTransform = null;
    public GameObject Target = null;
    private Transform targetTransform = null;
    //private Transform LookAtPos = null;
    [Header("3��Ī ī�޶�")]
    public float Distance = 5.0f; // Ÿ�����κ��� ������ �Ÿ�.
    public float Height = 1.5f; // Ÿ���� ��ġ���� �� �߰����� ����.
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
            float wantedRotationAngle = targetTransform.eulerAngles.y; //���� Ÿ���� y�� ���� ��.
            float wantedHeight = targetTransform.position.y + Height; //���� Ÿ���� ���� + �츮�� �߰��� ���̰� ���� ����.

            float currentRotationAngle = myTransform.eulerAngles.y; //���� ī�޶��� y�� ���� ��.
            float currentHeight = myTransform.position.y;//���� ī�޶��� ���̰�.

            //���� �������� ���ϴ� ������ ���ΰ��� ��� �˴ϴ�.
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle,
                                                    RotationDamping * Time.deltaTime);
            //���� ���̿��� ���ϴ� ���̷� ���ΰ��� ����ϴ�.
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight,
                                        HeightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);

            myTransform.position = targetTransform.position;
            myTransform.position -= currentRotation * Vector3.forward * Distance;
           
            myTransform.position = new Vector3(myTransform.position.x, currentHeight, myTransform.position.z);

            myTransform.LookAt(targetTransform);
        }
    }

    // update�Լ� �Ŀ� ȣ��Ǵ� ������Ʈ.
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
