using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public VideoPlayer introVideo;
    // Start is called before the first frame update
    void Start()
    {
        introVideo.loopPointReached += LoadScene;
    }

    // Update is called once per frame
    void LoadScene(VideoPlayer vp)
    {
        SceneManager.LoadScene(3);
    }
}
