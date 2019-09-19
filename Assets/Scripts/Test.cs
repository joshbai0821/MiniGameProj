using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;

public class Test : MonoBehaviour {

    // Use this for initialization
    void Start () {

        Application.targetFrameRate = 20;
	}
	
	// Update is called once per frame
	void Update () {
        // Debug.Log("Update: " + Time.fixedDeltaTime);
	}

    void FixedUpdate()
    {
        // UnityEngine.Debug.Log(watch.ElapsedMilliseconds);
        Debug.Log("Fixed Update: " + GetCurrentMilliSecond());
    }

    long GetCurrentMilliSecond()
    {
        //获取当前Ticks
        long currentTicks = DateTime.Now.Ticks;
        DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long currentMillis = (currentTicks - dtFrom.Ticks) / 10000;
        return currentMillis;
    }
}
