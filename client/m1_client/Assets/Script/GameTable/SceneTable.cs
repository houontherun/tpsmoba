using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class SceneTable
    {
        /// <summary>
        /// 
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X6 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X7 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X8 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X9 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X10 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X11 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X12 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X13 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X14 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X15 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X16 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X17 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X18 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X19 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X20 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X21 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X22 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X23 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X24 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X25 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X26 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X27 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X28 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X29 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X30 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X31 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X32 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X33 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X34 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X35 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int X36 { get; set; }

        public SceneTable()
        {

        }
    }

    public class SceneTable_S1Config
    {
        public string GetTableName()
        {
            return "S1";
        }

        public bool Load(string text)
        {
            m_kDatas = JsonMapper.ToObject<SceneTable[]>(text);
            foreach (var item in m_kDatas)
            {
                m_kMapDatas.Add(item.Y, item);
            }
            return true;
        }

        public SceneTable Get(int iID)
        {
            SceneTable rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }


        public SceneTable At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Length;
        }


        private SceneTable[] m_kDatas;
        private Dictionary<int, SceneTable> m_kMapDatas = new Dictionary<int, SceneTable>();
    }
}