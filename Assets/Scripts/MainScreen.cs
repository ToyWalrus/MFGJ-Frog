using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainScreen : MonoBehaviour
{
    public UIDocument doc;

    private void Awake()
    {
        doc.rootVisualElement.Q("PlayButton").RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        doc.rootVisualElement.Q("QuitButton").RegisterCallback<ClickEvent>(OnQuit);
    }

    private void OnPlayButtonClicked(object _)
    {
        SceneManager.LoadScene(1);
    }

    private void OnQuit(object _)
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit(0);
    }
}