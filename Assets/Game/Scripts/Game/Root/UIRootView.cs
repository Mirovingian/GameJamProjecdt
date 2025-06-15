using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _restartScreen;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _gameplayUI;

    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _lightBar;
    [SerializeField] private Image _bulletBar;
    [SerializeField] private TMP_Text _pillsAmount;
    [SerializeField] private TMP_Text _catsAmount;


    private void Awake()
    {
        HideLoadingScreen();
        HideRestartScreen();
    }

    public void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        _loadingScreen.SetActive(false);
    }

    public void ChangeHealthBarView(float value)
    {
        _healthBar.fillAmount = value;
    }

    public void ChangeLightBarView(float value)
    {
        _lightBar.fillAmount = value;
    }

    public void ChangeBulletBarView(float value)
    {
        _bulletBar.fillAmount = value;
    }

    public void ChangePillsAmountView(int value)
    {
        _pillsAmount.text = value.ToString();
    }

    public void Restart()
    {
        GameEntryPoint._instance.Restart();
    }

    public void ShowRestartScreen()
    {
        _restartScreen.SetActive(true);
    }

    public void HideRestartScreen()
    {
        _restartScreen.SetActive(false);
    }

    public void ShowWinScreen()
    {
        _winScreen.SetActive(true);
    }

    public void HideWinScreen()
    {
        _winScreen.SetActive(false);
    }

    public void SetCatsCount(int count)
    {
        _catsAmount.text = count.ToString() + '/' + GameEntryPoint.MaxCatsCount;
    }

    public void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
    }

    public void HideMainMenu()
    {
        _mainMenu.SetActive(false);
    }

    public void DownloadGameplay()
    {
        GameEntryPoint._instance.DownloadGameplay();
    }


    public void ShowGameplayUI()
    {
        _gameplayUI.SetActive(true);
    }

    public void HideGameplayUI()
    {
        _gameplayUI.SetActive(false);
    }

}

