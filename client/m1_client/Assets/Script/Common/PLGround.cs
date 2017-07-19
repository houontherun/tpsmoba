/********************************************************************************
** auth： yanwei
** date： 2017-07-07
** desc： 地面网格数据管理。
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;

public class PLGround : MonoBehaviour
{
    public static PLGround Instance;
    public Transform plCellRoot = null;
    private Vector2 UnitSize = Vector2.one;
    private Vector3 plOriginRoot = new Vector3(0,0,0);
    private Vector2 GridSize;
    public int SubGridSize = 2;	//用于A*寻路的子网格
    private PLAStar AStar;	
    private Dictionary<Vector2, Cell> Cells = new Dictionary<Vector2, Cell>();

    void Awake()
    {
        Instance = this;
    }
    public void Init(int col, int row)
    {
        GridSize = new Vector2(col, row);
        Cells.Clear();
        AStar = gameObject.GetComponent<PLAStar>();
        AStar.Init((int)GridSize.x * SubGridSize, (int)GridSize.y * SubGridSize, 1.0f / (float)SubGridSize);
    }

    private Vector2 GetStartPos()
    {
        Vector2 vReturn = Vector2.zero;
        vReturn.x = GridSize.x  * -0.5f * UnitSize.x;
        vReturn.y = GridSize.y * -0.5f * UnitSize.y;
        return vReturn;
    }

    public void Move(GameObject go, Vector2 tilePos)
    {
        
        if (AStar.tiles != null)
        {
            int asx = SubGridSize * ((int)tilePos.x);
            int asy = SubGridSize * ((int)tilePos.y);
            for (int yy = 0; yy < SubGridSize; ++yy)
            {
                for (int xx = 0; xx < SubGridSize; ++xx)
                {

                }
            }
        }

        Move(go, tilePos, Vector2.one);

    }

    //考虑到可扩展性 tileSize参数表示元素占用格子大小
    private void Move(GameObject go, Vector2 tilePos, Vector2 tileSize )
    {
        Vector2 sPos = GetStartPos();
        if (tilePos.x > GridSize.x - tileSize.x) tilePos.x = GridSize.x - tileSize.x;
        if (tilePos.y > GridSize.y - tileSize.y) tilePos.y = GridSize.y - tileSize.y;

        if (go != null)
        {
            Vector3 localPos = Vector3.zero;
            localPos.x = sPos.x + (int)(tilePos.x + 0.5f) * UnitSize.x;
            localPos.y = 0.01f;
            localPos.z = sPos.y + (int)(tilePos.y + 0.5f) * UnitSize.y;
            go.transform.position = localPos;
            go.transform.rotation = transform.rotation;
            go.transform.SetParent(plCellRoot);
        }
       
    }

    public void AddCellsGround(int[][] Cells)
    {
         for (int i = 0; i < Cells.Length; i++)  
            {  
                for (int j = 0; j < Cells[i].Length; j++)  
                {
                    if(Cells[i][j]!=0)
                    {
                        ElementTable eTable = TableData.PSceneEleTableInfo.Get(Cells[i][j]);
                        ResourceManager.CreateSceneElemt(eTable.ArtResource1, delegate(UnityEngine.Object obj)
                        {
                            GameObject go = obj as GameObject;
                            Move(go, new Vector2(i, j));
                        });
                         
                    }
                    
                }  
            }   

    }

    public Vector2 GetTilePos(Vector3 vTarget, Vector2 tileSize)
    {
        Vector2 tilePos = Vector2.zero;
        Vector3 posLocal = vTarget;
        Vector2 sPos = GetStartPos();
        posLocal.x = Mathf.Clamp(posLocal.x, sPos.x, -sPos.x);
        posLocal.z = Mathf.Clamp(posLocal.z, sPos.y, -sPos.y);
        tilePos.x = (int)(posLocal.x - sPos.x) / (int)UnitSize.x;
        tilePos.y = (int)(posLocal.z - sPos.y) / (int)UnitSize.y;
        if (tilePos.x > GridSize.x - tileSize.x) tilePos.x = GridSize.x - tileSize.x;
        if (tilePos.y > GridSize.y - tileSize.y) tilePos.y = GridSize.y - tileSize.y;

        return tilePos;
    }

    public Vector3 TilePosToWorldPos(Vector2 tilePos)
    {

        Vector2 tileSize = Vector2.one;
        Vector2 sPos = GetStartPos();
        if (tilePos.x > GridSize.x - tileSize.x) tilePos.x = GridSize.x - tileSize.x;
        if (tilePos.y > GridSize.y - tileSize.y) tilePos.y = GridSize.y - tileSize.y;

        Vector3 localPos = Vector3.zero;
        localPos.x = sPos.x + (int)(tilePos.x + 0.5f) * UnitSize.x;
        localPos.y = 0.01f;
        localPos.z = sPos.y + (int)(tilePos.y + 0.5f) * UnitSize.y;

        return localPos;
    }


    // 通过地形中的物体得到A*网格
    public AStarTile GetBuidingAStarTile(Cell building)
    {
        int x = (int)building.vTilePos.x * SubGridSize;
        int y = (int)building.vTilePos.y * SubGridSize;
        return AStar.tiles[x, y];
    }

    //通过位置得到场景元素
    public Cell GetCell(Vector2 vecPos)
    {
        Cell cell = null;
        if(Cells.TryGetValue(vecPos,out cell))
        {
            return cell;
        }

        return cell;

    }
}
