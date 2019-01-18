using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public string playerName;
    public Color playerColor;
    public GameObject turnIndicator;
    public AudioSource aud;
    public Text statText;

    public List<Territory> territories;
    public int Units {
        get {
            int tempUnits = 0;
            foreach (Territory ter in territories) {
                tempUnits += ter.units;
            }
            return tempUnits;
        }
        set {

        }
    }
    public List<Card> playedCards;
    public List<Card> allCards;

    public bool isTurn;
    
    public float fortune;

    #region atkPower Property
    protected float AtkPower {
        get {
            float terAtk = 0;
            foreach (Territory ter in territories) {
                terAtk += ter.Atk;
            }
            return Mathf.RoundToInt(terAtk);
        }
        private set {
        }
    }
    #endregion
    #region defPower Property
    protected float DefPower {
        get {
            float terDef = 0;
            foreach (Territory ter in territories) {
                terDef += ter.Def;
            }
            return Mathf.RoundToInt(terDef);
        }
        set {
        }
    }
    #endregion
    #region mapControl Property
    public int MapControl {
        get {
            int value = Mathf.RoundToInt(territories.Count / GameManager.Instance.allTerritories.Count * 100f);
            return value;
        }
        private set {
        }
    }
    #endregion

    private void Start() {
        foreach (Territory ter in territories) {
            ter.UpdateStats();
        }
    }

    public void UpdateStats() {
        statText.text = "Map Control: " + MapControl + "%" + "\nAttack: " + AtkPower + "\nDefense: " + DefPower + "\nCultists: " + Units;
        foreach (Territory ter in territories) {
            ter.UpdateStats();
        }
    }

    public void DisableAllCards() {
        foreach (Card card in allCards) {
            card.isActive = false;
        }
    }

    public void Upkeep() {
        UpdateStats();
        aud.Play();
        isTurn = true;
        turnIndicator.SetActive(true);
        foreach (Territory ter in territories) {
            ter.SpawnCultist();
        }
        foreach (Territory ter in GameManager.Instance.allTerritories) {
            if (ter.defending > 0)
                ter.defending--;
            ter.UpdateStats();
            UpdateStats();
        }
        foreach (Card card in allCards) {
            card.isActive = true;
        }
    }

    public void EndStep() {
        GameManager.Instance.NextTurn();
        playedCards.Clear();
        isTurn = false;
        foreach (Card card in allCards) {
            card.isActive = false;
        }
        turnIndicator.SetActive(false);
    }
}
