using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyConversation : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip firstClip;
    public AudioClip secondClip;
    public AudioClip thirdClip;
    public AudioClip fourthClip;
    public AudioClip fifthClip;
    public AudioClip microphoneClip;


    public bool talk;
    public int talkIndex;
    public int listenIndex;
    public string microphoneName;

    [Serializable]
    class Subtitle
    {
        [SerializeField]
        public GameObject subtitle;
    }

    [SerializeField]
    List<Subtitle> subtitles = new List<Subtitle>();
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        microphoneName = Microphone.devices[0];
        talk = true; 
        talkIndex = 0;
        listenIndex = 0;
    }

    public void Next()
    {
        if (talk)
        {
            talk = false;
            if (talkIndex != 0)
                subtitles[talkIndex - 1].subtitle.SetActive(false);
            subtitles[talkIndex].subtitle.SetActive(true);
            switch (talkIndex)
            {
                case 0:
                    audioSource.PlayOneShot(firstClip);
                    talkIndex += 1;
                    break;
                case 1:
                    audioSource.PlayOneShot(secondClip);
                    talkIndex += 1;
                    talk = true;
                    break;
                case 2:
                    audioSource.PlayOneShot(thirdClip);
                    talkIndex += 1;
                    break;
                case 3:
                    audioSource.PlayOneShot(fourthClip);
                    audioSource.PlayOneShot(fifthClip);
                    talkIndex += 1;
                    break;
            }
        }
        else
        {
            subtitles[talkIndex-1].subtitle.SetActive(false);
        }
    }
}
