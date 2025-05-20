using UnityEngine;

public class BackGroundController : MonoBehaviour
{
   

    public Transform cam;             
  

    private void LateUpdate()
    {
        transform.position = new Vector3(cam.position.x, cam.position.y, 0);
    }
}
