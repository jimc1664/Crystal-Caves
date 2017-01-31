using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bot : AIPath {

    public Transform Points;

    public float  Accel = 5;
    [HideInInspector] public Rigidbody Bdy;
   // Seeker Seek;
    Transform Target {
        get { return target;  }
        set { target = value; }
    }
    Animator Anim;
	void Start () {
        base.Start();
        Bdy = GetComponent<Rigidbody>();

        GetComponentInChildren<AudioSource>().pitch *= Random.Range(0.5f, 2.0f);
      //  Seek = GetComponent<Seeker>();

      //  Seek.pathCallback = gotPath;

        TargetTime = 0;

        Anim = GetComponentInChildren<Animator>();
	}
    /*

    Pathfinding.Path CurPath, ReqPath;
    void gotPath( Pathfinding.Path p ) {
       
    }*/


  


   // Transform Target;


    public float Awareness = 0, HearMod =0;

    public Vector3 HearPosAcum, HearPos;


    public float WanderSpeed = 3, InvestigateSpeed = 4, SeekSpeed = 6;

    public float AttackTimer =0;

    public bool DoDamage = false;

    float RunTimer = 0;


    public override void Update() {

        float lAt = AttackTimer;
        AttackTimer -= Time.deltaTime;
        RunTimer -= Time.deltaTime;
        if(RunTimer < 0)
            RunTimer = 0;


        float maxA = 3;
        if(Awareness > maxA) Awareness = maxA;


        if(Player != null) {
            if(XZSqrMagnitude(Player.position, Bdy.position)  < 15 + Awareness*15  ) {
                Awareness += Time.deltaTime *1.5f;
                HearPosAcum += Player.position +Player.velocity*0.75f;
                HearMod++;
            }
        }

        Awareness -= Time.deltaTime;

        if(HearMod > 0) {
            HearPos = HearPosAcum / HearMod;
            HearPosAcum = Vector3.zero;
            HearMod = 0;
        }
       // State = Awareness > 0.5f ? BotState.Investigate : BotState.Wander;
        TargetTime -=Time.deltaTime;


        switch(State) {
            case BotState.Wander:
                if(Awareness > 0.5f) {
                    State = BotState.Investigate;
                    Target = null;
                } else if(Awareness < 0)
                    Awareness = 0;

                speed = WanderSpeed * (1- Awareness);
                if(Target == null ) {
                    //Target = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().transform;

                    if(TargetTime < -1) {

                        if(Player == null) {
                            var fps = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
                            if(fps)
                                Player = fps.GetComponent<Rigidbody>();
                        }

                        if(Random.value > 0.65f && Player !=  null) {
                            Target = Player.transform;
                            //  Seek.StartPath(transform.position, Target.position);
                            TargetTime = Random.Range(5, 9)*2;
                        } else {

                            Target = Points.GetChild(Random.Range(0, Points.childCount));
                            //  Seek.StartPath(transform.position, Target.position);
                            TargetTime = Random.Range(6, 18)*20;
                        }
                    }
                }

                if(Target != null ) {
                    TargetPos = Target.position;
                    if(XZSqrMagnitude(TargetPos, Bdy.position) < endReachedDistance* endReachedDistance  
                        || (TargetTime) < 0) {
                        Target = null;
                        TargetTime = Random.Range(-0.8f, 0);
                    }
                }
                break;
            case BotState.Investigate:
                speed = InvestigateSpeed;

                if(Player != null  && XZSqrMagnitude(Player.position, Bdy.position)  < 10 + Awareness*15  && RunTimer < 10) {
                    speed *= 1.8f;
                    RunTimer += Time.deltaTime *3;
                }
                TargetPos = HearPos;
                if(Player == null) {
                    var fps = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
                    if(fps)
                        Player =GetComponent<Rigidbody>();
                } else {
                    if(XZSqrMagnitude(Player.position, Bdy.position)  < 11) {
                        ///die  
                        if(AttackTimer < 0) {
                            Anim.SetTrigger("Attack");
                            var audio = Anim.GetComponent<AudioSource>();
                            audio.pitch *= Random.Range(0.8f, 1.2f);
                            audio.Play();
                            AttackTimer = 3;
                            DoDamage = true;
                        } 
                      
                    }
                }

                if(AttackTimer > 0) {
                    if(AttackTimer > 1.5) {
                        speed = InvestigateSpeed*2.5f;
                    } else if(AttackTimer > 0.7) 
                             speed = InvestigateSpeed*0.5f;

                    if(AttackTimer < 2.3f) {
                        if(AttackTimer > 1) {

                            if(DoDamage && Player != null  && XZSqrMagnitude(Player.position, Bdy.position)  < 15) {
                                DoDamage = false;
                                var wg = Player.GetComponentInChildren<WaveGen>();
                                if(wg)
                                    wg.Hp -= 22;
                            }
                        } else
                            DoDamage = false;
                    }                    
                  
                } else if(Awareness < -2.5f || (Awareness < 0 &&  XZSqrMagnitude(TargetPos, Bdy.position) < endReachedDistance* endReachedDistance)) {
                    State = BotState.Wander;
                    Awareness = 0;
                    TargetTime = Random.Range(-0.8f, -0.3f);
                    
                }
                break;
        }

        var v = Bdy.velocity; v.y = 0;
        Anim.SetFloat("Speed", v.magnitude / InvestigateSpeed);
    }

    protected Vector3 CalculateVelocity2( Vector3 currentPosition ) {
        if(path == null || path.vectorPath == null || path.vectorPath.Count == 0) return Vector3.zero;

        List<Vector3> vPath = path.vectorPath;

        if(vPath.Count == 1) {
            vPath.Insert(0, currentPosition);
        }

        if(currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count-1; }

        if(currentWaypointIndex <= 1) currentWaypointIndex = 1;

        while(true) {
            if(currentWaypointIndex < vPath.Count-1) {
                //There is a "next path segment"
                float dist = XZSqrMagnitude(vPath[currentWaypointIndex], currentPosition);
                //Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
                if(dist < pickNextWaypointDist*pickNextWaypointDist) {
                    lastFoundWaypointPosition = currentPosition;
                    lastFoundWaypointTime = Time.time;
                    currentWaypointIndex++;
                } else {
                    break;
                }
            } else {
                break;
            }
        }

        Vector3 dir = vPath[currentWaypointIndex] - vPath[currentWaypointIndex-1];
        Vector3 targetPosition = CalculateTargetPoint(currentPosition, vPath[currentWaypointIndex-1], vPath[currentWaypointIndex]);


        dir = targetPosition-currentPosition;

        
        return dir;


        dir.y = 0;
        float targetDist = dir.magnitude;

        float slowdown = Mathf.Clamp01(targetDist / slowdownDistance);

        this.targetDirection = dir;
        this.targetPoint = targetPosition;

        if(currentWaypointIndex == vPath.Count-1 && targetDist <= endReachedDistance) {
            if(!targetReached) { targetReached = true; OnTargetReached(); }

            //Send a move request, this ensures gravity is applied
            return Vector3.zero;
        }

        Vector3 forward = tr.forward;
        float dot = Vector3.Dot(dir.normalized, forward);
        float sp = speed * Mathf.Max(dot, minMoveScale) * slowdown;


        if(Time.deltaTime > 0) {
            sp = Mathf.Clamp(sp, 0, targetDist/(Time.deltaTime*2));
        }
        return forward*sp;
    }

    float TargetTime;

    static Rigidbody Player;

    public enum BotState {
        Wander,
        Investigate, 
        Seek,
    }
    public BotState State = BotState.Wander;
    void FixedUpdate() {

      
  
        Vector3 DesVel = Vector3.zero;

        DesVel = CalculateVelocity( Bdy.position );

        DesVel.y = 0;

        if(AttackTimer >0.5f && Player != null) {
            targetDirection = Player.position+Bdy.velocity*0.2f - Bdy.position;
            targetDirection.y = 0;
            if(targetDirection.sqrMagnitude > 0.1f)
                 targetDirection.Normalize();
        }
        if(targetDirection.sqrMagnitude > 0.1f) {

            Bdy.MoveRotation(Quaternion.Lerp(Bdy.rotation, Quaternion.LookRotation(targetDirection), 8*Time.deltaTime));
        }
        Debug.DrawLine(Bdy.position, Bdy.position + DesVel*10, Color.blue);
       // RotateTowards(DesVel);
      /*  if(DesVel.sqrMagnitude > Speed *Speed ) {
            DesVel *= Speed / DesVel.magnitude;
        }*/


       

        DesVel.y = Bdy.velocity.y;
        Bdy.velocity = Vector3.Lerp(Bdy.velocity, DesVel, Accel *Time.deltaTime);
	}

    void OnDrawGizmos() {

        Gizmos.DrawLine(transform.position, HearPos);
    }
}
