using UnityEngine;
using System.Collections.Generic;

public class WaveTrigger : MonoBehaviour {

    public SphereCollider Col;

	void Awake () {
        Col = GetComponent<SphereCollider>();
	}


//    SortedDictionary<>

    void OnTriggerStay( Collider c ) {

        var bot = c.GetComponent<Bot>();
        if(bot == null) return;
        float noise = 0.4f;
        Vector3 p1 = transform.position, p2 = bot.Bdy.position + Random.insideUnitSphere*noise;
        var vec =p2- p1;
        var mag =  vec.magnitude;
        bool hit = Physics.Raycast(p1, vec.normalized, mag, 2);
        Color col = Color.green;
        if(hit && (mag*=2) > Col.radius) {
            col = Color.red;
        } else {
            float mod = 1/ vec.magnitude;
            bot.HearPosAcum += p1 *mod;
            bot.HearMod += mod;
            bot.Awareness += Time.deltaTime / mag * 40;
        }
        Debug.DrawLine(p1, p2, col);
    }

}
