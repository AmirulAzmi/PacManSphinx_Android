using UnityEngine;
using System.Collections.Generic;

public enum GhostName { Blinky, Pinky, Inky, Clyde }

public enum GhostState { Dead, Wander, Scatter, Chase, Scared }

public class GhostController : MonoBehaviour
{
    public GhostName persona;

    [System.NonSerialized]
    public float moveSpeed = 1.5f;

    // movement variables 
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 0.5f; // look ahead 1 tile

    // states 
    private GhostState m_state;
    private GhostState m_lastState;

    // targets 	
    private GameObject m_target;
    private GameObject m_homeTarget;
    private GameObject m_scatterTarget;

    // audio 
    private AudioSource m_deathSound;

    // manage color based on state
    private Color m_ghostColor;
    private Color m_scaredColor = new Color32(50, 100, 255, 204);
    private Renderer m_renderer;
    private float m_blinkTimer = 0.0f;
    private bool m_altColor = false;
    private float m_scaredTimer = 0.0f;
    private float m_chaseTimer = 0.0f;
    public float m_chaseTime = 10.0f;

    private Vector3 initial;

    private GameManager gm;
    private Level m_levelScript; // Reference to the instanced Level script
    private PlayerController m_pacMan; // Reference to the instanced Player script (used for targeting)

    void Start()
    {

        gm = GameObject.Find("Managers").GetComponent<GameManager>();
        if (gm.m_level > 1)
        {
            moveSpeed += gm.m_level * 0.2f;
            m_chaseTime += gm.m_level * 0.5f;
        }
        m_dest = transform.position;
        m_state = GhostState.Scatter;
        m_lastState = m_state;
        m_renderer = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        m_deathSound = GetComponent<AudioSource>();

        if (m_target == null)
        {
            m_target = new GameObject();
            m_target.name = "Target";
        }

        if (m_homeTarget == null)
        {
            m_homeTarget = new GameObject();
            m_homeTarget.name = "Home Target";
            m_homeTarget.transform.position = transform.position;
        }

        if (m_scatterTarget == null)
        {
            m_scatterTarget = new GameObject();
            m_scatterTarget.name = "Scatter Target";
        }

        switch (persona)
        {
            case GhostName.Blinky:
                m_ghostColor = new Color32(255, 50, 50, 204);
                m_scatterTarget.transform.position = new Vector3(24, 0, 30);
                break;
            case GhostName.Pinky:
                m_ghostColor = new Color32(245, 100, 245, 204);
                m_scatterTarget.transform.position = new Vector3(3, 0, 30);
                break;
            case GhostName.Inky:
                m_ghostColor = new Color32(73, 179, 219, 204);
                m_scatterTarget.transform.position = new Vector3(24, 0, 2);
                break;
            case GhostName.Clyde:
                m_ghostColor = new Color32(255, 132, 0, 204);
                m_scatterTarget.transform.position = new Vector3(3, 0, 2);
                break;
        }
        m_renderer.material.color = m_ghostColor;

        // Pathfinding
        //m_pathToTarget = m_pathfinder.findBestPath(m_level.getPathMap(), m_pos, m_pacMan.pos);
    }

