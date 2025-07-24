using Cinemachine.Utility;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    public LayerMask layersToHit;
    [SerializeField] float laserLength = 10f;
    [SerializeField] int maxReflections = 5;
    [SerializeField] GameObject reflectionParticles;
    [SerializeField] GameObject reflectorsParticlesParent;
    private LineRenderer lineRenderer;
    private Vector2 dir;
    private PlayerController controller;
    private List<GameObject> _reflectionParticles = new List<GameObject>();
    private List<Vector2> hits = new List<Vector2>();
    private List<Vector2> lastHits = new List<Vector2>();

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
        AddParticles(hits);
        hits.Clear();
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
                    hits.Add(currentPos);
                    Debug.DrawRay(hit.point, hit.normal, Color.green);
                    reflectionsRemaining--;
                }
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

    void AddParticles(List<Vector2> _hits)
    {
        if(lastHits.SequenceEqual(_hits)) return;
        else
        {
            for (int i = 0; i < _hits.Count; i++)
            {
                if (i >= _reflectionParticles.Count)
                {
                    _reflectionParticles.Add(Instantiate(reflectionParticles, _hits[i], Quaternion.identity, reflectorsParticlesParent.transform));
                }

                _reflectionParticles[i].transform.position = _hits[i];
                _reflectionParticles[i].SetActive(true);
            }

            for (int i = _hits.Count; i < _reflectionParticles.Count; i++)
            {
                _reflectionParticles[i].SetActive(false);
            }
        }
    }
}
