using TMPro;
using UnityEngine;

public class NoteUIDisplay : MonoBehaviour
{
    private MicManager micManagerInstance;
    [SerializeField] private TextMeshProUGUI NoteDisplayText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        micManagerInstance = MicManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("pitch in hz: " + currentPitchHz + ", note:" + currentNote);
        NoteDisplayText.text = "pitch in hz: " + micManagerInstance.currentPitchHz + ", note:" + micManagerInstance.currentNote;
    }
}
