using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using Conversation;

namespace Examples {
    public class SpeechRecognitionExample : MonoBehaviour {
        [SerializeField] private Button startButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private TextMeshProUGUI text;

        private AudioClip clip;
        private byte[] bytes;
        private bool recording;

        private void Start() {
            startButton.onClick.AddListener(StartRecording);
            stopButton.onClick.AddListener(StopRecording);
            stopButton.interactable = false;
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
                double similarity = JaroWinklerSimilarity(response, "Hello");
                if (similarity >= 0.5)
                {
                    text.color = Color.white;
                    text.text = response;
                    LobbyConversation.talk = true;
                }
                else
                {
                    text.color = Color.red;
                    text.text = "Jawaban salah atau pengucapan kurang tepat";
                }
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
    }
}