using UnityEngine;

public class MirrorPlate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Sprite mirror;
    [SerializeField] Sprite bounce;
    [SerializeField] float bounciness;
    [SerializeField] float horizontalBounciness;
    PlayerController player;
    SpriteRenderer sr;
    float rotation = 0f;

    PlateState currentState;
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
        if (collision.collider.CompareTag("Laser") && currentState == PlateState.Reflect) // mirror needs to reflect lasers that touch it
        {

        }
        else if (collision.collider.CompareTag("Player") && currentState == PlateState.Bounce) // mirror needs to become bouncy on contact
        {
            switch(rotation)
            {
                case 0:
                    player.AddForce(bounciness);
                    break;
                case 45:
                    player.AddForce(new Vector2(-horizontalBounciness, bounciness));
                    break;
                case 90:
                    player.AddXForce(-horizontalBounciness);
                    break;
                case 135:
                    player.AddForce(new Vector2(-horizontalBounciness, -bounciness));
                    break;
                case 180:
                    player.AddForce(-bounciness);
                    break;
                case 225:
                    player.AddForce(new Vector2(horizontalBounciness, -bounciness));
                    break;
                case 270:
                    player.AddXForce(horizontalBounciness);
                    break;
                case 315:
                    player.AddForce(new Vector2(horizontalBounciness, bounciness));
                    break;
                case 360:
                    player.AddForce(bounciness);
                    break;
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
        }
        else
        {
            sr.sprite = bounce;
        }
    }

    public void Rotate()
    {

        rotation += 45f;
        if (rotation >= 360f)
        {
            rotation = 0f;
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotation);
    }
}
