using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Normal, Excited, Pause, Over }
public class GameManager : MonoBehaviour
{
    private AudioSource[] aud;

    public GameState m_gamestate;
    //private List<GameObject> m_ghostList;
    private List<GhostController> m_ghostList;
    private PlayerController m_pacman;
    private HUD hud;

    private bool isWin = false;
    public int m_level = 1;
    public int m_life = 3;
    public int m_score = 0;
    public int m_hiscore;
    public bool m_isStarted = false;
    public float m_counterDuration = 10f;
    public bool useSpeech = true;

    void Start()
    {
        aud = GetComponents<AudioSource>();
        hud = GetComponent<HUD>();
        m_pacman = GameObject.Find("player").GetComponent<PlayerController>();

        m_gamestate = GameState.Start;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        m_hiscore = PlayerPrefs.GetInt("hiscore", m_hiscore);
        m_score = PlayerPrefs.GetInt("score", m_score);
        m_life = PlayerPrefs.GetInt("life", m_life);
        
        m_level = PlayerPrefs.GetInt("level");
        if (m_level == 0) { m_level = 1; }

        if (PlayerPrefs.GetInt("input") == 1) { useSpeech = false; }
        else { useSpeech = true; }

        StartCoroutine(StartGame());
    }

    void Update()
    {

        //if game is normal and excited music still playing
        if (m_gamestate == GameState.Normal && aud[2].isPlaying)
        {
            aud[2].Stop();
            aud[1].Play();
        }
        if (m_life == 0)
        {
            isWin = false;
            GameOver();
        }
        if (GameObject.FindGameObjectsWithTag("_dots").Length <= 0 && GameObject.FindGameObjectsWithTag("_energizers").Length <= 0)
        {
            m_isStarted = false;
            isWin = true;
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && m_isStarted)
            hud.TogglePauseGame();
    }
    //-------------------------------------------------------------

    public void addScore(int delta)
    {
        m_score += delta;
    }
    public void minusLife()
    {
        if (m_life > 0)
        {
            m_life--;
            aud[3].Play(); //play death sfx
            StartCoroutine(RestartGame());
        }
    }

    public void CounterAttack()
    { //called from pickup
        //set gamestate to excited
        m_gamestate = GameState.Excited;
        m_pacman.m_state = PlayerState.Super;
        foreach (GhostController g in m_ghostList)
        {
            g.setState(GhostState.Scared);   //GetComponent<GhostController>().setState(GhostState.Scared);
        }
        StartCoroutine(EnergizePacman());
        StopMusic();
        aud[2].Play();
    }

    private IEnumerator EnergizePacman()
    {
        yield return new WaitForSeconds(m_counterDuration);
        m_pacman.m_state = PlayerState.Normal;
        m_gamestate = GameState.Normal;
    }
    public void Init()
    {
        m_ghostList = new List<GhostController>();
    }

    public void AddGhost(GhostController ghost)
    {
        m_ghostList.Add(ghost);
    }

    private IEnumerator StartGame()
    {
        StopMusic();
        aud[0].Play();

        hud.pauseBtn.SetActive(false);
        hud.logtext.transform.gameObject.SetActive(true);
        hud.logtext.text = "ready..";
        yield return new WaitForSeconds(aud[0].clip.length * 4 / 5);
        hud.logtext.text = "start!";
        yield return new WaitForSeconds(aud[0].clip.length * 1 / 5);
        hud.logtext.transform.gameObject.SetActive(false);
        hud.pauseBtn.SetActive(true);
        hud.logtext.text = "";

        m_pacman.movement = "none";
        m_pacman.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0)); //debugging
        m_gamestate = GameState.Normal;
        m_isStarted = true;

        StopMusic();
        aud[1].Play();
    }

    public void PauseGame()
    {
        m_isStarted = false;
        //Time.timeScale = 0;
        //m_pacman.ToggleMoving();

    }
    public void ResumeGame()
    {
        m_isStarted = true;
        //Time.timeScale = 1;
        //m_pacman.ToggleMoving();
    }
    IEnumerator RestartGame()
    {
        m_isStarted = false;
        yield return new WaitForSeconds(3.0f);
        foreach (GhostController g in m_ghostList)
        {
            g.reset();
        }
        m_pacman.reset();
        StartCoroutine(StartGame());
    }
    public void GameOver()
    {
        if (!isWin)
        {
            hud.logtext.text = "gameover!";
            hud.logtext.transform.gameObject.SetActive(true);
            StartCoroutine(ToScoreView());
        }
        else
        {
            hud.logtext.text = "level complete!";
            hud.logtext.transform.gameObject.SetActive(true);
            StartCoroutine("LevelComplete");
            PlayerPrefs.SetInt("level", m_level + 1);
            PlayerPrefs.SetInt("score", m_score);
            PlayerPrefs.SetInt("life", m_life);
            if (useSpeech) { PlayerPrefs.SetInt("input", 0); }
            else PlayerPrefs.SetInt("input", 1);

        }
    }

    public IEnumerator ToScoreView()
    {
        //Time.timeScale = 1;
        PlayerPrefs.SetInt("score", m_score);
        PlayerPrefs.SetInt("life", m_life);
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("gameover", LoadSceneMode.Single);
    }

    private IEnumerator LevelComplete(){
        if(!aud[4].isPlaying)
            aud[4].Play();
        
        yield return new WaitForSeconds(aud[4].clip.length + 0.5f);
        SceneManager.LoadScene("level", LoadSceneMode.Single);
    }

    private void StopMusic()
    {
        foreach (AudioSource a in aud)
        {
            a.Stop();
        }
    }
}
