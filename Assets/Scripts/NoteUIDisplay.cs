using TMPro;
using UnityEngine;

public class NoteUIDisplay : MonoBehaviour
{
    private MicManager micManager;
    [SerializeField] private TextMeshProUGUI NoteDisplayText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        micManager = MicManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("pitch in hz: " + currentPitchHz + ", note:" + currentNote);
        NoteDisplayText.text = "pitch in hz: " + micManager.currentPitchHz + ", note:" + micManager.currentNote;
    }
}
