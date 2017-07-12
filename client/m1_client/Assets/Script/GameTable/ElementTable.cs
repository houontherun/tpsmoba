using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class ElementTable
    {
        /// <summary>
        /// 元素ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 美术资源
        /// </summary>
        public int ArtResource { get; set; }
        /// <summary>
        /// 是否阻挡
        /// </summary>
        public int Stop { get; set; }
        /// <summary>
        /// 是否可被普攻摧毁
        /// </summary>
        public int Destroy1 { get; set; }
        /// <summary>
        /// 是否可被大招摧毁
        /// </summary>
        public int Destroy2 { get; set; }
        /// <summary>
        /// 恢复时间
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 能否隐身
        /// </summary>
        public int Invisible { get; set; }
        public ElementTable()
        { }
    }
    public class ElementTableConfig
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
                JsonData data = jsonData[i];
                ElementTable TableInstance = new ElementTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempArtResource = data["ArtResource"];
                TableInstance.ArtResource = int.Parse(tempArtResource.ToString());
                JsonData tempStop = data["Stop"];
                TableInstance.Stop = int.Parse(tempStop.ToString());
                JsonData tempDestroy1 = data["Destroy1"];
                TableInstance.Destroy1 = int.Parse(tempDestroy1.ToString());
                JsonData tempDestroy2 = data["Destroy2"];
                TableInstance.Destroy2 = int.Parse(tempDestroy2.ToString());
                JsonData tempTime = data["Time"];
                TableInstance.Time = int.Parse(tempTime.ToString());
                JsonData tempInvisible = data["Invisible"];
                TableInstance.Invisible = int.Parse(tempInvisible.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public ElementTable Get(int iID)
        {
            ElementTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public ElementTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<ElementTable> m_kDatas = new List<ElementTable>();
        private Dictionary<int, ElementTable> m_kMapDatas = new Dictionary<int, ElementTable>();
    }
}
