using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<EnemySpawnerManager> spawners;

    public void RespawnAllEnemies()
    {
        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.RespawnEnemy();
            }
        }
    }
}
