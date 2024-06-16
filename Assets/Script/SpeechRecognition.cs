using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using HuggingFace.API;
using Unity.VisualScripting;

public class SpeechRecognition : MonoBehaviour
{
    [SerializeField]
    public Button listenButton;
    [SerializeField]
    public TextMeshProUGUI listenText;

    public int buttonClick;
    public AudioClip clip;
    public byte[] bytes;
    public bool recording;
    // Start is called before the first frame update
    void Start()
    {
        buttonClick = 0;
        if (buttonClick == 0)
            listenButton.onClick.AddListener(StartRecording);
        else
        {
            listenButton.onClick.AddListener(StopRecording);
            buttonClick = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(recording && Microphone.GetPosition(null) >= clip.samples)
            StopRecording();
    }

    void StartRecording()
    {
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memorySystem = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memorySystem))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }

            return memorySystem.ToArray();
        }
    }

    private void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response =>
        {
            listenText.text = response;
        }, error =>
        {
            listenText.text = error;
        });
    }
}
