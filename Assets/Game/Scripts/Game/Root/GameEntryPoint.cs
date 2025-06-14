using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.SearchService;
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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutostartGame()
    {
        Application.targetFrameRate = 60;

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
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    public void Restart()
    {
        _uiRoot.HideRestartScreen();
        _coroutines.StartCoroutine(LoadAndStartGameplay());
    }

    private IEnumerator LoadAndStartGameplay()
    {
        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.GAMEPLAY);

        yield return null;

        _playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
        _cameraEffects = UnityEngine.Object.FindObjectOfType<CameraEffects>();
        _shockWaveManager = UnityEngine.Object.FindObjectOfType<ShockWaveManager>();


        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadAndStartMainMenu()
    {
        _uiRoot.ShowLoadingScreen();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.MAIN_MENU);

        yield return new WaitForSeconds(2);

        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}