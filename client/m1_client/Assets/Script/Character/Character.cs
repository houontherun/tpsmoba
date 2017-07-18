using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    // Use this for initialization
    CharacterController cc;
    float speed = 0.03f;
    Animation animation;

	void Start ()
    {
        if(null == cc)
        {
            cc = gameObject.AddComponent<CharacterController>();
            cc.height = 1;
            cc.center = new Vector3(0f, 0.5f, 0f);
        }
        transform.position = Vector3.zero;
        animation = GetComponent<Animation>();
    }

    public void Move(Vector3 dir)
    {
        cc.transform.rotation = Quaternion.LookRotation(dir);
        cc.Move(dir * speed);
        animation.Play("RunForward");
    }

    public void StopMove()
    {
        animation.Play("Idle");
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
