using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour {

    public AudioClip[] clips;

    public void Deactivate() {
        this.gameObject.SetActive(false);
    }

    public virtual void PlayAudio() {
        GameManager.Instance.sfxSource.clip = clips[Random.Range(0, clips.Length - 1)];
        GameManager.Instance.sfxSource.Play();
    }

}
