using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    #region singleton
    private static Settings _instance;

    public static Settings Instance {
        get {
            if (_instance == null) {
                GameObject instance = new GameObject("Settings");
                instance.AddComponent<Settings>();
            }

            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    public int fireDeaths = 10;
    public int fireTurnsToSpread = 3;
    public int floodDeaths = 10;
    public int floodTurnsToSpread = 3;
    public int forestLife = 10;
    public int forestTurnsToSpread = 3;
    public int turnsToFizzleOut = 10;

    public float defMultiplier = 1.5f;
    public int lightningKillAmount = 15;
    public int chompKillAmount = 10;
    public int laserKillAmount = 10;
    public int summonAmount = 50;
    public int captureAmount = 10;
    public int passiveSummonAmount = 10;
}
