using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class HeroTable
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 英雄名
        /// </summary>
        public int Name { get; set; }
        /// <summary>
        /// 模型ID
        /// </summary>
        public int Modle { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public int Describe { get; set; }
        /// <summary>
        /// 生命值
        /// </summary>
        public int Hp { get; set; }
        /// <summary>
        /// 升级生命值增量
        /// </summary>
        public int Hpincre { get; set; }
        /// <summary>
        /// 普攻ID
        /// </summary>
        public int Attack { get; set; }
        /// <summary>
        /// 技能ID
        /// </summary>
        public int Super { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// 射击点数
        /// </summary>
        public int Shoot { get; set; }
        /// <summary>
        /// 恢复时间
        /// </summary>
        public int Time { get; set; }
        public HeroTable()
        { }
    }
    public class HeroTableConfig
    {
        public string GetTableName()
        {
            return "Hero";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                HeroTable TableInstance = new HeroTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempName = data["Name"];
                TableInstance.Name = tempName.ToString();
                JsonData tempModle = data["Modle"];
                TableInstance.Modle = int.Parse(tempModle.ToString());
                JsonData tempDescribe = data["Describe"];
                TableInstance.Describe = tempDescribe.ToString();
                JsonData tempHp = data["Hp"];
                TableInstance.Hp = int.Parse(tempHp.ToString());
                JsonData tempHpincre = data["Hpincre"];
                TableInstance.Hpincre = int.Parse(tempHpincre.ToString());
                JsonData tempAttack = data["Attack"];
                TableInstance.Attack = int.Parse(tempAttack.ToString());
                JsonData tempSuper = data["Super"];
                TableInstance.Super = int.Parse(tempSuper.ToString());
                JsonData tempSpeed = data["Speed"];
                TableInstance.Speed = int.Parse(tempSpeed.ToString());
                JsonData tempShoot = data["Shoot"];
                TableInstance.Shoot = int.Parse(tempShoot.ToString());
                JsonData tempTime = data["Time"];
                TableInstance.Time = int.Parse(tempTime.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public HeroTable Get(int iID)
        {
            HeroTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public HeroTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<HeroTable> m_kDatas = new List<HeroTable>();
        private Dictionary<int, HeroTable> m_kMapDatas = new Dictionary<int, HeroTable>();
    }
}
