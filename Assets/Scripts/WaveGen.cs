using UnityEngine;
using System.Collections;

public class WaveGen : MonoBehaviour {

    public GameObject WaveFab;

    public Vector3 Vel;

    Rigidbody Bdy;
    Vector3 LPos;

    public float Hp  =1, MaxHp = 25;
	// Use this for initialization
	void Start () {
        Hp = MaxHp;
        Bdy = GetComponentInParent<Rigidbody>();
        LPos = Bdy.position;
	}

    void FixedUpdate() {
        var vec = Bdy.position - LPos;
        LPos = Bdy.position;
        vec /= Time.deltaTime;
        Vel = vec;
    }

    float FootStepTimer;
	// Update is called once per frame

    public void makeWave( float r, Vector3 at ) {
        var go = Instantiate(WaveFab) as GameObject;
        go.GetComponent<Wave>().init(r, r /26.0f  * 1.8f, at );

        GetComponent<LightMod>().LightLevel +=  r /26.0f    *0.2f;
    }

    public float Stamina = 5;
	void Update () {

    

        if( Hp < 0 ) {
            var cc=GetComponentInParent< UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
            cc.m_WalkSpeed = cc.m_RunSpeed = 0;
            FindObjectOfType<Level>().delayedRestart();
            var audio = GetComponent<AudioSource>();
            //audio.pitch *= Random.Range(0.8f, 1.2f);
            audio.Play();
            Destroy( this );
        }

        Hp += Time.deltaTime*0.8f;
        if(Hp >MaxHp)
            Hp = MaxHp;

        var fps = Bdy.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        if(fps.m_IsWalking) {
            Stamina += Time.deltaTime *0.5f;
            if(Stamina > 5)
                Stamina = 5;
        } else {
            Stamina -= Time.deltaTime;
            if(Stamina <-0.5f)
                Stamina = -0.5f;
        }
        
        float maxRun =  Mathf.Lerp(fps.m_WalkSpeed, 9.0f, Stamina *0.333f );
        fps.m_RunSpeed = Mathf.Lerp(fps.m_RunSpeed, fps.m_IsWalking ? fps.m_WalkSpeed : maxRun, 3*Time.deltaTime);
            //fps.m_WalkSpeed + (rs -fps.m_WalkSpeed) *Mathf.Min(Stamina*0.5f, 1);

        FootStepTimer -= Time.deltaTime;
        var vm = Vel.magnitude;
        if(FootStepTimer < 0 && vm > 0.5f) {
            makeWave(2 + Mathf.Pow( vm, 0.9f )*2.0f , transform.position +Vel*0.4f  );
            FootStepTimer = 0.4f;
        }

        if(Input.GetKeyUp(KeyCode.Q) || Input.GetMouseButtonUp(0) ) {
            makeWave(32, transform.position );

        }

       
	}
}
