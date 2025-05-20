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

}
