using UnityEngine;
using System.Collections;

public class CoroutineInsManager : MonoBehaviour {

    #region Singleton
    private static CoroutineInsManager s_Instance = null;
    public static CoroutineInsManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject gameObjectRoot = GameObject.Find("MainManager");
                if (gameObjectRoot != null)
                {
                    s_Instance = gameObjectRoot.GetComponent<CoroutineInsManager>();
                    if (s_Instance == null)
                    {
                        s_Instance = gameObjectRoot.AddComponent<CoroutineInsManager>();
                    }
                }
            }
            return s_Instance;
        }
    }
    #endregion
}
