using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
  

    [Header("UI References")]
    public GameObject loadingCanvas;
    public Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingCanvas.SetActive(true);
        loadingBar.value = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float targetProgress = 0;
        float displayProgress = 0;
        float minLoadTime = 2f;
        float fakeTime = 0f;

        while (!operation.isDone)
        {
            // Tính target progress từ operation
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Fake loading time
            fakeTime += Time.deltaTime;

            // Tăng displayProgress chậm dần về targetProgress
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime);
            loadingBar.value = displayProgress;

            // Chờ cho đến khi load xong và thời gian tối thiểu đã đủ
            if (operation.progress >= 0.9f && displayProgress >= 0.99f && fakeTime >= minLoadTime)
            {
                yield return new WaitForSeconds(0.3f);
                operation.allowSceneActivation = true;
            }
            loadingText.text = "Loading... " + Mathf.RoundToInt(displayProgress * 100f) + "%";
            yield return null;
        }

        loadingCanvas.SetActive(false);
    }



}
