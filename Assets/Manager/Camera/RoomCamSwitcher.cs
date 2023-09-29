using Cinemachine;
using UnityEngine;

public class RoomCamSwitcher : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.parent != null && collision.transform.parent.name == "FullPlayer")
        {
            FindObjectOfType<CameraShake>().gameObject.GetComponent<CinemachineConfiner>().m_BoundingShape2D = transform.GetComponent<PolygonCollider2D>();
        }
    }
}
