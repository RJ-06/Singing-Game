using UnityEngine;

public class NoteBasedLock : MonoBehaviour
{

    MicManager micManagerInstance;
    [SerializeField] string noteToUnlock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        micManagerInstance = MicManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (micManagerInstance.currentNote == noteToUnlock) 
        {
            Destroy(gameObject);
        }
    }
}
