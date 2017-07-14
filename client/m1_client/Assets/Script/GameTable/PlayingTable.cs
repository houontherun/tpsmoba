using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class PlayingTable
    {
        /// <summary>
        /// 战斗玩法
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 每队人数或混战总人数
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 参数1
        /// </summary>
        public int Para1 { get; set; }
        /// <summary>
        /// 参数2
        /// </summary>
        public int Para2 { get; set; }
        /// <summary>
        /// 参数3
        /// </summary>
        public int Para3 { get; set; }
        public PlayingTable()
        { }
    }
    public class PlayingTableConfig
    {
        public string GetTableName()
        {
            return "Playing";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                PlayingTable TableInstance = new PlayingTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempNumber = data["Number"];
                TableInstance.Number = int.Parse(tempNumber.ToString());
                JsonData tempPara1 = data["Para1"];
                TableInstance.Para1 = int.Parse(tempPara1.ToString());
                JsonData tempPara2 = data["Para2"];
                TableInstance.Para2 = int.Parse(tempPara2.ToString());
                JsonData tempPara3 = data["Para3"];
                TableInstance.Para3 = int.Parse(tempPara3.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public PlayingTable Get(int iID)
        {
            PlayingTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public PlayingTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<PlayingTable> m_kDatas = new List<PlayingTable>();
        private Dictionary<int, PlayingTable> m_kMapDatas = new Dictionary<int, PlayingTable>();
    }
}
