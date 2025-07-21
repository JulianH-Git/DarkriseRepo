using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowBlock : MonoBehaviour
{
    SpriteRenderer sr;
    BoxCollider2D bx;
    [SerializeField] Transform center;
    bool[] connected = { false, false, false, false }; // 0 = down, 1 = up, 2 = left, 3 = right
    [SerializeField] LayerMask ShadowBlockLayer;
    [SerializeField] float horizontalConnectionLength;
    [SerializeField] float verticalConnectionLength;

    ContactFilter2D ctf;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bx = GetComponent<BoxCollider2D>();
        ctf.useTriggers = true;
        ctf.SetLayerMask(ShadowBlockLayer);
        Physics2D.IgnoreLayerCollision(2, 7, true);
        CheckForShadowBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Instance.playerArrowIndicator.SetActive(false);

            Vector2 direction = (collision.transform.position - transform.position).normalized;

            int exitDirection;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                exitDirection = direction.x > 0 ? 3 : 2;
            }
            else
            {
                exitDirection = direction.y > 0 ? 1 : 0;
            }

            if (connected[exitDirection] == false)
            {
                sr.sortingOrder = 0;
                PlayerController.Instance.pState.shadowWalking = false;
                PlayerController.Instance.SR.sortingOrder = 1;
                PlayerController.Instance.ExitShadowWalk();
            }
        }
    }
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if(PlayerController.Instance.currentAttackType == PlayerController.AttackType.Dark)
            {
                if (!PlayerController.Instance.pState.shadowWalking) { PlayerController.Instance.playerArrowIndicator.SetActive(true); }
  
                if (PlayerController.Instance.Interact() || PlayerController.Instance.dashedIntoShadowBlock)
                {    
                    PlayerController.Instance.pState.shadowWalking = true;
                    PlayerController.Instance.dashedIntoShadowBlock = false;
                    PlayerController.Instance.playerArrowIndicator.SetActive(false);
                }
                
            }
            else
            {
                PlayerController.Instance.pState.shadowWalking = false;
                PlayerController.Instance.playerArrowIndicator.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255f, 255f, 255f, 0.5f);
        Gizmos.DrawRay(center.transform.position, Vector2.down * verticalConnectionLength);
        Gizmos.DrawRay(center.transform.position, Vector2.up * verticalConnectionLength);
        Gizmos.DrawRay(center.transform.position, Vector2.left * horizontalConnectionLength);
        Gizmos.DrawRay(center.transform.position, Vector2.right * horizontalConnectionLength);
    }

    private void CheckForShadowBlocks()
    {
        int[] hits = new int[4]; // 0 = up, 1 = down, 2 = left, 3 = right

        List<RaycastHit2D> results1 = new List<RaycastHit2D>();
        List<RaycastHit2D> results2 = new List<RaycastHit2D>();
        List<RaycastHit2D> results3 = new List<RaycastHit2D>();
        List<RaycastHit2D> results4 = new List<RaycastHit2D>();

        if(horizontalConnectionLength > 0)
        {
            hits[2] = Physics2D.Raycast(center.transform.position, Vector2.left, ctf, results3, horizontalConnectionLength);
            hits[3] = Physics2D.Raycast(center.transform.position, Vector2.right, ctf, results4, horizontalConnectionLength);
        }
        else
        {
            hits[2] = 0;
            hits[3] = 0;
        }

        if(verticalConnectionLength > 0)
        {
            hits[0] = Physics2D.Raycast(center.transform.position, Vector2.down, ctf, results1, verticalConnectionLength);
            hits[1] = Physics2D.Raycast(center.transform.position, Vector2.up, ctf, results2, verticalConnectionLength);
        }
        else
        {
            hits[0] = 0;
            hits[1] = 0;
        }

        for(int i = 0; i < hits.Length; i++)
        {
            if (hits[i] >= 2)
            {
                connected[i] = true;
            }
            else
            {
                connected[i] = false;
            }
        }
    }
}
