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
    }

    public class SceneElementTableConfig
    {
        public string GetTableName()
        {
            return "SceneElementTable";
        }


        public bool Load(string text)
        {
            m_kDatas = JsonHelp.ReadFromJsonString<SceneElementTable[]>(text);
            foreach (var item in m_kDatas)
            {
                m_kMapDatas.Add(item.ID, item);
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
            return m_kDatas.Length;
        }


        private SceneElementTable[] m_kDatas;
        private Dictionary<int, SceneElementTable> m_kMapDatas = new Dictionary<int, SceneElementTable>();
    }
}
