﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuOptions : MonoBehaviour {
    
	void Start ()
    {
        
	}
	
	void Update ()
    {
        
	}

    public void startScene()
    {
        SceneManager.LoadScene("Test");
        Time.timeScale = 1;
    }

    public void quitScene()
    {
        Application.Quit();
    }

    public void menuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}