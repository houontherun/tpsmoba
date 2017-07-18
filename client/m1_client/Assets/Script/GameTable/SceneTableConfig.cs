using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public abstract class SceneTableConfig
    {
        public abstract string GetTableName();

        public abstract bool Load(string text);

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        public int Col
        {
            get { return col; }
            set { col = value; }
        }

        private int row = 0;
        private int col = 0;
        public int[][] m_kMapDatas;
    }
}