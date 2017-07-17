using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class SkillTable
    {
        /// <summary>
        /// 技能ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 技能名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 射程
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// 作用范围
        /// </summary>
        public string SkillRange { get; set; }
        /// <summary>
        /// 弹道速度
        /// </summary>
        public int Speed { get; set; }
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
        /// <summary>
        /// 技能效果描述
        /// </summary>
        public int Description { get; set; }
        public SkillTable()
        { }
    }
    public class SkillTableConfig
    {
        public string GetTableName()
        {
            return "Skill";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                SkillTable TableInstance = new SkillTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempName = data["Name"];
                TableInstance.Name = tempName.ToString();
                JsonData tempDistance = data["Distance"];
                TableInstance.Distance = int.Parse(tempDistance.ToString());
                JsonData tempSkillRange = data["SkillRange"];
                TableInstance.SkillRange = tempSkillRange.ToString();
                JsonData tempSpeed = data["Speed"];
                TableInstance.Speed = int.Parse(tempSpeed.ToString());
                JsonData tempPara1 = data["Para1"];
                TableInstance.Para1 = int.Parse(tempPara1.ToString());
                JsonData tempPara2 = data["Para2"];
                TableInstance.Para2 = int.Parse(tempPara2.ToString());
                JsonData tempPara3 = data["Para3"];
                TableInstance.Para3 = int.Parse(tempPara3.ToString());
                JsonData tempDescription = data["Description"];
                TableInstance.Description = int.Parse(tempDescription.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public SkillTable Get(int iID)
        {
            SkillTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public SkillTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<SkillTable> m_kDatas = new List<SkillTable>();
        private Dictionary<int, SkillTable> m_kMapDatas = new Dictionary<int, SkillTable>();
    }
}
