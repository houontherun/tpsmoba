/********************************************************************************
** auth： yanwei
** date： 2017-07-07
** desc： 地面网格数据管理。
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLGround : SingletonBehaviour<PLGround>
{
    public Transform plCellRoot = null;
    private Vector2 UnitSize = Vector2.one;
    private Vector3 plOriginRoot = new Vector3(0,0,0);
    private Vector2 GridSize;

    public Cell[,] Cells;

	public void Init()
    {

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
        }
    }

    private void AddCellsGround()
    {

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
}
