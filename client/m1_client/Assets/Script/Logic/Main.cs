using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        AppFacade.Instance.StartUp();
        AppFacade.Instance.GetManager<GameManager>(ManagerName.Game).StartGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
