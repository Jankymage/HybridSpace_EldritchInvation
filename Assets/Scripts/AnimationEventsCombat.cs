using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsCombat : AnimationEvents {

    public string result;
    public AudioClip[] clipsLoss;

    public override void PlayAudio() {
        if (result == "Victory") {
            GameManager.Instance.sfxSource.clip = clips[Random.Range(0, clips.Length - 1)];
            GameManager.Instance.sfxSource.Play();
        }
        else if (result == "Defeat") {
            GameManager.Instance.sfxSource.clip = clipsLoss[Random.Range(0, clips.Length - 1)];
            GameManager.Instance.sfxSource.Play();
        }
    }
}
