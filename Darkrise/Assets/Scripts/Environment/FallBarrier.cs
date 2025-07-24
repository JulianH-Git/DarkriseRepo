using UnityEngine;

public class FallBarrier : InteractTrigger
{
    [SerializeField] protected GameObject teleportPoint;
    protected override void TriggerActivated()
    {
        Vector2 point = new Vector2(teleportPoint.transform.position.x, teleportPoint.transform.position.y);
        player.transform.localPosition = point;
        controller.TakeDamage(1);
        controller.HitStopTime(0.1f, 2, 0.5f);
    }
}
