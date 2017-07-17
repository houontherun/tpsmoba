using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class MatchTable
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 匹配时间下限
        /// </summary>
        public int TimeFloor { get; set; }
        /// <summary>
        /// 战斗力差值上限
        /// </summary>
        public int FightLimit { get; set; }
        /// <summary>
        /// 是否匹配AI
        /// </summary>
        public int AI { get; set; }
        public MatchTable()
        { }
    }
    public class MatchTableConfig
    {
        public string GetTableName()
        {
            return "Match";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                MatchTable TableInstance = new MatchTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempTimeFloor = data["TimeFloor"];
                TableInstance.TimeFloor = int.Parse(tempTimeFloor.ToString());
                JsonData tempFightLimit = data["FightLimit"];
                TableInstance.FightLimit = int.Parse(tempFightLimit.ToString());
                JsonData tempAI = data["AI"];
                TableInstance.AI = int.Parse(tempAI.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public MatchTable Get(int iID)
        {
            MatchTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public MatchTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<MatchTable> m_kDatas = new List<MatchTable>();
        private Dictionary<int, MatchTable> m_kMapDatas = new Dictionary<int, MatchTable>();
    }
}
