using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIRootView : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;

    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _lightBar;
    [SerializeField] private Image _bulletBar;
    [SerializeField] private TMP_Text _pillsAmount;

    private void Awake()
    {
        HideLoadingScreen();
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

}

