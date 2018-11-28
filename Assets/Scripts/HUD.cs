using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
    private GameManager gm; 
    private PocketSphinxMobile ps;
    
    public Image inputButton;
    public Sprite inputMicImage;
    public Sprite inputSwipeImage;
    public Text score;
    public Text life;
    public Text info;
    public Text hiscore;
    public Text command;
    public Text logtext;
    public Text usespeech;
    public Text currentLevel;
    private bool isPaused = false;
    public GameObject pauseHud;
    public GameObject pauseBtn;

    void Awake()
    {
        info.text = "Info: Loading speech recognition engine";
    }
    void Start()
    {
        gm = GameObject.Find("Managers").GetComponent<GameManager>();
        ps = GameObject.Find("MovementInput").GetComponent<PocketSphinxMobile>();
        int lvl = PlayerPrefs.GetInt("level");
        if (lvl == 0) { lvl = 1; }
        currentLevel.text = "Level: " + lvl.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        score.text = "Score: " + gm.m_score.ToString();
        life.text = "Life: " + gm.m_life.ToString();

        hiscore.text = "Highscore: " + gm.m_hiscore.ToString();
        if (gm.useSpeech)
        {
            command.text = "Command: " + ps.detected.ToUpper();
            usespeech.text = "voice command";
            inputButton.sprite = inputMicImage;

            if (ps.detected != null || ps.detected != "")
            {
                info.text = "Info: Speak the direction";
            }
        }
        else
        {
            command.text = "Command: - ";
            usespeech.text = "finger swipe";
            inputButton.sprite = inputSwipeImage;
            info.text = "Info: Swipe up, down, left or right";
        }
	}

    public void TogglePauseGame() {
        if (!isPaused)
        {
            isPaused = true;
            gm.PauseGame();
            pauseHud.SetActive(true);
            pauseBtn.SetActive(false);
        }
        else {
            isPaused = false;
            gm.ResumeGame();
            pauseHud.SetActive(false);
            pauseBtn.SetActive(true);
        }
    }
    public void ToggleSpeech()
    {
        if (!gm.useSpeech) { gm.useSpeech = true; }
        else { gm.useSpeech = false; }
    }

    public void ExitGame() {
        StartCoroutine(gm.ToScoreView());
    }

    public void test() {
        gm.m_isStarted = false;
    }
}
