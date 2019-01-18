using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    #region Singleton
    private static EventManager _instance;

    public static EventManager Instance {
        get {
            if (_instance == null) {
                GameObject instance = new GameObject("Event Manager");
                instance.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    public delegate void AnimationDelegate(string cardName);
    public delegate void CombatAnimationDelegate(string result, string cardName);
    public static event AnimationDelegate StartAnimation;
    public static event CombatAnimationDelegate StartCombatAnimation;

    public void OnStartAnimation(string cardName) {
        if (StartAnimation != null)
            StartAnimation(cardName);
    }

    public void OnStartCombatAnimation(string result, string cardName) {
        if (StartCombatAnimation != null)
            StartCombatAnimation(result, cardName);
    }
}
