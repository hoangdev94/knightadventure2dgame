using Cainos.PixelArtPlatformer_Dungeon;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string transiotionTo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Door.Instance.IsOpened)
        {
            StartCoroutine(LoadLevel(5f));
        }
    }

    IEnumerator LoadLevel( float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(transiotionTo);
    }
}
