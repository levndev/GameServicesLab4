using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using YG;
using UnityEngine.SceneManagement;
using System.Linq;

public class DragonPicker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TestText;
    [SerializeField] private UnityEvent authorizationCheck;
    public GameObject EnergyShieldPrefab;
    public int EnergyShieldAmount = 3;
    public float EnegyShieldBottomY = -6;
    public float EnegyShieldRadius = 1.5f;
    private bool FirstLaunch = true;
    public List<GameObject> shieldList;
    private int score;
    public TextMeshProUGUI scoreGT;

    void Start()
    {
        var scoreGO = GameObject.Find("Score");
        scoreGT = scoreGO.GetComponent<TextMeshProUGUI>();
        scoreGT.text = "0";
        shieldList = new List<GameObject>();
        for (var i = 1; i <= EnergyShieldAmount; i++)
        {
            var tShieldGo = Instantiate(EnergyShieldPrefab);
            tShieldGo.transform.position = new Vector3(0, EnegyShieldBottomY, 0);
            tShieldGo.transform.localScale = new Vector3(1 * i, 1 * i, 1 * i);
            tShieldGo.GetComponent<EnergyShield>().EggCaught += DragonEggCaught;
            shieldList.Add(tShieldGo);
        }
    }

    public void DragonEggCaught()
    {
        score++;
        scoreGT.text = score.ToString();
    }

    public void DragonEggDestroyed()
    {
        var tDragonEggArray = GameObject.FindGameObjectsWithTag("Dragon Egg");
        foreach (var egg in tDragonEggArray)
        {
            Destroy(egg);
        }
        var shield = shieldList.Last();
        shieldList.Remove(shield);
        Destroy(shield);
        if (shieldList.Count == 0)
        {
            SceneManager.LoadScene("_0Scene");
        }
    }

    public void ResolvedAuthorization()
    {
        TestText.text = $"SDK подключен. Игрок : \"{YandexGame.playerName}\"";
    }

    public void RejectedAuthorization()
    {
        TestText.text = $"SDK подключен. Авторизация провалена";
    }

    private void OnEnable() => YandexGame.GetDataEvent += SdkDataReceived;

    private void OnDisable() => YandexGame.GetDataEvent -= SdkDataReceived;

    private void SdkDataReceived()
    {
        if (YandexGame.SDKEnabled && FirstLaunch)
        {
            TestText.text = $"SDK подключен. Авторизация...";
            authorizationCheck?.Invoke();
            FirstLaunch = false;
        }
    }
}
