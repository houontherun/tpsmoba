using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class S2Table
    {
        /// <summary>
        /// y轴格子
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 1.0
        /// </summary>
        public int X1 { get; set; }
        /// <summary>
        /// 2.0
        /// </summary>
        public int X2 { get; set; }
        /// <summary>
        /// 3.0
        /// </summary>
        public int X3 { get; set; }
        /// <summary>
        /// 4.0
        /// </summary>
        public int X4 { get; set; }
        /// <summary>
        /// 5.0
        /// </summary>
        public int X5 { get; set; }
        /// <summary>
        /// 6.0
        /// </summary>
        public int X6 { get; set; }
        /// <summary>
        /// 7.0
        /// </summary>
        public int X7 { get; set; }
        /// <summary>
        /// 8.0
        /// </summary>
        public int X8 { get; set; }
        /// <summary>
        /// 9.0
        /// </summary>
        public int X9 { get; set; }
        /// <summary>
        /// 10.0
        /// </summary>
        public int X10 { get; set; }
        /// <summary>
        /// 11.0
        /// </summary>
        public int X11 { get; set; }
        /// <summary>
        /// 12.0
        /// </summary>
        public int X12 { get; set; }
        /// <summary>
        /// 13.0
        /// </summary>
        public int X13 { get; set; }
        /// <summary>
        /// 14.0
        /// </summary>
        public int X14 { get; set; }
        /// <summary>
        /// 15.0
        /// </summary>
        public int X15 { get; set; }
        /// <summary>
        /// 16.0
        /// </summary>
        public int X16 { get; set; }
        /// <summary>
        /// 17.0
        /// </summary>
        public int X17 { get; set; }
        /// <summary>
        /// 18.0
        /// </summary>
        public int X18 { get; set; }
        /// <summary>
        /// 19.0
        /// </summary>
        public int X19 { get; set; }
        /// <summary>
        /// 20.0
        /// </summary>
        public int X20 { get; set; }
        /// <summary>
        /// 21.0
        /// </summary>
        public int X21 { get; set; }
        /// <summary>
        /// 22.0
        /// </summary>
        public int X22 { get; set; }
        /// <summary>
        /// 23.0
        /// </summary>
        public int X23 { get; set; }
        /// <summary>
        /// 24.0
        /// </summary>
        public int X24 { get; set; }
        /// <summary>
        /// 25.0
        /// </summary>
        public int X25 { get; set; }
        /// <summary>
        /// 26.0
        /// </summary>
        public int X26 { get; set; }
        /// <summary>
        /// 27.0
        /// </summary>
        public int X27 { get; set; }
        /// <summary>
        /// 28.0
        /// </summary>
        public int X28 { get; set; }
        /// <summary>
        /// 29.0
        /// </summary>
        public int X29 { get; set; }
        /// <summary>
        /// 30.0
        /// </summary>
        public int X30 { get; set; }
        /// <summary>
        /// 31.0
        /// </summary>
        public int X31 { get; set; }
        /// <summary>
        /// 32.0
        /// </summary>
        public int X32 { get; set; }
        /// <summary>
        /// 33.0
        /// </summary>
        public int X33 { get; set; }
        /// <summary>
        /// 34.0
        /// </summary>
        public int X34 { get; set; }
        /// <summary>
        /// 35.0
        /// </summary>
        public int X35 { get; set; }
        /// <summary>
        /// 36.0
        /// </summary>
        public int X36 { get; set; }
        public S2Table()
        { }
    }
    public class S2TableConfig
    {
        public string GetTableName()
        {
            return "S2";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData data = jsonData[i];
                S2Table TableInstance = new S2Table();
                JsonData tempID = data["ID"];
                TableInstance.ID = int.Parse(tempID.ToString());
                JsonData tempX1 = data["X1"];
                TableInstance.X1 = int.Parse(tempX1.ToString());
                JsonData tempX2 = data["X2"];
                TableInstance.X2 = int.Parse(tempX2.ToString());
                JsonData tempX3 = data["X3"];
                TableInstance.X3 = int.Parse(tempX3.ToString());
                JsonData tempX4 = data["X4"];
                TableInstance.X4 = int.Parse(tempX4.ToString());
                JsonData tempX5 = data["X5"];
                TableInstance.X5 = int.Parse(tempX5.ToString());
                JsonData tempX6 = data["X6"];
                TableInstance.X6 = int.Parse(tempX6.ToString());
                JsonData tempX7 = data["X7"];
                TableInstance.X7 = int.Parse(tempX7.ToString());
                JsonData tempX8 = data["X8"];
                TableInstance.X8 = int.Parse(tempX8.ToString());
                JsonData tempX9 = data["X9"];
                TableInstance.X9 = int.Parse(tempX9.ToString());
                JsonData tempX10 = data["X10"];
                TableInstance.X10 = int.Parse(tempX10.ToString());
                JsonData tempX11 = data["X11"];
                TableInstance.X11 = int.Parse(tempX11.ToString());
                JsonData tempX12 = data["X12"];
                TableInstance.X12 = int.Parse(tempX12.ToString());
                JsonData tempX13 = data["X13"];
                TableInstance.X13 = int.Parse(tempX13.ToString());
                JsonData tempX14 = data["X14"];
                TableInstance.X14 = int.Parse(tempX14.ToString());
                JsonData tempX15 = data["X15"];
                TableInstance.X15 = int.Parse(tempX15.ToString());
                JsonData tempX16 = data["X16"];
                TableInstance.X16 = int.Parse(tempX16.ToString());
                JsonData tempX17 = data["X17"];
                TableInstance.X17 = int.Parse(tempX17.ToString());
                JsonData tempX18 = data["X18"];
                TableInstance.X18 = int.Parse(tempX18.ToString());
                JsonData tempX19 = data["X19"];
                TableInstance.X19 = int.Parse(tempX19.ToString());
                JsonData tempX20 = data["X20"];
                TableInstance.X20 = int.Parse(tempX20.ToString());
                JsonData tempX21 = data["X21"];
                TableInstance.X21 = int.Parse(tempX21.ToString());
                JsonData tempX22 = data["X22"];
                TableInstance.X22 = int.Parse(tempX22.ToString());
                JsonData tempX23 = data["X23"];
                TableInstance.X23 = int.Parse(tempX23.ToString());
                JsonData tempX24 = data["X24"];
                TableInstance.X24 = int.Parse(tempX24.ToString());
                JsonData tempX25 = data["X25"];
                TableInstance.X25 = int.Parse(tempX25.ToString());
                JsonData tempX26 = data["X26"];
                TableInstance.X26 = int.Parse(tempX26.ToString());
                JsonData tempX27 = data["X27"];
                TableInstance.X27 = int.Parse(tempX27.ToString());
                JsonData tempX28 = data["X28"];
                TableInstance.X28 = int.Parse(tempX28.ToString());
                JsonData tempX29 = data["X29"];
                TableInstance.X29 = int.Parse(tempX29.ToString());
                JsonData tempX30 = data["X30"];
                TableInstance.X30 = int.Parse(tempX30.ToString());
                JsonData tempX31 = data["X31"];
                TableInstance.X31 = int.Parse(tempX31.ToString());
                JsonData tempX32 = data["X32"];
                TableInstance.X32 = int.Parse(tempX32.ToString());
                JsonData tempX33 = data["X33"];
                TableInstance.X33 = int.Parse(tempX33.ToString());
                JsonData tempX34 = data["X34"];
                TableInstance.X34 = int.Parse(tempX34.ToString());
                JsonData tempX35 = data["X35"];
                TableInstance.X35 = int.Parse(tempX35.ToString());
                JsonData tempX36 = data["X36"];
                TableInstance.X36 = int.Parse(tempX36.ToString());
                ////////////////////
                m_kDatas.Add(TableInstance);
                m_kMapDatas.Add(TableInstance.ID, TableInstance);
            }

            return true;
        }

        public S2Table Get(int iID)
        {
            S2Table rkRet = null;
            if (!m_kMapDatas.TryGetValue(iID, out rkRet))
            {
                return null;
            }
            return rkRet;
        }
        public S2Table At(int index)
        {
            return m_kDatas[index];
        }

        public int GetSize()
        {
            return m_kDatas.Count;
        }
        private List<S2Table> m_kDatas = new List<S2Table>();
        private Dictionary<int, S2Table> m_kMapDatas = new Dictionary<int, S2Table>();
    }
}
