using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTester : MonoBehaviour {

	public void EyeAttack() {
        EventManager.Instance.OnStartAnimation("attackEye");
    }

    public void TeethAttack() {
        EventManager.Instance.OnStartAnimation("attackTeeth");
    }

    public void EyeCapture() {
        EventManager.Instance.OnStartAnimation("captureEye");
    }

    public void TeethCapture() {
        EventManager.Instance.OnStartAnimation("captureTeeth");
    }

    public void EyeDefend() {
        EventManager.Instance.OnStartAnimation("defendEye");
    }

    public void TeethDefend() {
        EventManager.Instance.OnStartAnimation("defendTeeth");
    }

    public void Flood() {
        EventManager.Instance.OnStartAnimation("flood");
    }

    public void Laser() {
        EventManager.Instance.OnStartAnimation("laser");
    }

    public void LightningBolt() {
        EventManager.Instance.OnStartAnimation("lightningBolt");
    }
}
