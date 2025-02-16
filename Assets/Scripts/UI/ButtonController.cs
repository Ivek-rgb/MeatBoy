using UnityEngine;

public class ButtonController : MonoBehaviour
{

    [Header("Button settings")]
    [Tooltip("Leave blank if not calling loadLevel() method")]public string loadLevelName; 
    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager  = LevelManager.Instance;
    }

    public void MainMenu()
    {
        _levelManager.MainMenuu();
    }

    public void ResumeGame()
    {
        _levelManager.ResumeGame();
    }

    public void RestartLevel()
    {
        _levelManager.RestartScene();
    }

    public void LoadLevel()
    {
        _levelManager.LoadScene(loadLevelName);
    }

    public void ExitGame()
    {
        _levelManager.ExitGame();
    }


}
