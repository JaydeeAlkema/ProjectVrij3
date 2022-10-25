using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WwiseGameState { MainMenu, Gameplay, GameOver, Paused, None};
// these reference the "Game State variables" and "Music State variables"
public enum WwiseMusicState { MainMenu, CombatLean, CombatValaidria, None};

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private bool bIsInitialized = false;

    [Header("Startup Soundbanks")]
    [SerializeField] private List<AK.Wwise.Bank> Soundbanks;

    [Header("Game State variables")]
    // Game states (scene 1, 2, 3 etc.)
    // Should probably make this a prefab
    [SerializeField] private AK.Wwise.State Game_MainMenu;
    [SerializeField] private AK.Wwise.State Game_Gameplay;
    [SerializeField] private AK.Wwise.State Game_GameOver;
    [SerializeField] private AK.Wwise.State Game_Paused;
    [SerializeField] private AK.Wwise.State Game_None;

    private WwiseGameState currentGameState;

    [Header("Music State variables")]
    // States that influence the music
    [SerializeField] private AK.Wwise.State Music_MainMenu;
    [SerializeField] private AK.Wwise.State Music_CombatLean;
    [SerializeField] private AK.Wwise.State Music_CombatValaidria;
    [SerializeField] private AK.Wwise.State Music_None;

    private WwiseMusicState currentMusicState;

    [Header("Wwise Voice Events")]
    [SerializeField] public AK.Wwise.Event CheckGameState;
    [SerializeField] public AK.Wwise.Event CheckMusicState;

    [Header("Wwise Music Events")]
    [SerializeField] public AK.Wwise.Event MainMusic_Play;
    [SerializeField] public AK.Wwise.Event MainMusic_Stop;

    // Awake is called on awake
    private void Awake()
    {
        Initialize();
    }

        // Start is called before the first frame update
        void Start()
        {
        SetWwiseGameState(WwiseGameState.MainMenu);
        SetWwiseMusicState(WwiseMusicState.MainMenu);

        MainMusic_Play.Post(gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            //Only necessary for global RTPC's like a day night cycle
            //or sliders for volume of different busses you've set up
        }

    void Initialize()
    {
        // Singleton logic
        // AudioManger instance manager (no more than 1)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogWarning("AudioManager already exists! Destroying new instance.");
            Destroy(this);
        }

        if (!bIsInitialized)
        {
            LoadSoundbanks();
        }

        SetWwiseGameState(WwiseGameState.None)
        SetWwiseMusicState(WwiseMusicState.None)

        bIsInitialized = true;
    }

    void LoadSoundbanks()
    {
        if (Soundbanks.Count > 0)
        {
            foreach (AK.Wwise.Bank bank in Soundbanks)
                bank.Load();

            Debug.Log("Startup Soundbanks have been loaded");
        }
        else
            Debug.LogError("Soundbanks list is empty! Are the banks assigned to the AudioManager?");
    }


    public void SetWwiseGameState(WwiseGameState GameState)
    {
        if (GameState == currentGameState)
        {
            Debug.Log("GameState is already " + GameState + ".");
            return;
        }

        switch(GameState)
        {
            default://default state for when all goes wrong
            case (WwiseGameState.MainMenu):
                Game_MainMenu.SetValue();
                break;
            case (WwiseGameState.Gameplay):
                Game_Gameplay.SetValue();
                break;
            case (WwiseGameState.GameOver):
                Game_GameOver.SetValue();
                break;
            case (WwiseGameState.Paused):
                Game_Paused.SetValue();
                break;
            case (WwiseGameState.None):
                Game_None.SetValue();
                break;
        }

        Debug.Log("New Wwise GameState: " + GameState + ".")

        currentGameState = GameState
    }

    public void SetWwiseMusicState(WwiseMusicState MusicState)
    {
        if (MusicState == currentMusicState)
        {
            Debug.Log("MusicState is already " + MusicState + ".");
            return;
        }

        switch (MusicState)
        {
            default: //default state for when all goes wrong
            case (WwiseMusicState.MainMenu):
                Music_MainMenu.SetValue();
                break;
            case (WwiseMusicState.CombatLean):
                Music_CombatLean.SetValue();
                break;
            case (WwiseMusicState.CombatValaidria):
                Music_CombatValaidria.SetValue();
                break;
            case (WwiseMusicState.None):
                Music_None.SetValue();
                break;
        }

        Debug.Log("New Wwise MusicState: " + MusicState + ".")

        currentMusicState = MusicState;
    }

    public void PostEvent(AK.Wwise.Event wwiseEvent)
    {
        if (wwiseEvent == null)
        {
            return;
        }
        wwiseEvent.Post(gameObject);
        var wwiseGameObject = GetComponent<AkGameObj>();
        wwiseGameObject.enabled = true;
    }