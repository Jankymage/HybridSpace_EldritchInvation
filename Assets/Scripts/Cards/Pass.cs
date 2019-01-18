using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Pass : MonoBehaviour, ITrackableEventHandler {

    public Player owner;

    protected AudioSource aud;

    private TrackableBehaviour mTrackableBehaviour;

    private void Start() {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        aud = this.GetComponent<AudioSource>();
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
            if (GameManager.Instance.whoseTurn == owner) {
                owner.EndStep();
            }
        }
    }
}
