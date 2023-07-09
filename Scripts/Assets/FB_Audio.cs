//
//    `````              Code founded by Alex Mankin                       
// .:///////:-.                               -.    
//./////////////:-`          `....``    ```.-://:-` 
//`/////////////////:.``  .:///////////////////+yy/-
//  `..---:://////////////////////////////////::::.`
//           `-://////////////////////////////`     
//                ```````.//////::::--..-////.      
//                      -//////-       `--``-/`     
//                      -//:.`         .     ./.    
//                        -::.                ``  
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Audio : MonoBehaviour {



    //VARIABLES
    private static AudioSource bgm, txtSrc;
    private static AudioSource[] sfx;

    public static float VolumeSettingBGM;
    public static float VolumeSettingSFX;

    private static float bgmAdjust; //Adjust this in-game to modify volume with respect to volume setting
    private static float volumeInitial;

    private static Timer bgmFadeTimer;

    private static FB_Audio instance;

    private static Coroutine fadeRoutine;

    private static BGM currentMusic;
    private static bool lockBGM;

    //CONSTANTS
    public static readonly string AUDIO_EXT = ".ogg";
    private static readonly int NUM_SFX_CHANNELS = 32;
    private static readonly float DEFAULT_BGM_VOLUME = 0.5f, MAX_BGM_VOLUME = 0.8f;
    private static readonly float DEFAULT_SFX_VOLUME = 0.8f, MAX_SFX_VOLUME = 1f;

    // Use this for initialization
    void Awake() {

        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        GameObject.DontDestroyOnLoad(gameObject);

        generateBGMObject();
        generateSFXOBjects();

        VolumeSettingBGM = DEFAULT_BGM_VOLUME;
        VolumeSettingSFX = DEFAULT_SFX_VOLUME;
        bgmAdjust = 1f;

        bgmFadeTimer = new Timer();
    }

    void OnDestroy() {

    }

    void Update() {
        bgm.volume = bgmAdjust * VolumeSettingBGM * MAX_BGM_VOLUME;
    }

    public static void PlayBGM(BGM ID, float t = 0f) {

        if (ID == BGM._NULL) {
            StopBGM();
            return;
        }

        //if (MG_Debug.Instance.RadioMode && MG_Debug.Enabled)
        //	return;

        bgmFadeTimer.Clear();

        if (fadeRoutine != null)
            instance.StopCoroutine(fadeRoutine);



        if (ID != currentMusic) {
            bgm.Stop();
            bgmAdjust = 1f;
            bgm.clip = AudioDatabase.GetBGM(ID);

            fadeRoutine = instance.StartCoroutine(crFade(1f, t));

            if (bgm.clip != null) {
                bgm.Play();
                currentMusic = ID;
            }
            else {
                Debug.LogWarning("Could not find song: " + ID + "\nPlease reimport audio.");
            }
        }
    }
    public static void PlayBGM() {
        if (Map.current != null) {
            if (Map.current.defaultBGM != BGM._NULL) {
                LockBGM = false;
                PlayBGM(Map.current.defaultBGM);
            }
        }
    }

    public static void StopBGM(float duration) {
        currentMusic = BGM._NULL;

        if (Helpers.IsFloatZero(duration)) {
            bgm.Stop();
            return;
        }

        if (fadeRoutine != null) instance.StopCoroutine(fadeRoutine);
        fadeRoutine = instance.StartCoroutine(crFade(0f, duration));
    }
    public static void StopBGM() {
        StopBGM(0f);
    }

    public static void SetBGMPause(bool isPaused) {
        if (isPaused) bgm.Pause();
        else bgm.UnPause();
    }

    public static int PlaySFX(SFX ID, float volume = 1f) {
        if (ID != SFX._NULL) {
            return PlaySFX(AudioDatabase.GetSFX(ID), volume);
        }

        return -1;
    }
    public static int PlaySFX(AudioClip clip, float volume = 1f) {
        if (clip != null) {
            for (int i = 0; i < sfx.Length; ++i) {
                if (!sfx[i].isPlaying) {
                    sfx[i].clip = clip;
                    sfx[i].volume = MAX_SFX_VOLUME * VolumeSettingSFX * volume;
                    sfx[i].Play();
                    return i;
                }
            }
            Debug.LogWarning("Could not play sound effect " + clip.name + ".  Consider adding more channels.");
        }

        return -1;
    }

    public static AudioSource GetSFXChannel(int i) {
        return sfx[i];
    }

    private static IEnumerator crFade(float volumeFinal, float duration) {
        volumeInitial = bgm.volume;
        bgmFadeTimer.Begin(duration);

        if (Helpers.IsFloatZero(volumeFinal))
            currentMusic = BGM._NULL;

        while (bgmFadeTimer.IsRunning) {
            bgm.volume = volumeInitial + (bgmFadeTimer.Percentage * (volumeFinal - volumeInitial));
            yield return null;
        }

        bgm.volume = volumeFinal;

        if (Helpers.IsFloatZero(volumeFinal))
            bgm.Stop();
    }

    void generateBGMObject() {
        GameObject O = new GameObject("BGM");
        O.transform.parent = transform;
        bgm = O.AddComponent<AudioSource>();
        bgm.loop = true;
        bgm.playOnAwake = false;
    }

    void generateSFXOBjects() {
        GameObject O;
        sfx = new AudioSource[NUM_SFX_CHANNELS];
        for (int i = 0; i < sfx.Length; ++i) {
            O = new GameObject();
            O.name = "SFX" + i;
            O.transform.parent = transform;
            sfx[i] = O.AddComponent<AudioSource>();
        }

        O = new GameObject();
        O.name = "SFX_Dialogue";
        O.transform.parent = transform;
        txtSrc = O.AddComponent<AudioSource>();
    }

    public static BGM CurrentlyPlaying {
        get {
            if (!bgm.isPlaying)
                return BGM._NULL;

            return currentMusic;
        }
    }

    public static AudioSource GetDialogueSource {
        get { return txtSrc; }
    }

    /*
	public static float VolumeBGMFinal{
		get { return bgmVolume * MAX_BGM_VOLUME; }
		set { bgmVolume = value; 
			bgm.volume = bgmVolume;
			if (bgm.volume >= MAX_BGM_VOLUME * value)
				bgm.volume = MAX_BGM_VOLUME * value;
		}
	}

	public static float VolumeSFXFinal{
		get { return sfxVolume * MAX_SFX_VOLUME; }
		set { sfxVolume = value; 
			for (int i = 0; i < sfx.Length; ++i) {
				sfx [i].volume = sfxVolume;
				if (sfx[i].volume >= MAX_SFX_VOLUME * value)
					sfx[i].volume = MAX_SFX_VOLUME * value;
			}
		}
	}
	*/

    public static bool IsFading {
        get { return (bgmFadeTimer.IsRunning); }
    }

    public static bool LockBGM {
        get {
            return lockBGM;
        }
        set {
            lockBGM = value;
        }
    }


}
