using UnityEngine;

public class ContextTesting : MonoBehaviour
{
    /// Add a context menu named "Do Something" in the inspector
    /// of the attached script.
    [ContextMenu("Clear prefs")]
    void DoSomething()
    {
        PlayerPrefs.DeleteAll();
    }
}