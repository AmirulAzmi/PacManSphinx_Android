using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PocketSphinxMobile : MonoBehaviour, IPocketSphinxEvents
{
    public string grammar;
    //private const String MENU_SEARCH = "direction";

    [SerializeField]
    private GameObject _pocketSphinxPrefab;

    private UnityPocketSphinx.PocketSphinx _pocketSphinx;

    public string detected;

    // ---- Private function -------------------
    private void SubscribeToPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech += OnBeginningOfSpeech;
        em.OnEndOfSpeech += OnEndOfSpeech;
        em.OnError += OnError;
        em.OnInitializeFailed += OnInitializeFailed;
        em.OnInitializeSuccess += OnInitializeSuccess;
        em.OnPocketSphinxError += OnPocketSphinxError;
        em.OnResult += OnResult;
        em.OnTimeout += OnTimeout;
    }

    private void UnsubscribeFromPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech -= OnBeginningOfSpeech;
        em.OnEndOfSpeech -= OnEndOfSpeech;
        em.OnError -= OnError;
        em.OnInitializeFailed -= OnInitializeFailed;
        em.OnInitializeSuccess -= OnInitializeSuccess;
        em.OnPocketSphinxError -= OnPocketSphinxError;
        em.OnResult -= OnResult;
        em.OnTimeout -= OnTimeout;
    }

    private void Searching(string searchKey)
    {
        _pocketSphinx.StopRecognizer();
        _pocketSphinx.StartListening(searchKey, 10000);
        detected = "...";
    }
    //-------------------------------------------------------------
    // Use this for initialization
    void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_pocketSphinxPrefab, "No PocketSphinxPrefab");
        var obj = Instantiate(_pocketSphinxPrefab, this.transform) as GameObject;
        _pocketSphinx = obj.GetComponent<UnityPocketSphinx.PocketSphinx>();

        if (_pocketSphinx == null)
        {
            Debug.LogError("PocketSphinx not found");
        }

        SubscribeToPocketSphinxEvents();
    }

    void Start()
    {
        _pocketSphinx.SetAcousticModelPath("en-us-ptm");
        //Debug.Log("[SpeechRecognizerDemo] " + Application.streamingAssetsPath + "cmudict-en-us.dict");
        _pocketSphinx.SetDictionaryPath("cmudict-en-us.dict");
        _pocketSphinx.SetKeywordThreshold(1e-45f);
        _pocketSphinx.AddBoolean("-allphone_ci", true);

        // These one are optional
        //_pocketSphinx.AddGrammarSearchPath(MENU_SEARCH, "direction.gram");
        _pocketSphinx.AddGrammarSearchPath(grammar, grammar + ".gram");

        _pocketSphinx.SetupRecognizer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        if (_pocketSphinx != null)
        {
            UnsubscribeFromPocketSphinxEvents();
            _pocketSphinx.DestroyRecognizer();
        }
    }

    // ---- Events functions -------------------

    public void OnResult(string hypothesis)
    {
        detected = hypothesis;
    }

    public void OnBeginningOfSpeech() { }
    public void OnEndOfSpeech()
    {
        //Searching(MENU_SEARCH);
        Searching(grammar);
    }

    public void OnError(string error)
    {
        Debug.LogError("OnError(): " + error);
    }

    public void OnTimeout()
    {
        Debug.LogError("OnTimeout()");
        //Searching(MENU_SEARCH);
        Searching(grammar);
    }
    public void OnInitializeSuccess()
    {
        //_pocketSphinx.AddGrammarSearchPath(MENU_SEARCH, "menu.gram"); //***********
        //Searching(MENU_SEARCH);
        Searching(grammar);
        Debug.Log("Initialize Success");
    }
    public void OnInitializeFailed(string error)
    {
        Debug.LogError("InitializeFailed(): " + error);
    }
    public void OnPocketSphinxError(string error)
    {
        Debug.LogError("OnPocketSphinxError(): " + error);
    }
}
