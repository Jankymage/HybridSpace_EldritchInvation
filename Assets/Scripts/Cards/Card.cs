using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Card : MonoBehaviour, ITrackableEventHandler {

    public Player owner;
    public bool isActive;

    private TrackableBehaviour mTrackableBehaviour;
    private BoxCollider boxCollider;
    private bool tracking;
    private static float activationDelay = 0.5f;
    private float delayTimer;

    void Start() {
        boxCollider = GetComponent<BoxCollider>();
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
            tracking = true;
            boxCollider.enabled = false;
            delayTimer = 0;
        }
        else if (newStatus == TrackableBehaviour.Status.NO_POSE || newStatus == TrackableBehaviour.Status.LIMITED) {
            tracking = false;
            boxCollider.enabled = false;
        }
    }

    private void Update() {
        if (tracking && delayTimer < activationDelay) {
            delayTimer += Time.deltaTime;
            if(delayTimer >= activationDelay) {
                boxCollider.enabled = true;
            }
        }
    }

    public virtual bool CardPlayed() {
        if (GameManager.Instance.whoseTurn == owner && isActive) {
            owner.playedCards.Add(this);
            isActive = false;
            if (owner.playedCards.Count >= GameManager.Instance.cardsPerTurn) {
                owner.EndStep();
            }
            return true;
        }
        return false;
    }
}
