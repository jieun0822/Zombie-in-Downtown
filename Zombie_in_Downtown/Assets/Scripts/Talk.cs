using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    public AudioSource[] npcSounds;
    int random = -1;

    public void Enter()
    {
        random = Random.Range(0, npcSounds.Length);
        npcSounds[random].Play();
    }

    public void Exit()
    {
        if (random != -1 && npcSounds[random].isPlaying)
        {
            npcSounds[random].Stop();
            random = -1;
        }
    }
}
