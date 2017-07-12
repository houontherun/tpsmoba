using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class RankLevelTable
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int RankLevel { get; set; }
        /// <summary>
        /// 杯数
        /// </summary>
        public int Cups { get; set; }
        /// <summary>
        /// Icon
        /// </summary>
        public int Icon { get; set; }
        /// <summary>
        /// 奖励
        /// </summary>
        public List<int> Reward1 { get; set; }
        public RankLevelTable()
        { }
    }
    public class RankLevelTableConfig
    {
        public string GetTableName()
        {
            return "RankLevel";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                RankLevelTable TableInstance = new RankLevelTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempRankLevel = data["RankLevel"];
                TableInstance.RankLevel = int.Parse(tempRankLevel.ToString());
                JsonData tempCups = data["Cups"];
                TableInstance.Cups = int.Parse(tempCups.ToString());
                JsonData tempIcon = data["Icon"];
                TableInstance.Icon = int.Parse(tempIcon.ToString());
                JsonData tempReward1 = data["Reward1"];
                TableInstance.Reward1 = new List<int>();
                for (int j = 0; j < tempReward1.Count; j++)
                {
                    string v = tempReward1[i].ToString();
                    if (v == "") continue;
                    TableInstance.Reward1.Add(int.Parse(v));
                }
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public RankLevelTable Get(int iID)
        {
            RankLevelTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public RankLevelTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<RankLevelTable> m_kDatas = new List<RankLevelTable>();
        private Dictionary<int, RankLevelTable> m_kMapDatas = new Dictionary<int, RankLevelTable>();
    }
}
