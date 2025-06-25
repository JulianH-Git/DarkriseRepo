using UnityEngine;

public class MirrorPlate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Sprite mirror;
    [SerializeField] Sprite bounce;
    [SerializeField] PlateState currentState;
    [SerializeField] float bounciness;
    [SerializeField] GameObject reflectionSurface;
    PlayerController player;
    SpriteRenderer sr;
    float rotation = 0f;

    enum PlateState
    {
        Reflect,
        Bounce
    }


    void Start()
    {
        player = PlayerController.Instance;
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && currentState == PlateState.Bounce) // mirror needs to become bouncy on contact
        {
            if (!player.Grounded())
            {
                Vector2 bounceDirection = transform.up;
                player.AddForce(bounceDirection * bounciness);

            }
        }
    }
    public void ChangeState()
    {
        currentState++;
        if ((int)currentState >= 2) { currentState = 0; }

        if (currentState == PlateState.Reflect)
        {
            sr.sprite = mirror;
            reflectionSurface.SetActive(true);
        }
        else
        {
            sr.sprite = bounce;
            reflectionSurface.SetActive(false);
        }
    }

    public void Rotate()
    {

        rotation -= 45f;
        if (rotation <= -360f)
        {
            rotation = 0f;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotation);
    }
}
