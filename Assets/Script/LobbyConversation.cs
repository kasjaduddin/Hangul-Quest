using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Conversation 
{
    public class LobbyConversation : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip firstClip;
        public AudioClip secondClip;
        public AudioClip thirdClip;
        public AudioClip fourthClip;
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
                listen.text.text = "";
                listen.listenButton.SetActive(false);
                listen.listenText.SetActive(false);
                switch (talkIndex)
                {
                    case 0:
                        subtitles[talkIndex].subtitle.SetActive(true);
                        audioSource.PlayOneShot(firstClip);
                        talkIndex++;
                        break;
                    case 1:
                        subtitles[talkIndex].subtitle.SetActive(true);
                        audioSource.PlayOneShot(secondClip);
                        talkIndex++;
                        Invoke(nameof(TalkAgain), secondClip.length);
                        break;
                    case 2:
                        subtitles[talkIndex].subtitle.SetActive(true);
                        audioSource.PlayOneShot(thirdClip);
                        talkIndex++;
                        break;
                    case 3:
                        audioSource.PlayOneShot(fourthClip);
                        Invoke(nameof(ShowSubtitle4), 3.35f);
                        sceneFinished = true;
                        break;
                }
            }
            else
            {
                if (sceneFinished)
                    SceneManager.LoadScene("ClassLaboratoryScene");
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
        private void ShowSubtitle4()
        {
            subtitles[talkIndex].subtitle.SetActive(true);
        }
    }
}