using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Conversation 
{
    public class CallConversation : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip firstClip;
        public AudioClip secondClip;
        public AudioClip thirdClip;
        public AudioClip microphoneClip;

        public static bool talk;
        private int talkIndex;
        private int listenIndex;
        public static bool sceneFinished;

        [Serializable] 
        class Listen
        {
            [SerializeField] public GameObject listenButton;
            [SerializeField] public GameObject listenText;
            [SerializeField] public TextMeshProUGUI text;
        }

        [Serializable]
        class Subtitle
        {
            [SerializeField] public GameObject subtitle;
        }

        [SerializeField]
        List<Subtitle> subtitles = new List<Subtitle>();
        [SerializeField]
        Listen listen = new Listen();
        public void Start()
        {
            audioSource = GetComponent<AudioSource>();
            talk = false;
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
                listen.text.text = "";
                listen.listenButton.SetActive(false);
                listen.listenText.SetActive(false);
                switch (talkIndex)
                {
                    case 0:
                        audioSource.PlayOneShot(firstClip);
                        Invoke(nameof(TalkAgain), firstClip.length);
                        talkIndex += 1;
                        break;
                    case 1:
                        audioSource.PlayOneShot(secondClip);
                        talkIndex += 1;
                        Invoke(nameof(TalkAgain), secondClip.length);
                        break;
                    case 2:
                        audioSource.PlayOneShot(thirdClip);
                        talkIndex += 1;
                        sceneFinished = true;
                        break;
                }
            }
            else
            {
                if (sceneFinished)
                    SceneManager.LoadScene("StartScene");
                else
                {
                    subtitles[talkIndex - 1].subtitle.SetActive(false);
                    listen.listenButton.SetActive(true);
                    listen.listenText.SetActive(true);
                }
            }
        }
        private void TalkAgain()
        {
            talk = true;
        }
    }
}