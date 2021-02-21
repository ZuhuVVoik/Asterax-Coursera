#define DEBUG_PlayerShip_RespawnNotifications

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    // This is a somewhat protected private singleton for PlayerShip
    static private PlayerShip   _S;
    static public PlayerShip    S
    {
        get
        {
            return _S;
        }
        private set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to set PlayerShip singleton _S.");
            }
            _S = value;
        }
    }

    [Header("Set in Inspector")]
    public float        shipSpeed = 10f;
    public GameObject   bulletPrefab;

    Rigidbody           rigid;


    void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
        ui = FindObjectOfType<UIHandler>();
        startPos = this.transform.position;
    }


    void Update()
    {
        // Using Horizontal and Vertical axes to set velocity
        float aX = CrossPlatformInputManager.GetAxis("Horizontal");
        float aY = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 vel = new Vector3(aX, aY);
        if (vel.magnitude > 1)
        {
            // Avoid speed multiplying by 1.414 when moving at a diagonal
            vel.Normalize();
        }

        rigid.velocity = vel * shipSpeed;

        // Mouse input for firing
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }


    void Fire()
    {
        // Get direction to the mouse
        Vector3 mPos = Input.mousePosition;
        mPos.z = -Camera.main.transform.position.z;
        Vector3 mPos3D = Camera.main.ScreenToWorldPoint(mPos);

        // Instantiate the Bullet and set its direction
        GameObject go = Instantiate<GameObject>(bulletPrefab);
        go.transform.position = transform.position;
        go.transform.LookAt(mPos3D);
    }

    static public float MAX_SPEED
    {
        get
        {
            return S.shipSpeed;
        }
    }
    
	static public Vector3 POSITION
    {
        get
        {
            return S.transform.position;
        }
    }






    /* The code for task */
    private void OnCollisionEnter(Collision collision)
    {
        /* If collision object is not a player, add scores */
        if (collision.gameObject.tag == "Asteroid" && canBeHitted)
        {
            OnAsteroidHit();
        }
    }

    public void JumpToSafeLocation()
    {
        StartCoroutine(WaitForJump());
    }
    IEnumerator WaitForJump()
    {
        canBeHitted = false;
        this.gameObject.transform.position = new Vector3(-this.gameObject.transform.position.x, -this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        yield return new WaitForSeconds(1f);
        canBeHitted = true;
    }



    public static UIHandler ui;

    bool canBeHitted = true;
    static int jumps = 3;
    public int Jumps
    {
        get { return jumps; }
        set
        {
            jumps = value;
            ui.RefreshJumpsCount(value);
            if(jumps < 0)
            {
                this.GameOver();
            }
        }
    }

    int scores = 0;
    public int Scores
    {
        get { return scores; }
        set
        {
            scores = value;
            ui.RefreshScoreCount(value);
        }
    }
    /* Start position*/
    public static Vector3 startPos;
    /* Scorecounts for asteroids */
    public static int bigAsteroidScores = 500;
    public static int mediumAsteroidScores = 300;
    public static int smallAsteroidScores = 100;

    public void OnAsteroidHit()
    {
        Jumps--;
        JumpToSafeLocation();
    }

    /* Adding scores. Count is different for asteroid sizes */
    public void AddScores(int asteroidSize)
    {
        if (asteroidSize == 3)
        {
            Scores += bigAsteroidScores;
        }
        if (asteroidSize == 2)
        {
            Scores += mediumAsteroidScores;
        }
        if (asteroidSize == 1)
        {
            Scores += smallAsteroidScores;
        }
    }

    /* If no jumps left */
    public void GameOver()
    {
        ui.ToggleGameOverScreen();
        StartCoroutine(WaitForRestart());
    }
    public void ResetData()
    {
        this.transform.position = startPos;
        Jumps = 3;
        Scores = 0;
        AsteraX asteraX = FindObjectOfType<AsteraX>();
        asteraX.DestroyAsteroids();
        asteraX.SpawnAsteroids();
        
    }
    IEnumerator WaitForRestart()
    {
        yield return new WaitForSeconds(3f);
        ResetData();
        ui.ToggleGameOverScreen();
    }
}
