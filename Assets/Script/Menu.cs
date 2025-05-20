using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject OptionMenu;
    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject Stage;

    private void OnEnable()
    {
        // Đặt lại trạng thái menu mỗi lần quay lại
        OptionMenu?.SetActive(false);
        StartMenu?.SetActive(false);

        // Gắn lại sự kiện cho nút mỗi khi quay lại menu
        Button[] levelButtons = Stage.GetComponentsInChildren<Button>();
        foreach (Button btn in levelButtons)
        {
            btn.onClick.RemoveAllListeners(); // Xóa listener cũ nếu có
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
            string level = btnText.text;
            btn.onClick.AddListener(() => OnLevelClick(level));
        }
    }

    void OnLevelClick(string level)
    {
        SceneManager.LoadScene("Level " + level);
    }

    public void PlayGame()
    {
        StartMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Setting()
    {
        OptionMenu.SetActive(true);
    }
}
