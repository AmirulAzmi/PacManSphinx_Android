using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class commandTest : MonoBehaviour {
    //public SphinxWin sphinx;
    public PocketSphinxMobile ps;
    public List<Animator> anims;
    public Text micText;
    public Text commandText;

	// Use this for initialization

    void Awake() {
        micText.text = "Info: Loading speech recognition engine";
    }
	void Start () {

        ps = GetComponent<PocketSphinxMobile>();
        anims = new List<Animator>();
        anims.Add(GameObject.Find("ArrowUp").GetComponent<Animator>());
        anims.Add(GameObject.Find("ArrowDown").GetComponent<Animator>());
        anims.Add(GameObject.Find("ArrowLeft").GetComponent<Animator>());
        anims.Add(GameObject.Find("ArrowRight").GetComponent<Animator>());
        //sphinx.m_detected = "none";
	}
	
	// Update is called once per frame
    void Update()
    {
        if (ps.detected != null || ps.detected != "")
        {
            micText.text = "Info: Speak the direction";
        }
        if (ps.detected != commandText.text)
        {
            commandText.text = "Command: " + ps.detected;
        }

        if (ps.detected == "up") { Highlight(0); }
        else if (ps.detected == "down") { Highlight(1); }
        else if (ps.detected == "left") { Highlight(2); }
        else if (ps.detected == "right") { Highlight(3); }
        else { StartCoroutine(DimAll()); }
    }

    IEnumerator DimAll() {
        foreach (Animator a in anims) {
            a.SetBool("highlight", false);
        }
        yield return new WaitForSeconds(0);
    }
    void Highlight(int i) {
        StartCoroutine(DimAll());
        anims[i].SetBool("highlight", true);
    }
}
