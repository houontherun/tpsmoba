using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class RandomAwardTable
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 礼包ID
        /// </summary>
        public int Drop { get; set; }
        /// <summary>
        /// 奖励道具
        /// </summary>
        public string BonusProps { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
        public RandomAwardTable()
        { }
    }
    public class RandomAwardTableConfig
    {
        public string GetTableName()
        {
            return "RandomAward";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                RandomAwardTable TableInstance = new RandomAwardTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempDrop = data["Drop"];
                TableInstance.Drop = int.Parse(tempDrop.ToString());
                JsonData tempBonusProps = data["BonusProps"];
                TableInstance.BonusProps = tempBonusProps.ToString();
                JsonData tempWeight = data["Weight"];
                TableInstance.Weight = int.Parse(tempWeight.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public RandomAwardTable Get(int iID)
        {
            RandomAwardTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public RandomAwardTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<RandomAwardTable> m_kDatas = new List<RandomAwardTable>();
        private Dictionary<int, RandomAwardTable> m_kMapDatas = new Dictionary<int, RandomAwardTable>();
    }
}
