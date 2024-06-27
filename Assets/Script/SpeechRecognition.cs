using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using Conversation;
using System.Collections.Generic;

namespace Speech
{
    public class SpeechRecognition : MonoBehaviour {
        [SerializeField] private Button startButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private GameObject recordButton;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private AudioSource objectiveAudioSource;
        [SerializeField] private AudioClip objectiveSfx;

        private AudioClip clip;
        private byte[] bytes;
        private bool recording;

        [SerializeField] private int speechIndex;
        private int objectiveIndex;
        private string firstSpeech = "안녕하세요, 제 이름은 부디입니다";
        private string secondSpeech = "진희는 한국인인가요?";
        private string thirdSpeech = "아니요, 저는 인도네시아 사람입니다";
        private string fourthSpeech = "진희 씨, 학생들이 캠퍼스 컴퓨터를 빌려서 과제를 할 수 있나요?";
        private string fifthSpeech = "안녕하세요 선생님, 숙제를 하기 위해 컴퓨터를 빌릴 수 있습니까?";
        private string sixthSpeech = "좋아, 작업을 완료한 후 불을 끄겠습니다";
        private string seventhSpeech = "안녕하세요 선생님, 실험실의 전기가 끊겼습니다";

        [Serializable]
        class Objective
        {
            [SerializeField] public GameObject objective;
        }

        [SerializeField]
        List<Objective> objectives = new List<Objective>();
        private void Start() {
            startButton.onClick.AddListener(StartRecording);
            stopButton.onClick.AddListener(StopRecording);
            stopButton.interactable = false;
            objectiveIndex = 0;
            objectives[objectiveIndex].objective.SetActive(true);
        }

        private void Update() {
            if (recording && Microphone.GetPosition(null) >= clip.samples) {
                StopRecording();
            }
        }

        private void StartRecording() {
            text.color = Color.white;
            text.text = "Recording...";
            startButton.interactable = false;
            stopButton.interactable = true;
            clip = Microphone.Start(null, false, 10, 44100);
            recording = true;
        }

        private void StopRecording() {
            var position = Microphone.GetPosition(null);
            Microphone.End(null);
            var samples = new float[position * clip.channels];
            clip.GetData(samples, 0);
            bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
            recording = false;
            SendRecording();
        }

        private void SendRecording() {
            text.color = Color.yellow;
            text.text = "Sending...";
            stopButton.interactable = false;
            HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
                double similarity = 0.0;
                Debug.Log(response);
                switch (speechIndex)
                {
                    case 0:
                        similarity = JaroWinklerSimilarity(response, firstSpeech);
                        break;
                    case 1:
                        similarity = JaroWinklerSimilarity(response, secondSpeech);
                        LobbyConversation.talk = true;
                        break;
                    case 2:
                        similarity = JaroWinklerSimilarity(response, thirdSpeech);
                        LobbyConversation.talk = true;
                        break;
                    case 3:
                        similarity = JaroWinklerSimilarity(response, fourthSpeech);
                        ClassLaboratoryConversation.talk = true;
                        break;
                    case 4:
                        similarity = JaroWinklerSimilarity(response, fifthSpeech);
                        ClassLaboratoryConversation.talk = true;
                        break;
                    case 5:
                        similarity = JaroWinklerSimilarity(response, sixthSpeech);
                        ClassLaboratoryConversation.sceneFinished = true;
                        break;
                    case 6:
                        similarity = JaroWinklerSimilarity(response, seventhSpeech);
                        CallConversation.talk = true;
                        break;
                }
                SimilarityCheck(similarity, "Hello");
                startButton.interactable = true;
            }, error => {
                text.color = Color.red;
                text.text = error;
                startButton.interactable = true;
            });
        }

        private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
                using (var writer = new BinaryWriter(memoryStream)) {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples) {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }

        static double JaroWinklerSimilarity(string s1, string s2)
        {
            int prefixLength = 0;
            int maxPrefixLength = Math.Min(s1.Length, s2.Length) / 2 - 1;

            while (prefixLength < maxPrefixLength && s1[prefixLength] == s2[prefixLength])
            {
                prefixLength++;
            }

            double jaroSimilarity = JaroSimilarity(s1, s2);
            double prefixScale = 0.1 * prefixLength * (1 - jaroSimilarity);

            return jaroSimilarity + prefixScale;
        }

        static double JaroSimilarity(string s1, string s2)
        {
            int matchDistance = Math.Max(s1.Length, s2.Length) / 2 - 1;
            int matches = 0;

            for (int i = 0; i < s1.Length; i++)
            {
                for (int j = Math.Max(0, i - matchDistance); j < Math.Min(s2.Length, i + matchDistance + 1); j++)
                {
                    if (s1[i] == s2[j])
                    {
                        matches++;
                        break;
                    }
                }
            }

            return (double)matches / (s1.Length + s2.Length - matches);
        }
        private void SimilarityCheck(double similarity, string speech)
        {
            if (similarity >= 0.5)
            {
                text.color = Color.white;
                text.text = speech;
                NextObjective();
                speechIndex++;
            }
            else
            {
                text.color = Color.red;
                text.text = "Jawaban salah atau pengucapan kurang tepat";
            }
        }

        private void NextObjective()
        {
            switch (speechIndex) 
            {
                case 0:
                    objectiveIndex++;
                    break;
                case 1:
                case 2:
                    recordButton.SetActive(false);
                    break;
                default: 
                    break;
            }
            objectives[objectiveIndex-1].objective.SetActive(false);
            objectives[objectiveIndex].objective.SetActive(true);
            objectiveAudioSource.PlayOneShot(objectiveSfx);
        }
    }
}