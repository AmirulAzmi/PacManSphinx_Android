using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{

    /* 
        Level is: 28 tiles wide (c/x/w) and 31 tiles high (r/z/d) - Original Game
        (screen is 36 tiles high) 	- 3 @ top for score/highscore 
                                    - 2 @ bottom for lives and fruit collected
									 
        a - upper right corner 					B - Blinky spawn 
        b - bottom right corner 				P - Pinky spawn 
        c - bottom left corner  				I - Inky spawn
        d - upper left corner 					C - Clyde spawn 
        w - warp (matches opposite w)           S - PacMan and Fruit spawn
        x - horozantal wall 
        y - vertical wall						. - dot 
        z - ghost house doors 					* - energizer
		
        d x x x x x x x x x x x x a d x x x x x x x x x x x x a
        y . . . . . . . . . . . . y y . . . . . . . . . . . . y
        y . d x x a . d x x x a . y y . d x x x a . d x x a . y 
        y * y     y . y       y . y y . y       y . y     y * y
        y . c x x b . c x x x b . c b . c x x x b . c x x b . y
        y . . . . . . . . . . . . . . . . . . . . . . . . . . y
        y . d x x a . d a . d x x x x x x a . d a . d x x a . y
        y . c y y b . y y . c x x a d x x b . y y . c x x b . y
        y . . . . . . y y . . . . y y . . . . y y . . . . . . y
        c x x x x a . y c x x a   y y   d x x b y . d x x x x b
                  y . y d x x b   c b   c x x a y . y
                  y . y y                     y y . y
                  y . y y   d x x z z x x a   y y . y
        x x x x x b . c b   y             y   c b . c x x x x x 
        w           .       y   B P I C   y       .           w  
        x x x x x a . d a   y             y   d a . d x x x x x 
                  y . y y   c x x x x x x b   y y . y
                  y . y y                     y y . y
                  y . y y   d x x x x x x a   y y . y 
        d x x x x b . c b   c x x a d x x b   c b . c x x x x a
        y . . . . . . . . . . . . y y . . . . . . . . . . . . y
        y . d x x a . d x x x a . y y . d x x x a . d x x a . y
        y . c x a y . c x x x b . c b . c x x x b . y d x b . y
        y * . . y y . . . . . . . S   . . . . . . . y y . . * y
        c x a . y y . d a . d x x x x x x a . d a . y y . d x b
        d x b . c b . y y . c x x a d x x b . y y . c b . c x a
        y . . . . . . y y . . . . y y . . . . y y . . . . . . y
        y . d x x x x b c x x a . y y . d x x b c x x x x a . y
        y . c x x x x x x x x b . c b . c x x x x x x x x b . y
        y . . . . . . . . . . . . . . . . . . . . . . . . . . y
        c x x x x x x x x x x x x x x x x x x x x x x x x x x b
    */

    // pathfinding
    public int MaxSize { get { return m_levelDepth * m_levelWidth; } }
    public bool showPathfindingGrid = true;
    public bool wireframe = false;
    private Node[,] m_grid;
    public float m_nodeDiameter = 1f;

    private const int m_levelDepth = 33; // rows 
    private const int m_levelWidth = 28; // columns 
    private char[,] m_levelMap = new char[m_levelDepth, m_levelWidth] { 
			{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
			{ 'd','x','x','x','x','x','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','x','x','x','x','x','a'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
			{ 'y','*','y',' ',' ','y','.','y',' ',' ',' ','y','.','y','y','.','y',' ',' ',' ','y','.','y',' ',' ','y','*','y'}, 
			{ 'y','.','c','x','x','b','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','c','x','x','b','.','y'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','d','x','x','a','.','y'}, 
			{ 'y','.','c','x','x','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','x','x','b','.','y'}, 
			{ 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
			{ 'c','x','x','x','x','a','.','y','c','x','x','a',' ','y','y',' ','d','x','x','b','y','.','d','x','x','x','x','b'}, 
			{ ' ',' ',' ',' ',' ','y','.','y','d','x','x','b',' ','c','b',' ','c','x','x','a','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','z','z','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ 'x','x','x','x','x','b','.','c','b',' ','y',' ',' ',' ',' ',' ',' ','y',' ','c','b','.','c','x','x','x','x','x'}, 
			{ 'w',' ',' ',' ',' ',' ','.',' ',' ',' ','y',' ','B','P','I','C',' ','y',' ',' ',' ','.',' ',' ',' ',' ',' ','w'}, 
			{ 'x','x','x','x','x','a','.','d','a',' ','y',' ',' ',' ',' ',' ',' ','y',' ','d','a','.','d','x','x','x','x','x'}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','c','x','x','x','x','x','x','b',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','x','x','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ 'd','x','x','x','x','b','.','c','b',' ','c','x','x','a','d','x','x','b',' ','c','b','.','c','x','x','x','x','a'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
			{ 'y','.','c','x','a','y','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','y','d','x','b','.','y'}, 
			{ 'y','*','.','.','y','y','.','.','.','.','.','.','.','s',' ','.','.','.','.','.','.','.','y','y','.','.','*','y'}, 
			{ 'c','x','a','.','y','y','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','y','y','.','d','x','b'}, 
			{ 'd','x','b','.','c','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','b','.','c','x','a'}, 
			{ 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','x','x','b','c','x','x','a','.','y','y','.','d','x','x','b','c','x','x','x','x','a','.','y'},  
			{ 'y','.','c','x','x','x','x','x','x','x','x','b','.','c','b','.','c','x','x','x','x','x','x','x','x','b','.','y'},  
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'c','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','b'},
			{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}
	};

    //Objects
    public GameObject corner;
    public GameObject wall;
    public GameObject warp;
    public GameObject houseDoor;
    public GameObject dot;
    public GameObject energizer;
    public GameObject ghost;
    public GameObject player;
    public LayerMask unwalkableMask; // pathfinding

    //Container - to keep hierarchy neat
    public Transform levelContainer;
    private Transform m_ghostContainer;
    private Transform m_wallContainer;
    private Transform m_dotContainer;
    private Transform m_energizerContainer;
    private Transform m_portalContainer;
    private Transform m_doorContainer;

    //pathfinding
    public Transform m_pacMan;
    public Transform m_blinky;
    public Transform m_pinky;
    public Transform m_inky;
    public Transform m_clyde;

    private Renderer m_renderer;
    private List<GhostController> m_ghostList;
    private bool m_levelBuilt = false;
    private AudioSource bgSound;
    private GameManager gm;

    void Start()
    {
        gm = GetComponent<GameManager>();
        m_ghostList = new List<GhostController>();
        if (!m_levelBuilt)
        {
            BuildContainers();
            BuildLevel();
        }
    }

    void BuildContainers()
    {
        // setup the containers to keep things neat 
        //Create walls container
        m_wallContainer = new GameObject("Walls").transform; // create gameObject
        m_wallContainer.transform.parent = levelContainer; // let 'levelContainer become parent

        //Create dots container
        m_dotContainer = new GameObject("Dots").transform;
        m_dotContainer.transform.parent = levelContainer;

        //Create energizers container
        m_energizerContainer = new GameObject("Energizers").transform;
        m_energizerContainer.transform.parent = levelContainer;

        //Create ghost container
        m_ghostContainer = new GameObject("Ghosts").transform;
        m_ghostContainer.transform.parent = levelContainer;

        //Create Portals container
        m_portalContainer = new GameObject("Portals").transform;
        m_portalContainer.transform.parent = levelContainer;

        //Create doors container
        m_doorContainer = new GameObject("Doors").transform;
        m_doorContainer.transform.parent = levelContainer;
    }

    void BuildLevel()
    {

        Debug.Log("Building Level");

        GameObject tmp;
        bool singlePortal = false;
        GameObject unPairedWarp = null;
        //m_ghostList = new List<GameObject>();
        m_ghostList = new List<GhostController>();

        // pathfinding grid
        m_grid = new Node[m_levelDepth, m_levelWidth];

        // level 
        for (int r = 0, z = m_levelDepth - 1; r < m_levelDepth; r++, z--)
        {
            for (int c = 0, x = 0; c < m_levelWidth; c++, x++)
            {
                switch (m_levelMap[r, c])
                {
                    // Corners
                    case 'a': // top right
                        tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 180, 0)), m_wallContainer);
                        tmp.name = "corner " + x + "-" + z;
                        break;
                    case 'b': // bottom right
                        tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, -90, 0)), m_wallContainer);
                        tmp.name = "corner " + x + "-" + z;
                        break;
                    case 'c': // bottom left 
                        tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)), m_wallContainer);
                        tmp.name = "corner " + x + "-" + z;
                        break;
                    case 'd': // top left 
                        tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)), m_wallContainer);
                        tmp.name = "corner " + x + "-" + z;
                        break;

                    // Walls 
                    case 'x': // horizantal wall
                        tmp = Instantiate(wall, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)), m_wallContainer);
                        tmp.name = "wall " + x + "-" + z;
                        break;
                    case 'y': // vertical wall
                        tmp = Instantiate(wall, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)), m_wallContainer);
                        tmp.name = "wall " + x + "-" + z;
                        break;
                    case 'w': // warp
                        tmp = Instantiate(warp, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)), m_portalContainer);
                        tmp.name = "warp";

                        if (!singlePortal)
                        {
                            unPairedWarp = tmp;
                            singlePortal = true;
                        }
                        else
                        {
                            unPairedWarp.GetComponent<Warp>().setWarpMate(tmp);
                            unPairedWarp.GetComponent<Warp>().AllowWarp(true);
                            tmp.GetComponent<Warp>().setWarpMate(unPairedWarp);
                            tmp.GetComponent<Warp>().AllowWarp(true);
                            singlePortal = false;
                            unPairedWarp = null;
                        }
                        break;
                    case 'z': // doors to ghost house
                        tmp = Instantiate(houseDoor, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)), m_doorContainer);
                        tmp.name = "door";
                        break;
                    case '.':
                        tmp = Instantiate(dot, new Vector3(x, 0, z), Quaternion.identity, m_dotContainer);
                        tmp.name = "dot";
                        break;
                    case '*':
                        tmp = Instantiate(energizer, new Vector3(x, 0, z), Quaternion.identity, m_energizerContainer);
                        tmp.name = "energizer";
                        break;

                    // Ghosts 
                    case 'B':
                        tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity, m_ghostContainer);
                        GhostController blinkyTmp = tmp.GetComponent<GhostController>();
                        blinkyTmp.persona = GhostName.Blinky;
                        blinkyTmp.setLevel(this);
                        tmp.name = "Blinky";
                        //m_ghostList.Add(tmp);
                        m_ghostList.Add(blinkyTmp);
                        m_blinky = tmp.transform;
                        break;
                    case 'P':
                        tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity, m_ghostContainer);
                        GhostController pinkyTmp = tmp.GetComponent<GhostController>();
                        pinkyTmp.persona = GhostName.Pinky;
                        pinkyTmp.setLevel(this);
                        tmp.name = "Pinky";
                        //m_ghostList.Add(tmp);
                        m_ghostList.Add(pinkyTmp);
                        m_pinky = tmp.transform;
                        break;
                    case 'I':
                        tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity, m_ghostContainer);
                        GhostController inkyTmp = tmp.GetComponent<GhostController>();
                        inkyTmp.persona = GhostName.Inky;
                        inkyTmp.setLevel(this);
                        tmp.name = "Inky";
                        //m_ghostList.Add(tmp);
                        m_ghostList.Add(inkyTmp);
                        m_inky = tmp.transform;
                        break;
                    case 'C':
                        tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity, m_ghostContainer);
                        GhostController clydeTmp = tmp.GetComponent<GhostController>();
                        clydeTmp.persona = GhostName.Clyde;
                        clydeTmp.setLevel(this);
                        tmp.name = "Clyde";
                        //m_ghostList.Add(tmp);
                        m_ghostList.Add(clydeTmp);
                        m_clyde = tmp.transform;
                        break;
                    // Player 
                    case 's':
                        tmp = Instantiate(player, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
                        tmp.name = "player";
                        m_pacMan = tmp.transform;
                        gm.Init();
                        foreach (GhostController enemy in m_ghostList)
                        {
                            enemy.setPacMan(tmp.GetComponent<PlayerController>());
                            enemy.Init(enemy.transform.position);
                            enemy.gameObject.GetComponent<Pathfinding>().setTarget(m_pacMan.transform);
                            enemy.gameObject.GetComponent<Pathfinding>().setLevel(this);
                            gm.AddGhost(enemy);
                        }
                        break;
                }

                bool walkable = !(Physics.CheckSphere(new Vector3(x, 0, z), .4f, unwalkableMask));
                m_grid[z, x] = new Node(walkable, new Vector3(x, 0, z), z, x);
            }
        }
        m_levelBuilt = true;
    }

    // -- pathfinding ------------------------------------
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int z = Mathf.RoundToInt(worldPosition.z);

        return m_grid[z, x];
    }


    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int z = 1; z >= -1; z--)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && z == 0)
                {
                    continue; // TODO: cleanup
                }

                int checkX = node.m_gridX + x;
                int checkZ = node.m_gridZ + z;

                if (checkX >= 0 && checkX < m_levelWidth && checkZ >= 0 && checkZ < m_levelDepth)
                {
                    neighbours.Add(m_grid[checkZ, checkX]);
                }
            }
        }
        return neighbours;
    }

    public Transform getPacMan() { return m_pacMan.transform; }
    public Node[,] getGrid() { return m_grid; }

    // debugging in scene view 
    void OnDrawGizmos()
    {

        if (m_grid != null && showPathfindingGrid)
        {

            Node blinkyNode = NodeFromWorldPoint(m_blinky.position);
            Node pinkyNode = NodeFromWorldPoint(m_pinky.position);
            Node inkyNode = NodeFromWorldPoint(m_inky.position);
            Node clydeNode = NodeFromWorldPoint(m_clyde.position);

            foreach (Node n in m_grid)
            {
                Gizmos.color = (n.m_walkable) ? Color.white : Color.red;

                if (blinkyNode == n) { continue; }
                if (pinkyNode == n) { continue; }
                if (inkyNode == n) { continue; }
                if (clydeNode == n) { continue; }

                if (wireframe)
                {
                    Gizmos.DrawWireCube(n.m_worldPos, Vector3.one * (m_nodeDiameter - .1f));
                }
                else
                {
                    Gizmos.DrawCube(n.m_worldPos, Vector3.one * (m_nodeDiameter - .1f));
                }
            }
        }
    }
}
