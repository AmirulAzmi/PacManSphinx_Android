using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState { Dead, Normal, Super }

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    public Animator anim;
    private GameManager gm;
    private PocketSphinxMobile ps;
    private FingerSwipe fs;

    public string movement;

    public float m_speed = 5f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 1f; // look ahead 1 tile

    public PlayerState m_state = PlayerState.Normal;


    void Start() {
        //initialTransform = this.gameObject.transform;
        m_dest = transform.position;
        anim = GetComponent<Animator>();
        gm = GameObject.Find("Managers").GetComponent<GameManager>();
        ps = GameObject.Find("MovementInput").GetComponent<PocketSphinxMobile>();
        fs = GameObject.Find("MovementInput").GetComponent<FingerSwipe>();
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (gm.m_isStarted) {
            StartCoroutine(ReadInputAndMove());
            if (!gm.useSpeech) {
                #if UNITY_EDITOR
                keyboard();
                #else
                movement = fs.detected; 
                #endif
            }
            else { movement = ps.detected; }
        }
    }

    IEnumerator ReadInputAndMove() {

        // move closer to destination
        rb.MovePosition(Vector3.MoveTowards(transform.position, m_dest, m_speed * Time.deltaTime));
        if (movement == "right") { m_nextDir = Vector3.right; }
        if (movement == "left") { m_nextDir = -Vector3.right; }
        if (movement == "up") { m_nextDir = Vector3.forward; }
        if (movement == "down") { m_nextDir = -Vector3.forward; }

        //------------

        if (Vector3.Distance(m_dest, transform.position) < 0.0001f) {
            if (Valid(m_nextDir)) { 
                m_dest = (Vector3)transform.position + m_nextDir;
                m_dir = m_nextDir;
                anim.SetBool("isMoving",true);
                //TODO: rethink the following
            } else {   // nextDir NOT valid
                if (Valid(m_dir))
                {  // and the prev. direction is valid
                    m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
                }
                else { anim.SetBool("isMoving",false); }// otherwise, do nothing ?
            }
        }
        transform.LookAt(m_dest);
        yield return null;
    }

     bool Valid(Vector3 direction) {
        // cast line from 'next to pacman' to pacman 
        Vector3 pos = transform.position;
        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit; 
        Physics.Linecast(pos + direction, pos, out hit);
        
        if(hit.collider != null) {
            return (hit.collider.tag == "Ghost") || 
                (hit.collider.name == "warp") || 
                (hit.collider.name == "fruit") || 
                (hit.collider.name == "energizer") || 
                (hit.collider.name == "dot") || 
                (hit.collider == GetComponent<Collider>());
        } 
        else { return false; }
    }

     public void reset() {
         if (gm.m_life > 0)
         {
             anim.SetBool("isDead", false);
             anim.SetBool("isMoving", false);
             this.gameObject.transform.position = new Vector3(13, this.gameObject.transform.position.y, 8);
             setDest(new Vector3(13, this.gameObject.transform.position.y, 8));
             this.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
             m_dir = Vector3.zero;
             m_nextDir = Vector3.zero;
             m_state = PlayerState.Normal;
         }
         else Destroy(gameObject);
     }
    // accessors & mutators 
    public Vector3 getDir() { return m_dir; }
    public void setDir(Vector3 newDir) { m_dir = newDir; }
    public Vector3 getDest() { return m_dest; }
    public void setDest(Vector3 newDest){ m_dest = newDest; }
    
    private void keyboard(){
        if (Input.GetAxis("Horizontal") > 0) { movement = "right"; }
        if (Input.GetAxis("Horizontal") < 0) { movement = "left"; }
        if (Input.GetAxis("Vertical") > 0) { movement = "up"; }
        if (Input.GetAxis("Vertical") < 0) { movement = "down"; }
    }
}
