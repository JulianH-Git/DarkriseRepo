using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool jumping = false;
    public bool dashing = false;
    public bool recoilingX = false;
    public bool recoilingY = false;
    public bool lookingRight;
    public bool invincible;
    public bool casting;
    public bool hiding = false;
    public bool recovering = false;
    public bool shadowWalking = false;

    public void PrepForDeath()
    {
        jumping = false;
        dashing = false;
        recoilingX = false;
        recoilingY = false;
        invincible = true;
        casting = false;
        hiding = false;
        recovering = false;
        shadowWalking = false;
    }
}
