using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

[System.Serializable]

public class GameData
{
    // player properties

    public Vector3 position;
    public int maxHealth;
    public float maxEnergy;
    public bool doHitStop;

    // upgrades

    public bool canDash;
    public bool darkUnlocked;
    public bool lightUnlocked;

    // current attack state

    public PlayerController.AttackType currentAttackType;

    // location name will be added later for save file screen

    // one time use objects

    public SerializableDictionary<string, bool> upgradeStatus;

    //Forced Encounter Manager

    public SerializableDictionary<string, bool> FEMStatus;

    //Fuse Box

    public SerializableDictionary<string, bool> fbStatus;

    //Laser Reciever

    public SerializableDictionary<string, bool> laserRecieverStatus;

    //Switches

    public SerializableDictionary<string, bool> switchTriggerStatus;


    // Cutscene Triggers

    public SerializableDictionary<string, bool> cutsceneTriggerStatus;

    public GameData()
    {
        this.position = new Vector3(-4.48f, 4.2f, 0); // start of level 1
        this.maxHealth = 8;
        this.maxEnergy = 20;
        doHitStop = true;
        canDash = false;
        darkUnlocked = false;
        lightUnlocked = false;
        currentAttackType = PlayerController.AttackType.Neutral;
        upgradeStatus = new SerializableDictionary<string, bool>();
        FEMStatus = new SerializableDictionary<string, bool>();
        fbStatus = new SerializableDictionary<string, bool>();
        laserRecieverStatus = new SerializableDictionary<string, bool>();
        switchTriggerStatus = new SerializableDictionary<string, bool>();
        cutsceneTriggerStatus = new SerializableDictionary<string, bool>();
    }

}
