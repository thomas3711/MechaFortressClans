using UnityEngine;
using TMPro;

public class DebugInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_output;

    public static DebugInfoUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Message(string message)
    {
        text_output.text = message;
    }
}
