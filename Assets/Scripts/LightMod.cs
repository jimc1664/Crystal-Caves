using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMod : MonoBehaviour {

    public float LightLevel = 0.4f, SmoothLL;

	void Start () {
        SmoothLL = LightLevel;
	}
	
	// Update is called once per frame

    void FixedUpdate() {
        SmoothLL = Mathf.Lerp(SmoothLL, LightLevel, Time.deltaTime * 3);
    }
	void Update () {


        LightLevel -= Time.deltaTime /(5 -LightLevel*3)  *0.35f;
        if(LightLevel > 1)
            LightLevel = 1;
        if(LightLevel < 0)
            LightLevel = 0;

        GetComponent<Camera>().farClipPlane = 10 + SmoothLL*40;
	}
}
