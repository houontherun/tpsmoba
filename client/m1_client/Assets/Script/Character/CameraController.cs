/********************************************************************
	purpose:	镜头跟随英雄，球面坐标系
*********************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    float alpha_ = 1.5F;
    float beta_ = 1.6f;
    float speed = 0.02F;

    float distance = 9.3F;
    float offsetDistance = 0;
    float maxOffsetDistance = 5;
    float minOffsetDistance = -6.8f;
    float hideUIDistance = 4;

    float normalRotateX = 45f;
    float offsetRotateX = 0;
    float maxOffestRotateX = 0;
    float minOffestRotateX = -15;

    Vector3 tCameraSpeed_ = Vector3.zero;
    Vector3 cameraRotateSpeed = Vector3.zero;
    float lastDistance = 0f;
    Camera cacheCamera;
    public Transform target_ = null;

    void Awake()
    {
        cacheCamera = Camera.main;
    }

    void Start()
    {
        Reset();
    }

    [ContextMenu("Reset Camera")]
    public void Reset()
    {
        if (null == target_)
        {
            return;
        }
        offsetDistance = 0;
        offsetRotateX = 0;
        cacheCamera.transform.position = GetDestination();
    }

    Vector3 GetDestination()
    {
        Vector3 p = target_.position;
        p.y += (distance + offsetDistance) * Mathf.Sin(beta_);
        p.z -= (distance + offsetDistance) * Mathf.Cos(alpha_);
        p.x += (distance + offsetDistance) * Mathf.Sin(alpha_);
        return p;
    }

    bool initial = false;

    void LateUpdate()
    {
        if (null == target_) return;

        if (maxOffsetDistance == 0) maxOffsetDistance = 1;
        if (minOffsetDistance == 0) minOffsetDistance = -1;

        float deltaDistance = 0;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
            {
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);
                float curDistance = Vector2.Distance(touch1.position, touch2.position);
                deltaDistance = (lastDistance - curDistance) * 0.01f;
                lastDistance = curDistance;
            }
        }
        else
        {
            deltaDistance = Input.GetAxis("Mouse ScrollWheel") * 4;
        }

        offsetDistance += deltaDistance;
        offsetDistance = Mathf.Clamp(offsetDistance, minOffsetDistance, maxOffsetDistance);

        if (offsetDistance < minOffsetDistance) offsetDistance = minOffsetDistance;
        offsetRotateX += offsetDistance > 0 ? deltaDistance * maxOffestRotateX / maxOffsetDistance : deltaDistance * minOffestRotateX / minOffsetDistance;
        offsetRotateX = Mathf.Clamp(offsetRotateX, minOffestRotateX, maxOffestRotateX);
        Vector3 current = cacheCamera.transform.position;
        Vector3 dst = GetDestination();

        if (initial == false)   
        {
            //刚进场景不进行damp一步到位
            initial = true;
            cacheCamera.transform.localEulerAngles = new Vector3(normalRotateX + offsetRotateX, -90, 0);
            cacheCamera.transform.position = dst;
            cacheCamera.fieldOfView = 45;
        }
        else
        {
            Vector3 currentRotate = cacheCamera.transform.localEulerAngles;
            Vector3 dstRotate = new Vector3(normalRotateX + offsetRotateX, currentRotate.y, 0);
            cacheCamera.transform.position = Vector3.SmoothDamp(current, dst, ref tCameraSpeed_, speed);
            cacheCamera.transform.localEulerAngles = Vector3.SmoothDamp(currentRotate, dstRotate, ref cameraRotateSpeed, speed);
        }
    }
}
