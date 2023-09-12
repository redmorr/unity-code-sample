using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance;
    
    public static SoundManager Sounds { get; private set; }
    public static LevelManager Level { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public static Collider PlayerCollider { get; private set; }
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Persistence.Initialize();
            Sounds = GetComponentInChildren<SoundManager>();
            Level = GetComponentInChildren<LevelManager>();
            PlayerTransform = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
            PlayerCollider = PlayerTransform.GetComponent<Collider>();
        }
        else
        {
            Debug.LogError("Game MonoBehaviour duplicate!");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    // Careful! It also gets called when a duplicate is destroyed during its instantiation. 
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Constants.Level2)
        {
            PlayerTransform = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
            PlayerCollider = PlayerTransform.GetComponent<Collider>();
        }
    }
}