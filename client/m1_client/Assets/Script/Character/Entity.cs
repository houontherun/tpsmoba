using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityType
{
    EN_NULL,
    EN_PLAYER, //玩家
    EN_CELL,  //场景元素
    EN_PLAYERAI,
    EN_PLAYEROTHER,
}


public class Entity
{
    public int uID;
    public EntityType entityTpye;
    public EntityBehavior entityBehavior;
    void SetBehavior(EntityBehavior behavior)
    {
        entityBehavior = behavior;
    }

}

public class Cell : Entity
{
    protected int hp = -1; //不可摧毁的建筑物
    public bool bObstacle = false ; //是否可阻挡
    public bool bDestoryByAttack = false; //是否被普攻摧毁
    public bool bDestoryBySkill = false; //是否被技能摧毁
    public float fRecoveryTime = 0f;
    public bool bSteralth = false;       //是否隐身
    public Vector2 vTilePos = new Vector2(0, 0);		// 在网格中的位置
}

public class Grass : Cell  //草元素
{
    private bool CharacterInCell = false; //是否有角色进入

}


public class Obstacles : Cell
{
    public enum ObstacleType
    {
        OB_BOX,          //箱子
        OB_BUCKET,       //水桶
        OB_WALL,         //墙
        OB_WATER,        //水
    }

    public ObstacleType kCellTpye = ObstacleType.OB_BOX;
}

