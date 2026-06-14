using System;
using System.Collections;
using UnityEngine;


[Serializable]
public struct Note
{
    [Tooltip("The note string (e.g., 'Cs, Bf, A')")]
    public string note;

    [Tooltip("Time in seconds allowed to hit the next note")]
    public float duration;
}

public class MusicSequenceBasedLock : MonoBehaviour
{
    
    


    public Note[] noteSequence;
    protected int indexInSequence = 0;
    private MicManager micManager;
    private Coroutine timeoutTimer;

    void Start()
    {
        micManager = MicManager.instance;
    }

    void Update()
    {
        //do nothing if sequence is completed
        if (indexInSequence >= noteSequence.Length) return;

        // micManager.currentNote == noteSequence[indexInSequence].note
        if (MicManager.compareNotes(micManager.currentNote, noteSequence[indexInSequence].note, 0))
        {
            if (timeoutTimer != null) //stop previous timer if one exists
            {
                StopCoroutine(timeoutTimer);
            }

            float timeLimit = noteSequence[indexInSequence].duration;
            indexInSequence++;

            if (indexInSequence >= noteSequence.Length) //sequence completed
            {
                OnCompletedSequence();
                return;
            }

            //start timer
            timeoutTimer = StartCoroutine(ResetSequence(timeLimit, indexInSequence));
        }
    }

    IEnumerator ResetSequence(float time, int expectedIndex)
    {
        yield return new WaitForSeconds(time);

        if (expectedIndex == indexInSequence)
        {
            Debug.Log("Player took too long. Resetting sequence.");
            indexInSequence = 0;
        }
    }

    void OnCompletedSequence() 
    {
        Destroy(gameObject);
    }
}