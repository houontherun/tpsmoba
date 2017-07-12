using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class DungeonTable
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 副本
        /// </summary>
        public string Dungeon { get; set; }
        /// <summary>
        /// 战斗类型
        /// </summary>
        public int Playing { get; set; }
        /// <summary>
        /// A方出生点坐标
        /// </summary>
        public List<int> BirthPoint1 { get; set; }
        /// <summary>
        /// B方出生点坐标
        /// </summary>
        public List<int> BirthPoint2 { get; set; }
        public DungeonTable()
        { }
    }
    public class DungeonTableConfig
    {
        public string GetTableName()
        {
            return "Dungeon";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                DungeonTable TableInstance = new DungeonTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempDungeon = data["Dungeon"];
                TableInstance.Dungeon = tempDungeon.ToString();
                JsonData tempPlaying = data["Playing"];
                TableInstance.Playing = int.Parse(tempPlaying.ToString());
                JsonData tempBirthPoint1 = data["BirthPoint1"];
                TableInstance.BirthPoint1 = new List<int>();
                for (int j = 0; j < tempBirthPoint1.Count; i++)
                {
                    string v = tempBirthPoint1[i].ToString();
                    if (v == "") continue;
                    TableInstance.BirthPoint1.Add(int.Parse(v));
                }
                JsonData tempBirthPoint2 = data["BirthPoint2"];
                TableInstance.BirthPoint2 = new List<int>();
                for (int j = 0; j < tempBirthPoint2.Count; i++)
                {
                    string v = tempBirthPoint2[i].ToString();
                    if (v == "") continue;
                    TableInstance.BirthPoint2.Add(int.Parse(v));
                }
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public DungeonTable Get(int iID)
        {
            DungeonTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public DungeonTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<DungeonTable> m_kDatas = new List<DungeonTable>();
        private Dictionary<int, DungeonTable> m_kMapDatas = new Dictionary<int, DungeonTable>();
    }
}
