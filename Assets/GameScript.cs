﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour {

    public GameObject player;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject[] frags;
    public GameObject[] blobs;
    
    void Start ()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        frags = GameObject.FindGameObjectsWithTag("BlobFrag");
        blobs = GameObject.FindGameObjectsWithTag("Blob");
	}
	
	void Update ()
    {
        if(player.GetComponent<Blob>().blobSize == frags.Length + 1)
        {
            Win();
        }
	}

    public void Win()
    {
        winScreen.SetActive(true);
    }

    public void Lose()
    {
        loseScreen.SetActive(true);
    }
}