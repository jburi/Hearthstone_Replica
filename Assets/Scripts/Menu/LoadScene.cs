/*
* Jake Buri
* LoadScene.cs
* Assignment 3
* Used to change scenes OnClick()
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    public void LoadTheScene(string level)
    {
        Debug.Log("Clicked");
        SceneManager.LoadScene(level);
    }
}
