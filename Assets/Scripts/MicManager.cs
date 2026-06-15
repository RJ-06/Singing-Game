using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MicManager : MonoBehaviour
{
    public static MicManager instance;

    [Header("Current Player Values")]
    public float currentPitchHz;
    public string currentNote;
    public bool canPlay = true;

    private AudioSource audioSource;
    private const int SAMPLE_SIZE = 8192; // Must be a power of 2
    private float[] spectrumData = new float[SAMPLE_SIZE];
    private int sampleRate;

    [Header("Tunable Values")]
    public static string[] notes = { "A", "Bf", "B", "C", "Cs", "D", "Ef", "E", "F", "Fs", "G", "Af"};
    [SerializeField] private float volumeThreshold;
    [Tooltip("Center pitch: ie A440 which is default")]
    public static float centerPitch = 440f;
    [Tooltip("number of semitones playable - default of 12 is 1 scale")]
    public static int numSemitonesPlayable = notes.Length;

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

            // Wait until the microphone starts recording to prevent lag spikes
            while (!(Microphone.GetPosition(null) > 0)) { }

            //audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone device detected!");
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (!canPlay) return;

        if (Microphone.IsRecording(null))
        {
            AnalyzePitch();
            
        }
        currentNote = GetNoteFromPitch(currentPitchHz);

        //if(currentPitchHz != 0)Debug.Log("pitch in hz: " + currentPitchHz + ", note:" + currentNote);
    }

    /// <summary>
    /// Get the frequency being picked up by the mic
    /// </summary>
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

    /// <summary>
    ///  Gets what note a pitch corresponds to
    /// </summary>
    /// <param name="pitch"> Pitch being played in hertz </param>
    /// <returns>string representing the note, from the list: { "A", "Bf", "B", "C", "Cs", "D", "Ef", "E", "F", "Fs", "G", "Af"}</returns>
    public static string GetNoteFromPitch(float pitch) 
    {
        if (pitch <= 0) 
        {
            return "N/A";
        }

        //12 * log2 (f/440)
        float numSemitones = numSemitonesPlayable * Mathf.Log((pitch / centerPitch),2);

        int roundedSemitones = Mathf.RoundToInt(numSemitones);
        //1 octave = 12 semitones
        int placeInOctave = ((roundedSemitones % numSemitonesPlayable) + numSemitonesPlayable) % numSemitonesPlayable;

        return notes[placeInOctave];
    }

    /// <summary>
    /// Check if the pitch being played is close to a target pitch, with a tolerance for being slightly off
    /// </summary>
    /// <param name="pitchHz">Frequency being played</param>
    /// <param name="targetNote">The target note attempting to be played</param>
    /// <param name="semitoneTolerance">How many fractional semitones off you are (default half a semitone)</param>
    /// <returns>true if the notes are close, false if not</returns>
    public static bool IsPitchCloseToNote(float pitchHz, string targetNote, float semitoneTolerance = 0.5f)
    {
        if (pitchHz <= 0) return false;

        float exactSemitones = numSemitonesPlayable * Mathf.Log(pitchHz / centerPitch, 2f);

        //target notes index
        int targetIndex = Array.IndexOf(notes, targetNote);
        if (targetIndex == -1) return false; 

        // Wrap the exact semitones into a 0 to 12 continuous scale
        float wrappedSemitones = Mathf.Repeat(exactSemitones, numSemitonesPlayable);

        // calculate distance in semitones
        float distance = Mathf.Abs(wrappedSemitones - targetIndex);
        if (distance > 6f)
        {
            distance = numSemitonesPlayable - distance;
        }

        //check tolerance
        return distance <= semitoneTolerance;
    }


}
