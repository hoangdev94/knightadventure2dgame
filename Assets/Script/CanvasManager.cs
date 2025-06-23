using UnityEngine;

public class CanvasManager : MonoBehaviour
{

 

    public void PlayAgain()
    {
        GameManager.Instance.RestartGame();
    }
    public void ExitToMenu()
    {
        GameManager.Instance.GoToMenu();
    }
    public void PauseGame()
    {
        GameManager.Instance.PauseGame();
    }
    public void ContinueGame()
    {
        GameManager.Instance.ContinueGame();
    }
}
