using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalThing : MonoBehaviour {

    public Transform EnemyPool;
    public GameObject MapCover;
    public Material OffMat;

    void OnEnable() {
        Count++;
    }
    void OnDisable() {
        Count--;
    }
    static int Count = 0;

    void OnTriggerEnter() {

        Destroy(transform.parent.GetComponent<AudioSource>());

        foreach( var mr in transform.parent.GetComponentsInChildren<MeshRenderer>() )
            mr.material = OffMat;

        if(Count <= 1) {
            FindObjectOfType<Level>().lastCrystal();
        }
        for(int i = 5 - Count; i-- >0; ) {
            if(EnemyPool.childCount <= 0) break;
            EnemyPool.GetChild(Random.Range(0, EnemyPool.childCount)).SetParent(null);
        }

        var wg = FindObjectOfType<WaveGen>();
        if(wg != null)
            wg.makeWave(30, transform.position);
            
        Destroy(MapCover);
        Destroy(gameObject);


    }
}
