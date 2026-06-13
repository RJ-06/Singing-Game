using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MicManager : MonoBehaviour
{
    public static MicManager instance;

    public float currentPitchHz;
    public string currentNote;


    private AudioSource audioSource;
    private const int SAMPLE_SIZE = 8192; // Must be a power of 2
    private float[] spectrumData = new float[SAMPLE_SIZE];
    private int sampleRate;

    public string[] notes;
    [SerializeField] private float volumeThreshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        sampleRate = AudioSettings.outputSampleRate;

        // Check if there is at least one microphone connected
        if (Microphone.devices.Length > 0)
        {
            // Start recording into a looping 1-second AudioClip
            audioSource.clip = Microphone.Start(null, true, 1, sampleRate);
            audioSource.loop = true;
            //audioSource.volume = 0;

            // Wait until the microphone starts recording to prevent lag spikes
            while (!(Microphone.GetPosition(null) > 0)) { }

            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone device detected!");
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Microphone.IsRecording(null))
        {
            AnalyzePitch();
            
        }
        currentNote = GetNoteFromPitch(currentPitchHz);

        //if(currentPitchHz != 0)Debug.Log("pitch in hz: " + currentPitchHz + ", note:" + currentNote);
    }

    void AnalyzePitch()
    {
        // Get FFT spectrum data from the playing audio source
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        float maxVolume = 0f;
        int maxIndex = 0;

        // Find the index of the highest frequency bin (peak volume)
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            if (spectrumData[i] > maxVolume)
            {
                maxVolume = spectrumData[i];
                maxIndex = i;
            }
        }

        // Convert the bin index to an actual frequency in Hertz
        // Formula: Frequency = Bin Index * (Sample Rate / 2) / Spectrum Size
        float fundamentalFrequency = maxIndex * ((float)sampleRate / 2f) / SAMPLE_SIZE;

        // Optional: Filter out background noise using a threshold volume
        if (maxVolume > volumeThreshold)
        {
            currentPitchHz = fundamentalFrequency;

        }
        else
        {
            currentPitchHz = 0f; // Silence
        }
    }


    string GetNoteFromPitch(float pitch) 
    {
        if (pitch <= 0) 
        {
            return "N/A";
        }

        //12 * log2 (f/440)
        float numSemitones = 12 * Mathf.Log((pitch / 440),2);

        int roundedSemitones = Mathf.RoundToInt(numSemitones);
        //1 octave = 12 semitones
        int placeInOctave = ((roundedSemitones % 12) + 12) % 12;

        return notes[placeInOctave];
    }
}
