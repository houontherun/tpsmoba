﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public  class MainScreenUI : MonoBehaviour 
{
    public GameObject btnEnterGame;

    private void Start()
    {
        ClickEventListener.Get(btnEnterGame).onClick += EnterGame;
    }

    void EnterGame(PointerEventData e)
    {
        LoadSceneManager.Instance().LoadScene("Combat01", () => {
            AppFacade.Instance.GetManager<GameManager>(ManagerName.Game).LoadinScene("Scene1");
            BattleDirector.Instance.Start(); });
    }
}

