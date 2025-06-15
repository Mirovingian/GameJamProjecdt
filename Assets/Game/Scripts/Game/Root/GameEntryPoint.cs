
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;


class GameEntryPoint
{

    public static GameEntryPoint _instance;
    public UIRootView _uiRoot;
    public ManagerPills _managerPills;
    public PlayerController _playerController;
    public CameraEffects _cameraEffects;
    public ShockWaveManager _shockWaveManager;

    public GameObject _pillPrefab;

    private Coroutines _coroutines;

    public bool isOverdose;
    public void StartOverdose()
    {
        isOverdose = true;
        _cameraEffects.SetHeartbeatEffect(true);
        _shockWaveManager.CallShockWave();
    }
    public void TickOverdose()
    {
        _cameraEffects.DescreseZoom();
        _shockWaveManager.CallShockWave();
    }
    public void EndOverdose()
    {
        isOverdose = false;
        _cameraEffects.SetHeartbeatEffect(false);
    }


    private int _catsCount = 0;
    public const int MaxCatsCount = 8;
    public float _timePick = 0.2f;
    public void IncreaseCatsCount()
    {
        var currentTime = Time.realtimeSinceStartup;
        if (currentTime > _timePick + 0.2f)
        {
            ++_catsCount;
            _uiRoot.SetCatsCount(_catsCount);
            if (_catsCount == MaxCatsCount)
            {
                _uiRoot.ShowWinScreen();
            }
        }
        _timePick = Time.realtimeSinceStartup;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutostartGame()
    {
        Application.targetFrameRate = 100;

        _instance = new GameEntryPoint();
        _instance.RunGame();

    }

    private GameEntryPoint()
    {
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        UnityEngine.Object.DontDestroyOnLoad(_coroutines.gameObject);

        var prefabUIRoot = Resources.Load<UIRootView>("UIRootView");
        _uiRoot = UnityEngine.Object.Instantiate(prefabUIRoot);
        UnityEngine.Object.DontDestroyOnLoad(_uiRoot.gameObject);

        var prefabPillsManager = Resources.Load<ManagerPills>("PillsManager");
        _managerPills = UnityEngine.Object.Instantiate(prefabPillsManager);
        UnityEngine.Object.DontDestroyOnLoad(_managerPills);

        _pillPrefab = Resources.Load<GameObject>("Pill");
    }

    private void RunGame()
    {
#if UNITY_EDITOR
        var sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == Scenes.GAMEPLAY)
        {
            _coroutines.StartCoroutine(LoadAndStartGameplay());
            return;
        }
        if (sceneName == Scenes.MAIN_MENU)
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenu());
            return;
        }
        if (sceneName != Scenes.BOOT)
        {
            return;
        }

#endif 
        _coroutines.StartCoroutine(LoadAndStartMainMenu());
    }

    public void Restart()
    {
        _managerPills._timePick = 0.2f;
        _timePick = 0.2f;
        _catsCount = 0;
        _uiRoot.HideRestartScreen();
        _uiRoot.HideWinScreen();
        _uiRoot.ChangePillsAmountView(ManagerPills.StartCountPills);
        _uiRoot.SetCatsCount(0);
        _managerPills._currentPillsCount = ManagerPills.StartCountPills;
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    public void DownloadGameplay()
    {
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    private IEnumerator LoadAndStartGameplay()
    {

 

        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.GAMEPLAY);
        _uiRoot.HideMainMenu();
        _uiRoot.ShowGameplayUI();
        _instance._playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
        _cameraEffects = UnityEngine.Object.FindObjectOfType<CameraEffects>();
        _shockWaveManager = UnityEngine.Object.FindObjectOfType<ShockWaveManager>();

        yield return null;



        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadAndStartMainMenu()
    {
        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.MAIN_MENU);

        yield return null;

        _uiRoot.ShowMainMenu();

        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}