using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class SceneElementTable
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ArtResource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Stop { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Destroy1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Destroy2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Invisible { get; set; }

        public SceneElementTable()
        { }
    }

    public class SceneElementTableConfig
    {
        public string GetTableName()
        {
            return "Element";
        }


        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                SceneElementTable sceneElem = new SceneElementTable();
                JsonData idValue = jsonData[i]["ID"];
                JsonData artRes = jsonData[i]["ArtResource"];
                JsonData stop = jsonData[i]["Stop"];
                JsonData idestroy1 = jsonData[i]["Destroy1"];
                JsonData idestroy2 = jsonData[i]["Destroy2"];
                JsonData itime = jsonData[i]["Time"];
                JsonData invisible = jsonData[i]["Invisible"];
                sceneElem.ID = int.Parse(idValue.ToString());
                sceneElem.ArtResource = int.Parse(artRes.ToString());
                sceneElem.Stop = int.Parse(stop.ToString());
                sceneElem.Destroy1 = int.Parse(idestroy1.ToString());
                sceneElem.Destroy2 = int.Parse(idestroy2.ToString());
                sceneElem.Time = int.Parse(itime.ToString());
                sceneElem.Invisible = int.Parse(invisible.ToString());
                m_kDatas.Add(sceneElem);
                m_kMapDatas.Add(sceneElem.ID, sceneElem);
            }

            return true;
        }

        public SceneElementTable Get(int iID)
        {
            SceneElementTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }


        public SceneElementTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }


        private List<SceneElementTable> m_kDatas = new List<SceneElementTable>();
        private Dictionary<int, SceneElementTable> m_kMapDatas = new Dictionary<int, SceneElementTable>();
    }
}
