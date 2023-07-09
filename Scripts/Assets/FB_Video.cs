using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FB_Video : MonoBehaviour {

	//VARIABLES
	[SerializeField]private RawImage vidImg;

	private static Sprite blankSprite;
	private static VideoPlayer _player;
	private static AudioSource _audioSource;
	private static ColorAdjustment _ca;
	private static bool isPlaying;

	private static FB_Video instance;
	//CONSTANTS
	private const float DURATION_FADE = 0.5f;
	//EVENTS
	
	//METHODS
	
	void Awake(){
		instance = this;

		_ca = new ColorAdjustment (vidImg, this);
		_ca.AdjustAlpha (0f);
		_player = GetComponent<VideoPlayer> ();
		_audioSource = GetComponent<AudioSource> ();

	}

	public static IEnumerator Play(VideoClip V){
		isPlaying = true;

		BGM currentMusic = FB_Audio.CurrentlyPlaying;
		FB_Audio.StopBGM (DURATION_FADE);

		_audioSource.volume = FB_Audio.VolumeSettingBGM;
		_player.SetTargetAudioSource (0, _audioSource);
		_player.clip = V;
		_player.Prepare ();
		yield return new WaitUntil (() => _player.isPrepared);

		_player.Play ();
		_ca.AdjustColor (Color.black);
		_ca.AdjustAlpha (1f, DURATION_FADE);
		yield return new WaitForSeconds (DURATION_FADE);
	
		_ca.AdjustColor (Color.white);


		while (_player.isPlaying) {
			if (FB_Input.ButtonDown (FB_Button.Cancel)) {
				_player.Stop ();
				_audioSource.Stop ();
			}
			yield return null;
		}
			
		_player.clip = null;
		_player.Stop ();

		_audioSource.Stop ();

		_ca.AdjustAlpha (0f, DURATION_FADE);
		yield return new WaitForSeconds (DURATION_FADE);

		FB_Audio.PlayBGM (currentMusic);
		_player.targetTexture.Release ();
		isPlaying = false;
	}

	public static bool IsPlaying{ get { return isPlaying; } }
}
