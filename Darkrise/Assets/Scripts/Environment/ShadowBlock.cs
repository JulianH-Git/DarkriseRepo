using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowBlock : MonoBehaviour
{
    SpriteRenderer sr;
    BoxCollider2D bx;
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
  
                if (PlayerController.Instance.Interact())
                {
                    PlayerController.Instance.pState.shadowWalking = true;
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
        Gizmos.DrawRay(transform.position, Vector2.down * verticalConnectionLength);
        Gizmos.DrawRay(transform.position, Vector2.up * verticalConnectionLength);
        Gizmos.DrawRay(transform.position, Vector2.left * horizontalConnectionLength);
        Gizmos.DrawRay(transform.position, Vector2.right * horizontalConnectionLength);
    }

    private void CheckForShadowBlocks() // this could probably be made more efficient ngl
    {
        List<RaycastHit2D> results1 = new List<RaycastHit2D>();
        List<RaycastHit2D> results2 = new List<RaycastHit2D>();
        List<RaycastHit2D> results3 = new List<RaycastHit2D>();
        List<RaycastHit2D> results4 = new List<RaycastHit2D>();

        int downHit = Physics2D.Raycast(transform.position, Vector2.down, ctf, results1, verticalConnectionLength);
        int upHit = Physics2D.Raycast(transform.position, Vector2.up, ctf, results2, verticalConnectionLength);
        int leftHit = Physics2D.Raycast(transform.position, Vector2.left, ctf, results3, horizontalConnectionLength);
        int rightHit = Physics2D.Raycast(transform.position, Vector2.right, ctf, results4, horizontalConnectionLength);

        if (downHit >= 2)
        {
            connected[0] = true;
        }
        if (upHit >= 2)
        {
            connected[1] = true;
        }
        if (leftHit >= 2)
        {
            connected[2] = true;
        }
        if (rightHit >= 2)
        {
            connected[3] = true;
        }
    }
}
