using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudiologRecorder : MonoBehaviour
{
    public bool canRecord = false;
    public int maxRecordingLength = 6;
    public List<AudioClip> recordings = new List<AudioClip>();

    private AudioSource audioSource;
    private const int SAMPLE_SIZE = 8192; // Must be a power of 2
    private float[] spectrumData = new float[SAMPLE_SIZE];
    private int sampleRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        sampleRate = AudioSettings.outputSampleRate;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartRecording()
    {
        if (!canRecord) return;
        // Check if there is at least one microphone connected
        if (Microphone.devices.Length > 0)
        {
            // Start recording into a looping 1-second AudioClip
            audioSource.clip = Microphone.Start(null, false, maxRecordingLength, sampleRate);

            // Wait until the microphone starts recording to prevent lag spikes
            //while (!(Microphone.GetPosition(null) > 0)) { }

            //audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone device detected!");
        }
    }

    //public void StopRecording()
    //{
    //    if (Microphone.devices.Length > 0) 
    //    {
    //        Microphone.End(null);
    //        AudioClip clip = audioSource.clip;
    //        recordings.Add(clip);
    //    }
    //}

    public void StopRecording()
    {
        if (Microphone.IsRecording(null))
        {
            int lastSamplePosition = Microphone.GetPosition(null);
            Microphone.End(null);

            AudioClip originalClip = audioSource.clip;

            //makes sure to edit the clip to only include the actual recording, so dead time at the end isnt included
            if (lastSamplePosition > 0 && lastSamplePosition < originalClip.samples)
            {
                float[] audioData = new float[lastSamplePosition * originalClip.channels];

                originalClip.GetData(audioData, 0);

                AudioClip trimmedClip = AudioClip.Create("TrimmedAudiolog", lastSamplePosition, originalClip.channels, originalClip.frequency, false);

                trimmedClip.SetData(audioData, 0);

                recordings.Add(trimmedClip);

                Destroy(originalClip);

                Debug.Log($"Trimmed recording saved! Length: {trimmedClip.length} seconds.");
            }
            else if (lastSamplePosition > 0)
            {
                recordings.Add(originalClip);
                Debug.Log($"Full-length recording saved! Length: {originalClip.length} seconds.");
            }
            else
            {
                Debug.LogWarning("Recording was too short to save.");
            }
        }
    }
}
