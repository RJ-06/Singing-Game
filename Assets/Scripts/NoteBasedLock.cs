using UnityEngine;

public class NoteBasedLock : MonoBehaviour
{

    MicManager micManager;
    [SerializeField] string noteToUnlock;
    [SerializeField] float noteTolerance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        micManager = MicManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (MicManager.IsPitchCloseToNote(micManager.currentPitchHz, noteToUnlock, noteTolerance))
        {
            Destroy(gameObject);
        }
    }
}
