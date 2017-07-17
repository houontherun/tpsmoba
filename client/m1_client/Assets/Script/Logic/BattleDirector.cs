using UnityEngine;

public class BattleDirector : Object
{
    static BattleDirector instance = new BattleDirector();
    public static BattleDirector Instance { get { return instance;  } }


    public Character hero;

    public void Start ()
    {
        ResourceManager.CreateCharacter("FutureSoldier/FutureSoldier_01", CharacterLoadCallBack);
    }
	
    void CharacterLoadCallBack(object o)
    {
        var go = o as GameObject;
        go.AddComponent<CameraController>().target_ = go.transform;
        hero = go.GetComponent<Character>();
        if (null == hero)
        {
            hero = go.AddComponent<Character>();
        }
    }
}
