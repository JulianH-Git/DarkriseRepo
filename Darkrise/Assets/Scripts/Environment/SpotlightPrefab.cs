using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static PlayerController;
//using static UnityEditor.Experimental.GraphView.GraphView;

public enum SpotlightStates 
{
    Off,
    Red,
    Yellow,
    Laser,
    ForcedEncounter
}

public class SpotlightPrefab : MonoBehaviour
{
    public SpotlightStates state;
    [SerializeField] GameObject lamp;
    [SerializeField] List<Sprite> lampSprites;
    [SerializeField] List<Sprite> spotlightSprites;
    [SerializeField] ParticleSystem spotlightParticles;
    ParticleSystem.ColorOverLifetimeModule colorOverLifetime;
    ParticleSystem.EmissionModule emission;
    [SerializeField] List<Gradient> spotlightParticleColors;
    [SerializeField] Light2D lampLight;
    [SerializeField] Light2D spotlightLight;
    [SerializeField] bool isLaser;


    PlayerController controller;

    [Header("Red Spotlight Settings")]
    [SerializeField] List<GameObject> gates = new List<GameObject>();
    private List<Vector2> originalSizes = new List<Vector2>();
    [SerializeField] List<Vector2> spottedSizes = new List<Vector2>();
    [Space(5)]

    [Header("Yellow Spotlight Settings")]
    [SerializeField] Transform enemyAlertTransform;
    [SerializeField] Vector2 enemyAlertRadius;
    [SerializeField] LayerMask enemyLayer;
    [Space(5)]

    [Header("Forced Encounter Spotlight Settings")]
    public bool startEncounter = false;
    [Space(5)]

    [Header("Audio Settings")]
    [SerializeField] private MusicArea area;
    [SerializeField] private bool spotOnce = false;

    // player spotted
    private bool playerSpotted = false;
    public bool PlayerSpotted { get => playerSpotted; }

    // cooldown
    [SerializeField] private float cooldown = 2f; // 2 seconds
    private float cooldownTimer = 0f;
    private float duration = 0.1f;
    private float enemyAlertCooldown = 2f;
    private float enemyAlertCooldownTimer;


    // temporary stuff until laser gets implemented

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        controller = PlayerController.Instance;
        colorOverLifetime = spotlightParticles.colorOverLifetime;
        emission = spotlightParticles.emission;
        sr = GetComponent<SpriteRenderer>();

