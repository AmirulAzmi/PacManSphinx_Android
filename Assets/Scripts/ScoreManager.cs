using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    private int m_score = 0;
    private int m_life = 0;
    private int m_totalscore;
    private int m_hiscore;
    public Text scoreText;
    public Text score;
    public Text lifeText;
    public Text life;
    public Text total;

    void Start()
    {
        m_score = PlayerPrefs.GetInt("score");
        m_hiscore = PlayerPrefs.GetInt("hiscore");
        m_life = PlayerPrefs.GetInt("life");
        m_totalscore = m_score + m_life * 50;
        scoreText.text = ": " + m_score.ToString();
        score.text = m_score.ToString();

        lifeText.text = ": " + m_life.ToString() + " x 50";
        life.text = (m_life * 50).ToString();

        total.text = "TOTAL :  " + m_totalscore.ToString();
        if (m_totalscore > m_hiscore)
        {
            m_hiscore = m_totalscore;
            PlayerPrefs.SetInt("hiscore", m_hiscore);
        }
        PlayerPrefs.DeleteKey("score");
        PlayerPrefs.DeleteKey("life");
        PlayerPrefs.DeleteKey("level");
        PlayerPrefs.DeleteKey("input");
    }
}
