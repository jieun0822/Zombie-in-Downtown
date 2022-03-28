using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    //public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText;

    Player enterPlayer;

    public AudioSource helpSound;
    //public AudioSource byeSound;

    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;

        helpSound.Play();
    }

    // Update is called once per frame
    public void Exit()
    {
        if (helpSound.isPlaying) helpSound.Stop();
        //byeSound.Play();

        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        if (itemObj[index].tag != "Weapon")
        {
            Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                + Vector3.forward * Random.Range(-3, 3);
            ranVec.y = 2f;
            Instantiate(itemObj[index], itemPos[index].position + ranVec,
               itemPos[index].rotation);
        }
        else
        {
            enterPlayer.hasWeapons[index] = true;
        }
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
