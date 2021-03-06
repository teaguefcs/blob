﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public Vector3 oPos, goPos;
    public float SPEED = 3;
    public bool open = false;
    float step;
    AudioSource source;

	void Start ()
    {
        oPos = transform.position;
        goPos.z = oPos.z;
        source = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        if (open)
        {
            step = SPEED * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goPos, step);
        }
        else
        {
            step = SPEED * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, oPos, step);
        }
	}

    public void OpenDoor()
    {
        open = true;
        source.PlayOneShot(source.clip);
    }

    public void CloseDoor()
    {
        
        open = false;
    }
}
