using Unity.VisualScripting;
using UnityEngine;

public class PlayerCollection : MonoBehaviour
{

    private void Awake()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.AddScore(1);
            
        }
        else if (collision.CompareTag("KeyChest")){
            Destroy(collision.gameObject);
           GameManager.Instance.AddKeyChest(1);
        }
        else if (collision.CompareTag("KeyDoor"))
        {
            GameManager.Instance.AddKeyDoor(1);
            Destroy(collision.gameObject);
        }
    }

     
}
