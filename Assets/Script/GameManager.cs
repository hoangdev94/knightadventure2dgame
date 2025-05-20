using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score;
    public int keychest = 0;
    public int keydoor = 0;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject hpbarBoss;
    [SerializeField] private GameObject Boss;
    [SerializeField] private string Level;
    private bool isGameOver = false;
    private bool isGameplayScene;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        Time.timeScale = 1;
        Level = SceneManager.GetActiveScene().name;
        FindUIReferences();
        UpdateScore();
        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            isGameplayScene = true;
        }

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Level = scene.name;
        Time.timeScale = 1;
        FindUIReferences();
        UpdateScore();
        UpdateTrigger();
        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (isGameplayScene && Boss != null && hpbarBoss != null)
        {
            hpbarBoss.SetActive(true);
        }
    }

    private void FindUIReferences()
    {
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.Find("ScoreText");
            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
            }
        }

        if (gameOverUi == null)
        {
            gameOverUi = GameObject.Find("GameOverUI");
        }
        if(hpbarBoss == null)
        {
            hpbarBoss = GameObject.Find("HpBarBoss");
        }
        if(Boss == null)
        {
                Boss = GameObject.FindWithTag("Boss");
        }
    }

    public void AddScore(int point)
    {
        score += point;
        AudioManager.Instance?.CoinClip();
        UpdateScore();
    }

    public void AddKeyChest(int key)
    {
        keychest += key;
        Debug.Log("Key chest: " + keychest);
        AudioManager.Instance?.PickKey();
    }

    public void AddKeyDoor(int key)
    {
        keydoor += key;
        AudioManager.Instance?.PickKey();
    }

    private void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        score = 0;
        if (gameOverUi != null) gameOverUi.SetActive(true);
    }

    public void TriggerGameOverAfter(float delay)
    {
        StartCoroutine(GameOverAfterDelay(delay));
    }

    private IEnumerator GameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameOver();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void RestartGame()
    {
        isGameOver = false;
        score = 0;
        UpdateScore();
        Time.timeScale = 1;
        SceneLoader.Instance.LoadScene(Level);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        isGameOver = false;
        SceneLoader.Instance.LoadScene("Menu");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.currentHp = 0;
                player.UpdateHpBar();
            }

            GameOver();
            Time.timeScale = 0;
        }
    }

    void UpdateTrigger()
    {
        if (SceneManager.GetActiveScene().name == "Level 2")
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            if (box != null)
            {
                box.enabled = false;
            }
        }
    }
}