using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour, IDataPersistence
{
    [Header("Volume")]
    [Range(0, 1)]

    public float masterVolume = 1;
    [Range(0, 1)]

    public float musicVolume = 1;
    [Range(0, 1)]

    public float sfxVolume = 1;
    

    

    private Bus masterBus;

    private Bus musicBus;

    private Bus sfxBus;

    

    private List<EventInstance> eventInstances;

    private EventInstance musicEventInstance;
    public static AudioManager instance {  get; private set; }

    public void LoadData(GameData data)
    {
        sfxVolume = data.sfxSlider;
        musicVolume = data.musicSlider;
        masterVolume = data.masterSlider;
    }
    public void SaveData(GameData data)
    {
        data.sfxSlider = sfxVolume;
        data.musicSlider = musicVolume;
        data.masterSlider = masterVolume;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
        

        //If the game is paused and not in settings, don't play music
        if (PauseMenu.GamePaused && !PauseMenu.inSettings)
        {
            musicEventInstance.setPaused(true);
            sfxBus.setMute(true);
        }

        //If the game is paused and in settings, play music and sfx
        else if (PauseMenu.GamePaused && PauseMenu.inSettings)
        {
            musicEventInstance.setPaused(false);
            sfxBus.setMute(false);
        }

        //If the game is not paused and not in settings, play music
        else if (!PauseMenu.GamePaused && !PauseMenu.inSettings) 
        {
            musicEventInstance.setPaused(false);
            sfxBus.setMute(false);
        }
    }
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    public void SetMusicArea(MusicArea area)
    {
        musicEventInstance.setParameterByName("area", (float) area);
    }
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
