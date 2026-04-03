using UnityEngine;
using UnityEngine.UI;

public class MainMenuLinker : MonoBehaviour
{
    public MainMenuManager manager;
    public Button playButton;

    void Awake()
    {
        if (manager == null) manager = FindFirstObjectByType<MainMenuManager>();
        
        if (playButton == null)
        {
            // Scan down to find PlayButton globally
            foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Button btn = go.GetComponentInChildren<Button>(true);
                if (btn != null && btn.name == "PlayButton")
                {
                    playButton = btn;
                    break;
                }
            }
        }
    }

    void Start()
    {
        if (manager != null && manager.modeDropdown != null)
            manager.modeDropdown.onValueChanged.AddListener(delegate { manager.OnModeChanged(); });
            
        if (playButton != null)
            playButton.onClick.AddListener(manager.OnPlayClicked);
    }
}
