using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechDemo : MonoBehaviour
{

    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private Text _SpeechResult;
    [SerializeField]
    private Text _IncText;

    private float inc;
    private PocketSphinxMobile ps;

    // Use this for initialization
    void Awake() {
        _infoText.text = "Loading speech recognition engine";
    }

    void Start()
    {
        ps = GameObject.Find("PocketSphinx").GetComponent<PocketSphinxMobile>();
    }

    // Update is called once per frame
    void Update()
    {
        inc += Time.deltaTime;
        _IncText.text = inc.ToString("F4");

        if (ps.detected != null || ps.detected != "") {
            _infoText.text = "Speak the direction";
        }

        if (ps.detected != _SpeechResult.text)
        {
            _SpeechResult.text = ps.detected;
            inc = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}