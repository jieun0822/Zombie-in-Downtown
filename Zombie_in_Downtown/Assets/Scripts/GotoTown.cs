using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoTown : MonoBehaviour
{
    public GameObject Target;
    public GameManager manager;
    private CharacterController myCharacterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    void Awake()
    {
        if (Target == null)
        {
            Target = GameObject.FindWithTag("Player");
            myCharacterController = Target.GetComponent<CharacterController>();
        }
    }

    void Start()
    {
       
    }

    public void LoadScene()
    {
        //Target.transform.position = new Vector3(-18, 0, 0);
        //Vector3 vector = new Vector3(-18, 0, 6);
        //vector = vector.normalized;
        //collisionFlags = myCharacterController.Move(vector);

        //SceneManager.LoadScene(0);

        ChangePositionController();
    }

    public void ChangePositionController()
    {
        StartCoroutine(ChangePos());
    }

    IEnumerator ChangePos()
    {
        myCharacterController.enabled = false;
        yield return null;
        Target.transform.position = new Vector3(-18, 0, 6);
        yield return null;
        myCharacterController.enabled = true;

        SceneManager.LoadScene(1);

        //manager.menuSound.Stop();
        //manager.townSound.Play();

        //manager.menuCam.SetActive(false);
        //manager.gameCam.SetActive(true);

        //manager.menuPanel.SetActive(false);
        //manager.gamePanel.SetActive(true);

    }

}
