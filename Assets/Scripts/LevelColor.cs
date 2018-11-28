using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelColor : MonoBehaviour {
    private Renderer m_renderer;
    //private Color red, green, blue, yellow, purple;
    
    private Color blue = new Color32(3, 110, 196, 214);
    private Color green = new Color32(53, 141, 53, 214);
    private Color red = new Color32(215, 35, 35, 214);
    private Color yellow = new Color32(225, 225, 45, 214);
    private Color purple = new Color32(135, 60, 135, 214);
    
    public GameManager gm;

	// Use this for initialization
    void Start()
    {
        gm = GameObject.Find("Managers").GetComponent<GameManager>();

        m_renderer = GetComponent<Renderer>();
        switch (gm.m_level % 5)
        {
            case 1:
                m_renderer.material.color = blue;
                break;
            case 2:
                m_renderer.material.color = green;
                break;
            case 3:
                m_renderer.material.color = yellow;
                break;
            case 4:
                m_renderer.material.color = red;
                break;
            case 0:
                m_renderer.material.color = purple;
                break;
        }
        //m_renderer.material.color = 
	}
	
	// Update is called once per frame
	void Update () {
	}
}
