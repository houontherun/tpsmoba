using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class ItemTable
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public int Name { get; set; }
        /// <summary>
        /// 转化碎片数量
        /// </summary>
        public int HeroShard { get; set; }
        /// <summary>
        /// 兑换所需碎片数
        /// </summary>
        public int NeedShardNum { get; set; }
        /// <summary>
        /// ICON
        /// </summary>
        public int Icon { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public int Description { get; set; }
        public ItemTable()
        { }
    }
    public class ItemTableConfig
    {
        public string GetTableName()
        {
            return "Item";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                ItemTable TableInstance = new ItemTable();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempName = data["Name"];
                TableInstance.Name = tempName.ToString();
                JsonData tempHeroShard = data["HeroShard"];
                TableInstance.HeroShard = int.Parse(tempHeroShard.ToString());
                JsonData tempNeedShardNum = data["NeedShardNum"];
                TableInstance.NeedShardNum = int.Parse(tempNeedShardNum.ToString());
                JsonData tempIcon = data["Icon"];
                TableInstance.Icon = int.Parse(tempIcon.ToString());
                JsonData tempDescription = data["Description"];
                TableInstance.Description = tempDescription.ToString();
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public ItemTable Get(int iID)
        {
            ItemTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public ItemTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<ItemTable> m_kDatas = new List<ItemTable>();
        private Dictionary<int, ItemTable> m_kMapDatas = new Dictionary<int, ItemTable>();
    }
}
