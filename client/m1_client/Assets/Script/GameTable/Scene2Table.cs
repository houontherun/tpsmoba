using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class Scene2TableConfig : SceneTableConfig
    {
        public override string GetTableName()
        {
            return "Scene2";
        }

        public override bool Load(string text)
        {
            JsonData jsonData = JsonMapper.ToObject(text);
            Row = jsonData.Count;
            m_kMapDatas = new int[Row][];
            for (int i = 0; i < jsonData.Count; i++)
            {
                int j = 0;
                JsonData data = jsonData[i];
                JsonData tempX1 = data["X1"];
                int iX1 = int.Parse(tempX1.ToString());
                j++;
                JsonData tempX2 = data["X2"];
                int iX2 = int.Parse(tempX2.ToString());
                j++;
                JsonData tempX3 = data["X3"];
                int iX3 = int.Parse(tempX3.ToString());
                j++;
                JsonData tempX4 = data["X4"];
                int iX4 = int.Parse(tempX4.ToString());
                j++;
                JsonData tempX5 = data["X5"];
                int iX5 = int.Parse(tempX5.ToString());
                j++;
                JsonData tempX6 = data["X6"];
                int iX6 = int.Parse(tempX6.ToString());
                j++;
                JsonData tempX7 = data["X7"];
                int iX7 = int.Parse(tempX7.ToString());
                j++;
                JsonData tempX8 = data["X8"];
                int iX8 = int.Parse(tempX8.ToString());
                j++;
                JsonData tempX9 = data["X9"];
                int iX9 = int.Parse(tempX9.ToString());
                j++;
                JsonData tempX10 = data["X10"];
                int iX10 = int.Parse(tempX10.ToString());
                j++;
                JsonData tempX11 = data["X11"];
                int iX11 = int.Parse(tempX11.ToString());
                j++;
                JsonData tempX12 = data["X12"];
                int iX12 = int.Parse(tempX12.ToString());
                j++;
                JsonData tempX13 = data["X13"];
                int iX13 = int.Parse(tempX13.ToString());
                j++;
                JsonData tempX14 = data["X14"];
                int iX14 = int.Parse(tempX14.ToString());
                j++;
                JsonData tempX15 = data["X15"];
                int iX15 = int.Parse(tempX15.ToString());
                j++;
                JsonData tempX16 = data["X16"];
                int iX16 = int.Parse(tempX16.ToString());
                j++;
                JsonData tempX17 = data["X17"];
                int iX17 = int.Parse(tempX17.ToString());
                j++;
                Col = j ;
                m_kMapDatas[i] = new int[] {iX1, iX2, iX3, iX4, iX5, iX6, iX7, iX8, iX9, iX10, iX11, iX12, iX13, iX14, iX15, iX16, iX17, };
            }

            return true;
        }

    }
}
