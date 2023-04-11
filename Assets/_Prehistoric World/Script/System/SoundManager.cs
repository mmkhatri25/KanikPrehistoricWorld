using UnityEngine;
using System.Collections;
/*
 * This is SoundManager
 * In other script, you just need to call SoundManager.PlaySfx(AudioClip) to play the sound
*/
public class SoundManager : MonoBehaviour {
	public static SoundManager Instance;
    public AudioClip beginSoundInMainMenu;
	[Tooltip("Play music clip when start")]
	public AudioClip musicsMenu;
	[Range(0,1)]
	public float musicMenuVolume = 0.5f;
	public AudioClip musicsGame;
	[Range(0,1)]
	public float musicsGameVolume = 0.5f;
	[Tooltip("Place the sound in this to call it in another script by: SoundManager.PlaySfx(soundname);")]
	public AudioClip soundClick;
    public AudioClip soundGamefinish;
	public AudioClip soundGameover;
	public AudioClip soundPause;
	private AudioSource musicAudio;
	private AudioSource soundFx;
	public AudioClip soundCheckpoint;
	public void PauseMusic(bool isPause){
		if (isPause)
			Instance.musicAudio.mute = true;
		else
			Instance.musicAudio.mute = false;
	}
	//GET and SET
	public static float MusicVolume{
		
		set{ Instance.musicAudio.volume = value; }
		get{ return Instance.musicAudio.volume; }
	}
	public static float SoundVolume{
		set{ Instance.soundFx.volume = value; }
		get{ return Instance.soundFx.volume; }
	}
	// Use this for initialization
	void Awake(){
		Instance = this;
		musicAudio = gameObject.AddComponent<AudioSource> ();
		musicAudio.loop = true;
		musicAudio.volume = 0.5f;
		soundFx = gameObject.AddComponent<AudioSource> ();
	}
	void Start () {
		PlayMusic (musicsGame, musicsGameVolume);
	}

	public static void Click(){
		PlaySfx (Instance.soundClick);
	}

	public  void ClickBut(){
		PlaySfx (soundClick);
	}

	public static void PlaySfx(AudioClip clip){
		if (Instance != null) {
			Instance.PlaySound (clip, Instance.soundFx);
			Debug.Log (clip);
		}


	}

	public static void PlaySfx(AudioClip clip, float volume){
		if (Instance!=null)
		Instance.PlaySound(clip, Instance.soundFx, volume);
	}

	public static void PlayMusic(AudioClip clip){
		Instance.PlaySound (clip, Instance.musicAudio);
	}

	public static void PlayMusic(AudioClip clip, float volume){
		Instance.PlaySound (clip, Instance.musicAudio, volume);
	}

    public static void ResetMusic()
    {
        Instance.musicAudio.Stop();
        Instance.musicAudio.Play();
    }

    private void PlaySound(AudioClip clip,AudioSource audioOut){

		if (clip == null) {
			return;
		}

		if (Instance == null)
			return;

		if (audioOut == musicAudio) {
			audioOut.clip = clip;
			audioOut.Play ();
		} else
			audioOut.PlayOneShot (clip, SoundVolume);
	}

	private void PlaySound(AudioClip clip,AudioSource audioOut, float volume){
		if (clip == null) {
			return;
		}

		if (audioOut == musicAudio) {
			audioOut.volume = volume;
			audioOut.clip = clip;
			audioOut.Play ();
		} else
			audioOut.PlayOneShot (clip, SoundVolume * volume);
	}
}
