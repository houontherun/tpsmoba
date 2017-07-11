using UnityEngine;

public class BattleDirector : Object
{
    static BattleDirector instance = new BattleDirector();
    public static BattleDirector Instance { get { return instance;  } }


    public GameObject hero;

    public void Start ()
    {
        ResourceManager.CreateCharacter("FutureSoldier/FutureSoldier_01", CharacterLoadCallBack);
	}
	
    void CharacterLoadCallBack(object go)
    {
        hero = go as GameObject;
        hero.AddComponent<CameraController>().target_ = hero.transform;
    }
}
