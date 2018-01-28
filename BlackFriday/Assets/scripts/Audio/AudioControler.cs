using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to controll the audio in the game.
/// </summary>
public class AudioControler : MonoBehaviour {

    public AudioSource[] audios;

    AudioSource activeSong;
    AudioSource lastSong;

    float fadeDuration;
    float fadeDurationLeft;
    bool fading;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (fading)
        {
            float lerp_value = fadeDurationLeft / fadeDuration;
            if (lerp_value <= 0.0f)
            {
                lerp_value = 0.0f;
            }
            fadeDurationLeft -= Time.deltaTime;
            if(lastSong) lastSong.volume = Mathf.Lerp(0.0f, 1.0f, lerp_value);
            activeSong.volume = Mathf.Lerp(0.0f, 1.0f, 1.0f - lerp_value);
            Debug.Log(lerp_value);
        }
	}

    //Plays a song registered in the AudioController fading up the volume
    //and if there is one actually playing fades down the last song.
    public void PlayWithFade(float seconds, int songToPlay)
    {
        if (activeSong)
        {
            lastSong = activeSong;
        }
        
        activeSong = audios[songToPlay];
        activeSong.Play();
        activeSong.volume = 0.0f;
        fading = true;
        fadeDuration = seconds;
        fadeDurationLeft = fadeDuration;
    }

    //Plays a song at the moment without fading.
    public void PlayWithoutFade(int songToPlay)
    {
        if (lastSong) lastSong.Stop();
        if (activeSong) activeSong.Stop();
        activeSong = audios[songToPlay];
        activeSong.Play();
        activeSong.volume = 1.0f;
    }
}
