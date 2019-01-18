using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public AudioSource aud;
    public AudioClip[] clips;
    private AudioClip currentSong;
    private bool succesful;

	void Start () {
        currentSong = clips[Random.Range(0, clips.Length - 1)];
        aud.clip = currentSong;
        aud.Play();
	}
	
	void Update () {
		if (!aud.isPlaying) {
            while (!succesful) {
                AudioClip newSong = clips[Random.Range(0, clips.Length - 1)];
                if (newSong != currentSong) {
                    currentSong = newSong;
                    succesful = true;
                }
            }
            aud.Play();
        }
	}
}
