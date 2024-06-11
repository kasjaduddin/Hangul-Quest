using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyConversation : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;
    public AudioClip fourthClip;

    public bool talk;
    public int talkIndex;
    public int listenIndex;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        talk = true; 
        talkIndex = 0;
        listenIndex = 0;
    }

    public void Next()
    {
        if (talk)
        {
            switch (talkIndex)
            {
                case 0:
                    audioSource.PlayOneShot(firstClip);
                    talkIndex += 1;
                    //talk = false;
                    break;
                case 1:
                    // code block
                    audioSource.PlayOneShot(secondClip);
                    talkIndex += 1;
                    break;
                case 2:
                    audioSource.PlayOneShot(thirdClip);
                    talkIndex += 1;
                    //talk = false;
                    break;
                case 3:
                    audioSource.PlayOneShot(fourthClip);
                    talkIndex += 1;
                    break;
            }
        }
        else
        {
            
        }
    }
}
