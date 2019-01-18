using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    #region singleton
    private static GameManager _instance;

    public static GameManager Instance {
        get {
            if (_instance == null) {
                GameObject instance = new GameObject("Game Manager");
                instance.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    void Awake() {
        _instance = this;
    }
    #endregion

    public Color villageColor;
    public AudioSource sfxSource;
    public Player whoseTurn;
    public int cardsPerTurn;

    public float cultistLossPercentage;

    public Territory AttackedTerritory { get; private set; }
    public Territory AttackingTerritory { get; private set; }
    public Text messageText;
    public Text errorMessageText;
    public List<Territory> allTerritories;

    public GameObject attackEye;
    public GameObject attackTeeth;
    public GameObject captureEye;
    public GameObject captureTeeth;
    public GameObject defendEye;
    public GameObject defendTeeth;
    public GameObject flood;
    public GameObject laser;
    public GameObject lightningBolt;
    public GameObject chomp;
    public GameObject summonEye;
    public GameObject summonEye2;
    public GameObject summonTeeth;
    public GameObject summonTeeth2;
    public GameObject fire;
    public GameObject forest;
    public GameObject winEye;
    public GameObject winTeeth;
    private Dictionary<string, GameObject> animationPrefabs;

    private Player[] players;
    private int turnIndex;

    private void OnEnable() {
        EventManager.StartAnimation += DisplayAnimation;
        EventManager.StartCombatAnimation += DisplayCombatAnimation;
    }

    private void OnDisable() {
        EventManager.StartAnimation -= DisplayAnimation;
        EventManager.StartCombatAnimation -= DisplayCombatAnimation;
    }

    private void Start() {
        players = GameObject.FindObjectsOfType<Player>();
        whoseTurn = players[Random.Range(0, players.Length - 1)];
        players[turnIndex].Upkeep();
        allTerritories = new List<Territory>(GameObject.FindObjectsOfType<Territory>());
        animationPrefabs = new Dictionary<string, GameObject>();
        animationPrefabs.Add("attackEye", attackEye);
        animationPrefabs.Add("attackTeeth", attackTeeth);
        animationPrefabs.Add("captureEye", captureEye);
        animationPrefabs.Add("captureTeeth", captureTeeth);
        animationPrefabs.Add("defendEye", defendEye);
        animationPrefabs.Add("defendTeeth", defendTeeth);
        animationPrefabs.Add("flood", flood);
        animationPrefabs.Add("laser", laser);
        animationPrefabs.Add("lightningBolt", lightningBolt);
        animationPrefabs.Add("chomp", chomp);
        animationPrefabs.Add("summonEye", summonEye);
        animationPrefabs.Add("summonTeeth", summonTeeth);
        animationPrefabs.Add("fire", fire);
        animationPrefabs.Add("forest", forest);
        animationPrefabs.Add("winEye", winEye);
        animationPrefabs.Add("winTeeth", winTeeth);
    }

    public void DisplayAnimation(string _cardName) {
        animationPrefabs[_cardName].SetActive(true);
    }

    public void DisplayCombatAnimation(string _result, string _cardName) {
        animationPrefabs[_cardName].SetActive(true);
        animationPrefabs[_cardName].GetComponent<AnimationEventsCombat>().result = _result;
    }

    public void SetAttackedTerritory(Territory _ter) {
        AttackedTerritory = _ter;

        Territory tmpAttackingTerritory = null;
        Territory prevTer = null;
        foreach (Territory ter in _ter.neighbors) {
            if (ter.owner == whoseTurn) {
                if (prevTer == null)
                    tmpAttackingTerritory = ter;
                else if (ter.Atk > prevTer.Atk)
                    tmpAttackingTerritory = ter;
            }
        }
        AttackingTerritory = tmpAttackingTerritory;
    }

    public void ClearCombatTerritories() {
        AttackedTerritory = null;
        AttackingTerritory = null;
    }
    
    public void TurnOffCards() {
        foreach (Player player in players) {
            player.DisableAllCards();
        }
    }

    public void NextTurn() {
        turnIndex++;
        if (turnIndex >= players.Length) {
            turnIndex = 0;
            foreach (Territory ter in allTerritories) {
                ter.OnPlayerUpkeep();
            }
        }
        whoseTurn = players[turnIndex];
        players[turnIndex].Upkeep();
    }

    public void ShowMessage(string _text, int _dur) {
        StartCoroutine(DisplayMessage(messageText, _text, _dur));
    }

    public void ShowErrorMessage(string _text) {
        errorMessageText.text = _text;
    }

    private IEnumerator DisplayMessage(Text _textBox, string _message, int _duration) {
        _textBox.text = _message;
        _textBox.enabled = true;
        yield return new WaitForSeconds(_duration);
        _textBox.text = null;
        _textBox.enabled = false;
    }

    public void OnGameWin(Player winPlayer) {
        StartCoroutine(GameWin(winPlayer));
    }

    private IEnumerator GameWin(Player winPlayer) {
        ShowMessage(winPlayer.playerName + " wins!", 10);
        if (winPlayer.playerName == "Eye") {
            EventManager.Instance.OnStartAnimation("winEye");
        }
        else if (winPlayer.playerName == "Teeth") {
            EventManager.Instance.OnStartAnimation("winTeeth");
        }
        yield return new WaitForSeconds(8);
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
