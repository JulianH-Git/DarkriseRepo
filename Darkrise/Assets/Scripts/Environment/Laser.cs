using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    public LayerMask layersToHit;
    [SerializeField] float laserLength = 10f;
    [SerializeField] int maxReflections = 5;
    private LineRenderer lineRenderer;
    private Vector2 dir;
    private PlayerController controller;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        controller = PlayerController.Instance;
    }

    void Update()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        dir = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

        DrawLaser(transform.position, dir);
    }

    void DrawLaser(Vector2 startPosition, Vector2 direction)
    {
        Vector2 currentPos = startPosition;
        Vector2 currentDir = direction;
        int reflectionsRemaining = maxReflections;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPos);

        int segmentIndex = 1;

        while (reflectionsRemaining > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, laserLength, layersToHit);

            if (hit.collider != null)
            {
                Vector2 hitPoint = hit.point;
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(segmentIndex, hitPoint);
                segmentIndex++;

                Debug.DrawLine(currentPos, hitPoint, Color.red);

                if (hit.collider.CompareTag("Mirror"))
                {
                    currentDir = Vector2.Reflect(currentDir, hit.normal);
                    currentPos = hitPoint + currentDir * 0.01f;
                    Debug.DrawRay(hit.point, hit.normal, Color.green);
                    reflectionsRemaining--;
                }
                /*else if(hit.collider.CompareTag("Player"))
                {
                    if(!controller.pState.invincible && !controller.pState.dashing && controller.Health >= 1)
                    {
                        controller.TakeDamage(1);
                        reflectionsRemaining--;
                    }
                    else
                    {
                        break;
                    }
                }*/
                else
                {
                    if (hit.collider.CompareTag("Receiver"))
                    {
                        LaserReciever receiver = hit.collider.GetComponent<LaserReciever>();
                        if (receiver != null)
                        {
                            receiver.ReceiveLaser();
                        }
                    }
                    break;
                }
            }
            else
            {
                Vector2 endPoint = currentPos + currentDir * laserLength;
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(segmentIndex, endPoint);
                Debug.DrawLine(currentPos, endPoint, Color.red);

                break;
            }
        }
    }
}
