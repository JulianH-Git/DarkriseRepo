using UnityEngine;

public class Laser : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask layersToHit;
    [SerializeField] float laserLength;
    PlayerController controller;
    void Start()
    {
        controller = PlayerController.Instance;
    }

    void Update()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 dir = -new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)); 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, laserLength, layersToHit);
        Debug.DrawRay(transform.position, dir);
        if (hit.collider == null)
        {
            transform.localScale = new Vector3(transform.localScale.x, laserLength, 1);
            return;
        }
        transform.localScale = new Vector3(transform.localScale.x, hit.distance, 1);
        Debug.Log(hit.collider.gameObject.name);
/*        if (hit.collider.CompareTag("Player"))
        {
            if(!controller.pState.invincible)
            {
                controller.TakeDamage(1);
            }
        }*/
    }
}

    
