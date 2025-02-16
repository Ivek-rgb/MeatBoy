using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private GameObject _pauseMenu; 
    private GameManager _gameManager;
    private bool _isPaused;
    public int storedNumOfLives = 0;
    public string currentSceneName;
    private HashSet<string> _completedLevels = new HashSet<string>(); 
    
    
    public static LevelManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _completedLevels.Add("Level-1");
        _completedLevels.Add("LevelSelection");
        _completedLevels.Add("StartScreen");
        Debug.Log(storedNumOfLives);
        
    }
    
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        _loaderCanvas.SetActive(false);
        _pauseMenu.SetActive(false);
        
        
    }

    private void TogglePause(bool forced = false)
    {
        if (currentSceneName == "StartScreen" && !forced) return; 
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0 : 1; 
        _pauseMenu.SetActive(_isPaused);
    }

    private GameManager UsurpGameManager()
    {
        return FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public bool IsCompletedLevel(string levelName)
    {
        return _completedLevels.Contains(levelName); 
    }

    public async void LoadScene(string sceneName)
    {
        
        var sceneAsync = SceneManager.LoadSceneAsync($"Scenes/{sceneName}");
        
        if (sceneAsync == null) return;
        _loaderCanvas.SetActive(true);


        while (sceneAsync.progress < 0.9f)
        {
            await Task.Yield();
        }
        
        sceneAsync.allowSceneActivation = false;

        if(_isPaused) TogglePause(true);

        sceneAsync.allowSceneActivation = true;
        _loaderCanvas.SetActive(false);
        
        _completedLevels.Add(sceneName); 
        this.currentSceneName = sceneName;
        
        this._gameManager = UsurpGameManager();
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void MainMenuu()
    {
        LoadScene("StartScreen");
    }

    public void RestartScene()
    {
        LoadScene(currentSceneName);    
    }

}
