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
    protected int hp = -1; //为-1时表示 不可摧毁的建筑物

}

public class Grass : Cell  //草元素
{
    private bool CharacterInCell = false; //是否有角色进入
    private bool bSteralth = false;       //是否隐身
}

public class Box : Cell  //箱子
{
    
}

public class Bucket : Cell  //水桶
{

}

public class Wall : Cell  //墙
{

}
