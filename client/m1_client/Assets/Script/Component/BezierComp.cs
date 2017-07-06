/********************************************************************************
** auth： panyinglong
** date： 2017/6/2
** desc： Bezier组件
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BezierComp : MonoBehaviour
{
    private Vector3 p0;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    Transform cacheTrans = null;
    private float time = 0f;
    private float totalTime = 0;
    private bool active = false;
    private int degree = 2;
    private Transform followTarget = null;
    void Start()
    {
        cacheTrans = transform;
    }

    void Update()
    {
        if(!active)
        {
            return;
        }
        time += Time.deltaTime * AppFacade.Instance.gameManager.GameSpeed;
        if(time > totalTime)
        {
            active = false;
        }
        if(followTarget != null)
        {
            if(degree == 2)
            {
                p2 = followTarget.position;
                p2.y += 1.5f;
            }
            else if (degree == 3)
            {
                p3 = followTarget.position;
                p3.y += 1.5f;
            }
        }
        UpdatePos(time/totalTime);
    }
    
    // 三次bezier
    public void Bezier3(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, float total)
    {
        p0 = v0;
        p1 = v1;
        p2 = v2;
        p3 = v3;
        totalTime = total;
        time = 0;
        degree = 3;
        followTarget = null;
        active = true;
    }

    // 二次bezier
    public void Bezier2(Vector3 v0, Vector3 v1, Vector3 v2, float total)
    {
        p0 = v0;
        p1 = v1;
        p2 = v2;
        totalTime = total;
        time = 0;
        degree = 2;
        followTarget = null;
        active = true;
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }
    
    private void UpdatePos(float t)
    {
        float t2 = t * t;
        float t3 = t * t * t;
        float dt = 1 - t;
        float dt2 = dt * dt;
        float dt3 = dt * dt * dt;
        if(degree == 2)
        {
            Vector3 p = dt2 * p0 + 2 * t * dt * p1 + t2 * p2;
            cacheTrans.position = p;
        }
        else if(degree == 3)
        {
            Vector3 p = p0 * dt3 + 3 * p1 * t * dt2 + 3 * p2 * t2 * dt + p3 * t3;
            cacheTrans.position = p;
        }
    }
}