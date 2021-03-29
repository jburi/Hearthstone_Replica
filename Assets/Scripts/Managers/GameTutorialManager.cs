using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTutorialManager : MonoBehaviour
{
    public bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetTutorialStatus()
    {
        return isEnabled;
    }

    public IEnumerator RunTutorial()
    {
        yield return new WaitForSeconds(5f);
    }
}
