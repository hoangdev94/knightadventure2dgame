using Cainos.PixelArtPlatformer_Dungeon;
using UnityEngine;

public class ChestCollection : Chest
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] GameObject keyDoor;
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.Instance.keychest > 0 )
        {
            base.Open();
            GameManager.Instance.keychest--;
            base.IsOpened = true;
            AudioManager.Instance.OpenChest();
            if (keyDoor != null)
            {

                Vector3 spawnPosition = this.transform.position + new Vector3(0, 1f, 0); // bay lên 1 đơn vị
                Instantiate(keyDoor, spawnPosition, Quaternion.identity);
                Debug.Log("This object's transform: " + transform.name + " at " + transform.position);

            }
            Debug.Log(GameManager.Instance.keychest);
        }
    }
}
