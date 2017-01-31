using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioKiller : MonoBehaviour {

    AudioSource Src;
	// Use this for initialization
	void Start () {
        Src = GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
        if (!Src.isPlaying)
            Destroy(gameObject);
	}
}
