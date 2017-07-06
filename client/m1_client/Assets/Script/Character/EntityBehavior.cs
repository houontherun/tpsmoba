using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour {

    private Entity pEntity;
	
    public void SetEntityProperty(Entity entity)
    {
        pEntity = entity;
    }

    public Entity GetEntity()
    {
        return pEntity;
    }
}