        if(gates != null && gates.Count > 0)
        {
            foreach (GameObject gate in gates)
            {
                originalSizes.Add(gate.transform.localScale);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            if (!controller.pState.dashing)
            {
                playerSpotted = true;
                cooldownTimer = cooldown;

                switch (state)
                {
                    case SpotlightStates.Red:
                        if (!spotOnce)
                        {
                            AudioManager.instance.PlayOneShot(FMODEvents.instance.redAlarm, this.transform.position);
                            spotOnce = true;
                        }
                        for (int i = 0; i < gates.Count; i++)
                        {
                            StartCoroutine(MoveGates(gates[i], spottedSizes[i]));
                        }
                        break;
                    case SpotlightStates.Yellow:
                        if (!controller.pState.invincible && enemyAlertCooldownTimer >= enemyAlertCooldown)
                        {
                            List<Collider2D> enemiesInRange = CheckForEnemies(enemyAlertTransform, enemyAlertRadius);
                            if (enemiesInRange != null && enemiesInRange.Count > 0)
                            {
                                SignalEnemies(enemiesInRange);
                            }
                            enemyAlertCooldownTimer = 0f;
                        }
                        break;
                    case SpotlightStates.Laser:
                        if (!controller.pState.invincible)
                        {
                            controller.TakeDamage(1);
                        }
                        break;
                    case SpotlightStates.ForcedEncounter:
                        if(!controller.pState.invincible)
                        {
                            if (!spotOnce)
                            {
                                AudioManager.instance.PlayOneShot(FMODEvents.instance.blueAlarm, this.transform.position);
                                
                                spotOnce = true;
                            }
                            
                            AudioManager.instance.SetMusicArea(MusicArea.EncounterArea);

                            startEncounter = true;

                            List<Collider2D> wallsInRange = CheckForWalls(enemyAlertTransform, enemyAlertRadius);
                            if(wallsInRange != null && wallsInRange.Count > 0)
                            {
                                foreach(Collider2D wall in wallsInRange)
                                {
                                    wall.GetComponent<ForcedEncounterRoomLock>().ActivateLock();
                                }
                            }
                            if (enemyAlertCooldownTimer >= enemyAlertCooldown)
                            {
                                List<Collider2D> enemiesinRange = CheckForEnemies(enemyAlertTransform, enemyAlertRadius);
                                if (enemiesinRange != null && enemiesinRange.Count > 0)
                                {
                                    SignalEnemies(enemiesinRange);
                                }
                                enemyAlertCooldownTimer = 0f;
                            }

                        }
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) 
        {
            case SpotlightStates.Off:
                if(isLaser) { lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[3]; }
                else { lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[0]; }
                
                this.GetComponent<BoxCollider2D>().enabled = false;
                sr.enabled = false;
                if(spotlightParticles != null) { emission.enabled = false; }
                if(lampLight != null) { lampLight.enabled = false; }
                if(spotlightLight != null) { spotlightLight.enabled = false; }
                break;

            case SpotlightStates.Red:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[1];
                this.GetComponent<BoxCollider2D>().enabled = true;
                sr.enabled = true;
                sr.sprite = spotlightSprites[0];

                emission.enabled = true;
                lampLight.enabled = true;
                lampLight.color = Color.red;
                spotlightLight.enabled = true;
                spotlightLight.color = Color.red;

                emission.enabled = true;
                colorOverLifetime.color = spotlightParticleColors[0];
                break;

            case SpotlightStates.Yellow:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[2];
                this.GetComponent<BoxCollider2D>().enabled = true;
                sr.enabled = true;
                sr.sprite = spotlightSprites[1];

                lampLight.enabled = true;
                lampLight.color = Color.white;
                spotlightLight.enabled = true;
                spotlightLight.color = Color.white;

                sr.color = Color.white;
                emission.enabled = true;
                colorOverLifetime.color = spotlightParticleColors[1];
                break;
            case SpotlightStates.Laser:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[3];
                this.GetComponent<BoxCollider2D>().enabled = true;
                sr.enabled = true;
                break;
            case SpotlightStates.ForcedEncounter:
                lamp.GetComponent<SpriteRenderer>().sprite = lampSprites[2];
                this.GetComponent<BoxCollider2D>().enabled = true;
                sr.enabled = true;
                sr.sprite = spotlightSprites[1];

                lampLight.enabled = true;
                lampLight.color = Color.blue;
                spotlightLight.enabled = true;
                spotlightLight.color = Color.blue;

                sr.color = Color.blue;
                emission.enabled = true;
                colorOverLifetime.color = spotlightParticleColors[2];
                break;
        }

        enemyAlertCooldownTimer += Time.deltaTime;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else
        {
            playerSpotted = false;
            if(state == SpotlightStates.Red) 
            {
                for (int i = 0; i < gates.Count; i++)
                {
                    StartCoroutine(RevertGates(gates[i], originalSizes[i]));
                }
            }
        }
    }

    private IEnumerator MoveGates(GameObject gate, Vector2 spotSize)
    {
        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, spotSize, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                yield break; // Exit the coroutine if the object is null
            }
        }

        if (gate != null)
        {
            gate.transform.localScale = spotSize; // Ensure exact final size
        }
    }

    private IEnumerator RevertGates(GameObject gate, Vector2 originalSize)
    {
        Vector2 initialScale = gate.transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (gate != null) // Ensure the object is still valid
            {
                gate.transform.localScale = Vector2.Lerp(initialScale, originalSize, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            else
            {
                yield break; // Exit the coroutine if the object is null
            }
        }

        if (gate != null)
        {
            gate.transform.localScale = originalSize; // Ensure exact final size
        }
    }

    void OnDrawGizmos() // comment this out when we're done placing things to keep everything visible
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(enemyAlertTransform.position, enemyAlertRadius);
    }

    List<Collider2D> CheckForEnemies(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] ObjectsToHit = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, enemyLayer);
        List <Collider2D> enemiesInRange = new List<Collider2D>();

        for (int i = 0; i < ObjectsToHit.Length; i++)
        {
            if (ObjectsToHit[i].GetComponent<enemyBase>() != null)
            {
                enemiesInRange.Add(ObjectsToHit[i]);
            }
        }
        return enemiesInRange;
    }
    List<Collider2D> CheckForWalls(Transform _roomTransform, Vector2 _roomArea)
    {
        Collider2D[] potentialWalls = Physics2D.OverlapBoxAll(_roomTransform.position, _roomArea, 0, enemyLayer);
        List<Collider2D> wallsInRange = new List<Collider2D>();

        for (int i = 0; i < potentialWalls.Length; i++)
        {
            if (potentialWalls[i].GetComponent<ForcedEncounterRoomLock>() != null)
            {
                wallsInRange.Add(potentialWalls[i]);
            }
        }
        return wallsInRange;
    }

    void SignalEnemies(List<Collider2D> enemies)
    {
        foreach (Collider2D obj in enemies)
        {
            if(obj.GetComponent<FootSolider>() != null)
            {
                Collider2D playerInRange = obj.GetComponent<FootSolider>().PlayerCheck();
                if (playerInRange != null && !obj.GetComponent<FootSolider>().aggressive)
                {
                    obj.GetComponent<FootSolider>().Alerted(playerInRange, enemyAlertTransform.position);
                }
            }
            obj.GetComponent<enemyBase>().Alerted(enemyAlertTransform.position);
        }
    }
}
