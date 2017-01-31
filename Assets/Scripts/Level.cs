using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {


    public GameObject PlayerFab, CurrentPlayer;
    public GameObject Door;

    void Start()
    {
        spawn();
        Time.timeScale = 1;
    }
    public void lastCrystal() {
        GetComponent<AudioSource>().Play();
        foreach(var b in FindObjectsOfType<Bot>()) {
            Destroy(b.gameObject);
        }

        delayedRestart();
    }
    public void delayedRestart() {
        Time.timeScale = 0.3f;
        Invoke("restart", 6.0f * Time.timeScale);
    }

    public void restart() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0 );
    }
    public void spawn() {
        //CancelInvoke("restart");

        if(CurrentPlayer)
            Destroy(CurrentPlayer);
        var spawn = transform.GetChild( Random.Range(0, transform.childCount));
        var waves = FindObjectsOfType<Wave>();
        foreach(var w in waves) {
            w.transform.SetParent(null);
        }
        CurrentPlayer = Instantiate(PlayerFab, spawn.position, spawn.rotation );

        var cam = CurrentPlayer.GetComponentInChildren<Camera>().transform;
        foreach(var w in waves) {
            w.transform.SetParent(cam );
            w.transform.localPosition = Vector3.zero;
            w.transform.localRotation = Quaternion.identity;
        }
    }

    void Update() {
        if(Input.GetKeyUp(KeyCode.F5)) {
            restart();
        }
        if(Input.GetKeyUp(KeyCode.F3)) {
            if(Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }
    }
}
