using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class Scene2TableConfig
    {
        public string GetTableName()
        {
            return "Scene2";
        }

        public bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            row = jsonData.Count;
            m_kMapDatas = new int[row][];
            for (int i = 0; i < jsonData.Count; i++)
            {
                int j = 0;
                JsonData data = jsonData[i];
                JsonData tempID = data["ID"];
                m_kMapDatas[i][j++] = int.Parse(tempID.ToString());
                JsonData tempX1 = data["X1"];
                m_kMapDatas[i][j++] = int.Parse(tempX1.ToString());
                JsonData tempX2 = data["X2"];
                m_kMapDatas[i][j++] = int.Parse(tempX2.ToString());
                JsonData tempX3 = data["X3"];
                m_kMapDatas[i][j++] = int.Parse(tempX3.ToString());
                JsonData tempX4 = data["X4"];
                m_kMapDatas[i][j++] = int.Parse(tempX4.ToString());
                JsonData tempX5 = data["X5"];
                m_kMapDatas[i][j++] = int.Parse(tempX5.ToString());
                JsonData tempX6 = data["X6"];
                m_kMapDatas[i][j++] = int.Parse(tempX6.ToString());
                JsonData tempX7 = data["X7"];
                m_kMapDatas[i][j++] = int.Parse(tempX7.ToString());
                JsonData tempX8 = data["X8"];
                m_kMapDatas[i][j++] = int.Parse(tempX8.ToString());
                JsonData tempX9 = data["X9"];
                m_kMapDatas[i][j++] = int.Parse(tempX9.ToString());
                JsonData tempX10 = data["X10"];
                m_kMapDatas[i][j++] = int.Parse(tempX10.ToString());
                JsonData tempX11 = data["X11"];
                m_kMapDatas[i][j++] = int.Parse(tempX11.ToString());
                JsonData tempX12 = data["X12"];
                m_kMapDatas[i][j++] = int.Parse(tempX12.ToString());
                JsonData tempX13 = data["X13"];
                m_kMapDatas[i][j++] = int.Parse(tempX13.ToString());
                JsonData tempX14 = data["X14"];
                m_kMapDatas[i][j++] = int.Parse(tempX14.ToString());
                JsonData tempX15 = data["X15"];
                m_kMapDatas[i][j++] = int.Parse(tempX15.ToString());
                JsonData tempX16 = data["X16"];
                m_kMapDatas[i][j++] = int.Parse(tempX16.ToString());
                JsonData tempX17 = data["X17"];
                m_kMapDatas[i][j++] = int.Parse(tempX17.ToString());
                col = j ;
            }

            return true;
        }

        public int Row
        {
            get { return row; }
        }

        public int Col
        {
            get { return col; }
        }

        private int row = 0;
        private int col = 0;
        public int[][] m_kMapDatas;
    }
}