    void Update()
    {
        if (gm.m_isStarted == true)
        {
            switch (m_state)
            {
                case GhostState.Dead:
                    GoHome();
                    FollowPath();
                    break;
                case GhostState.Scatter:
                    Scatter();
                    FollowPath();
                    break;
                case GhostState.Wander:
                    GetComponent<Pathfinding>().setTarget(m_scatterTarget.transform);
                    Wander();
                    break;
                case GhostState.Chase:
                    GetComponent<Pathfinding>().setTarget(m_pacMan.transform);
                    m_chaseTimer += Time.deltaTime;
                    if (m_chaseTimer > m_chaseTime)
                    {
                        m_state = m_lastState;
                        m_chaseTimer = 0;
                    }
                    FollowPath();
                    break;
                case GhostState.Scared:
                    m_scaredTimer += Time.deltaTime;
                    m_blinkTimer += Time.deltaTime;
                    if (m_blinkTimer > .2f)
                    {
                        m_altColor = !m_altColor;
                        m_renderer.material.color = (m_altColor) ? Color.white : m_scaredColor; //Color.blue;
                        m_blinkTimer = 0;
                    }
                    if (m_scaredTimer > 10.0f)
                    {
                        m_renderer.material.color = m_ghostColor;
                        m_state = m_lastState;
                        if (m_state == GhostState.Scatter)
                        {
                            FollowPath();
                        }
                        m_scaredTimer = 0;
                    }
                    Wander(); // switch to wander after being scared 

                    break;
                default:
                    Debug.LogWarning("Gost Controller doesn't know what state it's in");
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "player")
        {
            if (m_state == GhostState.Scared)
            {
                // Ghost dies
                gm.addScore(100);
                m_deathSound.Play();
                m_state = GhostState.Dead;
                m_renderer.enabled = false;

            }
            else if (m_state == GhostState.Dead) { }

            else
            {
                // PacMan dies
                if (other.GetComponent<PlayerController>().m_state != PlayerState.Dead)
                {
                    other.GetComponent<PlayerController>().m_state = PlayerState.Dead;
                    other.GetComponent<Animator>().SetBool("isDead", true);
                    gm.minusLife();
                }
            }
        }
    }


    void GoHome() { GetComponent<Pathfinding>().setTarget(m_homeTarget.transform); }
    void Scatter() { GetComponent<Pathfinding>().setTarget(m_scatterTarget.transform); }
    void Chase() { }
    void Wander()
    {
        //move closer to destination
        Vector3 p = Vector3.MoveTowards(transform.position, m_dest, moveSpeed * Time.deltaTime);
        GetComponent<Rigidbody>().MovePosition(p);


        Vector3[] choices = { Vector3.right, -Vector3.right, Vector3.forward, -Vector3.forward };
        int randomIndex;

        Vector3 reversed = Vector3.zero;
        if (!Valid(m_nextDir))
        {
            do
            {
                randomIndex = Random.Range(0, choices.Length);
            } while (choices[randomIndex] == reversed);

            m_nextDir = choices[randomIndex];

            if (m_nextDir == Vector3.forward)
            {
                reversed = -Vector3.forward;
            }
            else if (m_nextDir == -Vector3.forward)
            {
                reversed = Vector3.forward;
            }
            else if (m_nextDir == Vector3.right)
            {
                reversed = -Vector3.right;
            }
            else
            {
                reversed = Vector3.right;
            }
        }

        if (Vector3.Distance(m_dest, transform.position) < 0.00001f)
        {
            if (Valid(m_nextDir))
            {
                m_dest = (Vector3)transform.position + m_nextDir;
                m_dir = m_nextDir;
                //TODO: rethink the following
            }
            else
            {   // nextDir NOT valid
                if (Valid(m_dir))
                {  // and the prev. direction is valid
                    m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
                }
                // otherwise, do nothing ?
            }
        }
        transform.LookAt(m_dest);

    }


    void FollowPath()
    {

        List<Node> path = GetComponent<Pathfinding>().m_path;

        if (path != null)
        {
            Vector3 p = Vector3.MoveTowards(transform.position, m_dest, moveSpeed * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(p);

            if (path.Count > 0)
            {
                m_nextDir = path[0].m_worldPos;
                transform.LookAt(m_nextDir);
                m_nextDir = transform.forward;
                path.RemoveAt(0);
            }
            else
            { // target reached
                if (m_state == GhostState.Dead)
                {
                    m_renderer.enabled = true;
                    m_renderer.material.color = m_ghostColor;
                    m_state = GhostState.Scatter;
                }
                else if (m_state == GhostState.Scatter)
                {
                    m_state = GhostState.Chase;
                }

            }

            if (Vector3.Distance(m_dest, transform.position) < 0.00001f)
            {

                if (Valid(m_nextDir))
                {
                    m_dest = (Vector3)transform.position + m_nextDir;
                    m_dir = m_nextDir;
                    //TODO: rethink the following
                }
                else
                {   // nextDir NOT valid
                    if (Valid(m_dir))
                    {  // and the prev. direction is valid
                        m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
                    }
                    // otherwise, do nothing ?
                }
            }
            transform.LookAt(m_dest);
        }
    }

    bool Valid(Vector3 direction)
    {
        bool retVal = false;

        // cast line from 'next to pacman' to pacman not from directly the center of next tile but just a little further from center of next tile
        Vector3 pos = transform.position;
        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit;
        Physics.Linecast(pos + direction, pos, out hit);

        if (hit.collider != null)
        {
            retVal = (hit.collider.name == "player") ||
                (hit.collider.name == "door") ||
                (hit.collider.name == "fruit") ||
                (hit.collider.tag == "Ghost") ||
                (hit.collider.name == "energizer") ||
                (hit.collider.name == "dot") ||
                (hit.collider == GetComponent<Collider>());
        }
        return retVal;
    }

    public void Init(Vector3 init)
    {
        initial = init;
    }
    public void reset()
    {
        if (gm.m_life > 0)
        {
            m_renderer.enabled = true;
            m_renderer.material.color = m_ghostColor;
            this.gameObject.transform.position = initial;
            this.gameObject.transform.rotation = Quaternion.identity;
            m_dest = initial;
            m_dir = Vector3.zero;
            m_nextDir = Vector3.zero;
            m_state = GhostState.Scatter;
            m_lastState = m_state;
        }
        else Destroy(gameObject);
    }

    public void setLevel(Level lvl) { m_levelScript = lvl; }
    public Color getColor() { return m_ghostColor; }
    public void setPacMan(PlayerController player) { m_pacMan = player; }
    public PlayerController getPacMan() { return m_pacMan; }

    public void setState(GhostState newState)
    {
        if ((m_state != GhostState.Dead))
        {
            m_state = newState;
        }

        if (m_state == GhostState.Scared)
        {
            m_renderer.material.color = Color.blue;
            m_scaredTimer = 0;
        }
    }

}
