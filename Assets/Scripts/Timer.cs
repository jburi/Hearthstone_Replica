using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
	public float timeLeft = 30f;
	//public Text text_box;
	
    // Update is called once per frame
    void Update()
    {
		timeLeft -= Time.deltaTime;
		//text_box.text = timeLeft.ToString("0.00");
	}

	public float GetTime()
	{
		return timeLeft;
	}
	public void SetTime(float reducedTime)
	{
		timeLeft = reducedTime;
	}
}

