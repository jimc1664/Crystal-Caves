using UnityEngine;
using System.Collections;

public class Wave : MonoBehaviour {

    public GameObject TrigFab, BigTrigFab;
    public float Timer= 2;
    public float Range = 1;

	// Use this for initialization

    public Material Mat;
    public Color Col;


    public WaveTrigger Trig;

    float MaxTimer;
	void Start () {

      
	}

    public void init( float range, float tm, Vector3 at ) {

        transform.SetParent(Camera.main.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        var p = GetComponent<Projector>();

        Mat = p.material = Instantiate(p.material) as Material;
        Vector4 sp = at;
        sp.w = Time.time;
        Mat.SetVector("_SrcPos", sp);

        Mat.SetFloat("_MaxTime", MaxTimer = Timer = tm);

        var fab = TrigFab;
        if(range > 23)
            fab = BigTrigFab;
        Trig = (Instantiate(fab, at, Quaternion.identity) as GameObject).GetComponent<WaveTrigger>();

        Mat.SetFloat("_Range", Range= range);

    }
	// Update is called once per frame
	void Update () {
        if((Timer-= Time.deltaTime) < 0) {
            Destroy(gameObject);
            Destroy(Trig.gameObject);
            return;
        }


        Vector4 sp = Vector4.zero;
        sp.y = Time.time;
        Mat.SetVector("_Time2", sp);

        Trig.Col.radius = Range * (1-Timer / MaxTimer) +1.0f;
	}

 
}
