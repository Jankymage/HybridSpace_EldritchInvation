using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour {

    public int defending = 0;
    public bool isTargetTer;

    public float baseAtk;
    public float baseDef;
    public int startInhabitants;
    public int units;

    public Player owner;
    public List<Territory> neighbors;
    public GameObject[] trees;
    #region Atk Property
    public float Atk {
        get {
            float value = 0;
            value = baseAtk + units;
            return value;
        }
        private set {
        }
    }
    #endregion
    #region Def Property
    public float Def {
        get {
            float value = 0;
            value = baseDef + units;
            if (defending > 0) {
                value *= defMultiplier;
            }
            else if (defending <= 0 && defend != null) {
                defend.SetActive(false);
            }
            return value;
        }
        private set {
        }
    }
    #endregion

    private int fireIndex = 0;
    private int waterIndex = 0;
    private int forestIndex = 0;

    private int turnsOnFire = 0;
    private int turnsFlooded = 0;
    private int turnsForest = 0;

    private bool isFire;
    private bool isWater;
    private bool isForest;

    private GameObject fire;
    private GameObject water;
    private GameObject forest;
    private GameObject defend;
    private GameObject defendEye;
    private GameObject defendTeeth;

    private List<MeshRenderer> meshes;

    public TextMesh cultistCountText;
    public TextMesh terAtkText;
    public TextMesh terDefText;
    private Territory self;
    private static Territory centerTerritory;

    private static int fireDeaths;
    private static int fireTurnsToSpread;
    private static int floodDeaths;
    private static int floodTurnsToSpread;
    private static int forestLife;
    private static int forestTurnsToSpread;
    private static int turnsToFizzleOut;
    private static float defMultiplier;
    private static int lightningKillAmount;
    private static int chompKillAmount;
    private static int laserKillAmount;
    private static int summonAmount;
    private static int captureAmount;
    private static int passiveSummonAmount;

    private void Start() {
        fireDeaths = Settings.Instance.fireDeaths;
        fireTurnsToSpread = Settings.Instance.fireTurnsToSpread;
        floodDeaths = Settings.Instance.floodDeaths;
        floodTurnsToSpread = Settings.Instance.floodTurnsToSpread;
        forestLife = Settings.Instance.forestLife;
        forestTurnsToSpread = Settings.Instance.forestTurnsToSpread;
        turnsToFizzleOut = Settings.Instance.turnsToFizzleOut;
        defMultiplier = Settings.Instance.defMultiplier;
        lightningKillAmount = Settings.Instance.lightningKillAmount;
        chompKillAmount = Settings.Instance.chompKillAmount;
        laserKillAmount = Settings.Instance.laserKillAmount;
        summonAmount = Settings.Instance.summonAmount;
        captureAmount = Settings.Instance.captureAmount;
        passiveSummonAmount = Settings.Instance.passiveSummonAmount;

        self = GetComponent<Territory>();
        centerTerritory = GameObject.FindGameObjectWithTag("Center").GetComponent<Territory>();
        meshes = new List<MeshRenderer>(this.transform.Find("Meshes").GetComponentsInChildren<MeshRenderer>());
        fire = this.transform.Find("Fire").gameObject;
        water = this.transform.Find("Water").gameObject;
        forest = this.transform.Find("Forest").gameObject;
        defend = this.transform.Find("Defend").gameObject;
        defendEye = defend.transform.Find("DefendEye").gameObject;
        defendTeeth = defend.transform.Find("DefendTeeth").gameObject;
        if (owner != null) {
            owner.territories.Add(self);
            foreach (MeshRenderer mesh in meshes) {
                mesh.material.color = owner.playerColor;
            }
            owner.UpdateStats();
        }
        else {
            foreach (MeshRenderer mesh in meshes) {
                mesh.material.color = GameManager.Instance.villageColor;
            }
        }
        if (units == 0) {
            Summon(startInhabitants);
        }
    }

    public void UpdateStats() {
        cultistCountText.text = units.ToString();
        terAtkText.text = Atk.ToString();
        terDefText.text = Def.ToString();
    }

    public virtual void ChangeOwnership(Player atkPlayer) {
        if (owner != null) {
            owner.territories.Remove(self);
            owner.UpdateStats();
        }
        owner = atkPlayer;
        defending = 0;
        atkPlayer.territories.Add(self);
        foreach (MeshRenderer mesh in meshes) {
            mesh.material.color = owner.playerColor;
        }
        UpdateStats();
    }

    public void SpawnCultist() {
        Summon(passiveSummonAmount);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Attack") {
            GameManager.Instance.SetAttackedTerritory(this);
            Attack card = other.GetComponent<Attack>();
            if (owner != null) {
                if (GameManager.Instance.whoseTurn == card.owner && GameManager.Instance.AttackingTerritory != null && GameManager.Instance.whoseTurn != owner) {
                    TryAttack(other, card);
                }
            }
            else if (owner == null) {
                if (GameManager.Instance.whoseTurn == card.owner && GameManager.Instance.AttackingTerritory != null) {
                    TryAttack(other, card);
                }
            }
        }
        else if (other.tag == "Capture") {
            GameManager.Instance.SetAttackedTerritory(this);
            Capture card = other.GetComponent<Capture>();
            if (owner != null) {
                if (card.isActive && GameManager.Instance.whoseTurn == card.owner && GameManager.Instance.AttackingTerritory != null && GameManager.Instance.whoseTurn != owner) {
                    TryCapture(other, card);
                }
            }
            else if (owner == null) {
                if (card.isActive && GameManager.Instance.whoseTurn == card.owner && GameManager.Instance.AttackingTerritory != null) {
                    TryCapture(other, card);
                }
            }
        }
        else if (other.tag == "Defend") {
            Card card = other.GetComponent<Card>();
            if (owner != null) {
                if (owner == card.owner && defending <= 0) {
                    TryDefend(other, card);
                }
            }
            else if (owner == null) {
                if (defending <= 0) {
                    TryDefend(other, card);
                }
            }
        }
        else if (other.tag == "Summon") {
            Card card = other.GetComponent<Card>();
            if (owner == card.owner) {
                if (card.CardPlayed()) {
                    Summon(summonAmount);
                    if (card.owner.playerName == "Eye")
                        EventManager.Instance.OnStartAnimation("summonEye");
                    if (card.owner.playerName == "Teeth")
                        EventManager.Instance.OnStartAnimation("summonTeeth");
                }
                else if (GameManager.Instance.whoseTurn != card.owner)
                    GameManager.Instance.ShowErrorMessage("That is not your territory");
                else if (GameManager.Instance.whoseTurn == card.owner && !card.isActive)
                    GameManager.Instance.ShowErrorMessage("You already played that card");
            }
        }
        else if (other.tag == "LightningBolt") {
            Card card = other.GetComponent<Card>();
            if (owner != null) {
                if (units > 0 && card.owner != owner) {
                    TryLightning(other, card);
                }
            }
            else if (owner == null) {
                if (units > 0) {
                    TryLightning(other, card);
                }
            }
        }
        else if (other.tag == "Chomp") {
            Card card = other.GetComponent<Card>();
            if (owner != null) {
                if (units > 0 && card.owner != owner) {
                    TryChomp(other, card);
                }
            }
            else if (owner == null) {
                if (units > 0) {
                    TryChomp(other, card);
                }
            }
        }
        else if (other.tag == "Laser") {
            Card card = other.GetComponent<Card>();
            if (owner != null) {
                if (units > 0 && card.owner != owner) {
                    TryLaser(other, card);
                }
            }
            else if (owner == null) {
                if (units > 0) {
                    TryLaser(other, card);
                }
            }
        }
        else if (other.tag == "Fire") {
            Card card = other.GetComponent<Card>();
            if (!isWater && !isFire) {
                if (card.CardPlayed()) {
                    StartFire();
                    EventManager.Instance.OnStartAnimation("fire");
                }
            }
            else if (GameManager.Instance.whoseTurn == card.owner)
                GameManager.Instance.ShowErrorMessage("The fire cannot burn there");
        }
        else if (other.tag == "Flood") {
            Card card = other.GetComponent<Card>();
            if (!isForest && !isWater) {
                if (card.CardPlayed()) {
                    StartFlood();
                    EventManager.Instance.OnStartAnimation("flood");
                }
            }
            else if (GameManager.Instance.whoseTurn == card.owner)
                GameManager.Instance.ShowErrorMessage("The waters cannot reach that place");
        }
        else if (other.tag == "Forest") {
            Card card = other.GetComponent<Card>();
            if (!isFire && !isForest) {
                if (card.CardPlayed()) {
                StartForest();
                EventManager.Instance.OnStartAnimation("forest");
                }
            }
            else if (GameManager.Instance.whoseTurn == card.owner)
                GameManager.Instance.ShowErrorMessage("The forest will not grow there");
        }
    }

    #region Try
    private void TryAttack(Collider other, Attack card) {
        if (card.CardPlayed()) {

        }
        else if (GameManager.Instance.AttackingTerritory == null)
            GameManager.Instance.ShowErrorMessage("No attacked territory given");
        else if (GameManager.Instance.whoseTurn == owner)
            GameManager.Instance.ShowErrorMessage("That is your own territory");
    }

    private void TryCapture(Collider other, Capture card) {
        if (card.CardPlayed()) {
            if (card.owner.playerName == "Eye")
                EventManager.Instance.OnStartAnimation("captureEye");
            else if (card.owner.playerName == "Teeth")
                EventManager.Instance.OnStartAnimation("captureTeeth");
        }
        else if (GameManager.Instance.AttackingTerritory == null)
            GameManager.Instance.ShowErrorMessage("No attacked territory given");
        else if (GameManager.Instance.whoseTurn == owner)
            GameManager.Instance.ShowErrorMessage("That is your own territory");
    }

    private void TryLightning(Collider other, Card card) {
        if (card.CardPlayed()) {
            Kill(lightningKillAmount);
            EventManager.Instance.OnStartAnimation("lightningBolt");
        }
        else if (units == 0)
            GameManager.Instance.ShowErrorMessage("Nothing to kill");
        else if (owner == card.owner)
            GameManager.Instance.ShowErrorMessage("Don't kill you own cultists");
    }

    private void TryChomp(Collider other, Card card) {
        if (card.CardPlayed()) {
            Kill(chompKillAmount);
            if (!isForest && !isWater)
                StartFlood();
            EventManager.Instance.OnStartAnimation("chomp");
            GameManager.Instance.ShowErrorMessage("You consume the enemy");
        }
        else if (units == 0)
            GameManager.Instance.ShowErrorMessage("Nothing to kill");
        else if (owner == card.owner)
            GameManager.Instance.ShowErrorMessage("Don't kill you own cultists");
    }

    private void TryLaser(Collider other, Card card) {
        if (card.CardPlayed()) {
            Kill(laserKillAmount);
            if (!isWater && !isFire)
                StartFire();
            EventManager.Instance.OnStartAnimation("laser");
        }
        else if (units == 0)
            GameManager.Instance.ShowErrorMessage("Nothing to kill");
        else if (owner == card.owner)
            GameManager.Instance.ShowErrorMessage("Don't kill you own cultists");
    }

    private void TryDefend(Collider other, Card card) {
        if (card.CardPlayed()) {
            Defend();
            if (card.owner.playerName == "Eye") {
                EventManager.Instance.OnStartAnimation("defendEye");
                defendEye.SetActive(true);
                defendTeeth.SetActive(false);
                defendEye.transform.parent.gameObject.SetActive(true);
            }
            else if (card.owner.playerName == "Teeth") {
                EventManager.Instance.OnStartAnimation("defendTeeth");
                defendEye.SetActive(false);
                defendTeeth.SetActive(true);
                defendEye.transform.parent.gameObject.SetActive(true);
            }
        }
        else if (defending > 0)
            GameManager.Instance.ShowErrorMessage("You are already defending");
        else if (owner != other.gameObject.GetComponent<Card>().owner)
            GameManager.Instance.ShowErrorMessage("That is not yours to defend");
    }
    #endregion

    #region Combat
    public bool StartCombat() {
        if (GameManager.Instance.AttackedTerritory != null &&
            GameManager.Instance.AttackingTerritory != null &&
            owner != GameManager.Instance.AttackedTerritory.owner &&
            owner == GameManager.Instance.whoseTurn &&
            neighbors.Contains(GameManager.Instance.AttackedTerritory)) {
            InitiateCombat(GameManager.Instance.AttackedTerritory);
            return true;
        }
        else
            return false;
    }

    private void InitiateCombat(Territory defTer) {
        float combatDif = Atk / defTer.Def;

        if (combatDif <= 0.2) {
            GameManager.Instance.ShowMessage("You Lose!", 3);
            if (GameManager.Instance.whoseTurn.playerName == "Eye")
                EventManager.Instance.OnStartCombatAnimation("Defeat", "attackEye");
            if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                EventManager.Instance.OnStartCombatAnimation("Defeat", "attackTeeth");
            LoseUnits();
        }
        else if (combatDif >= 1.5) {
            WinCombat(defTer);
        }
        else if (combatDif > 0.2 && combatDif < 1.5) {
            float result = Random.Range(0f, 1f);
            Debug.Log(result + owner.fortune + "/" + combatDif);
            if (result + owner.fortune >= combatDif) {
                WinCombat(defTer);
            }
            else if (result + owner.fortune < combatDif) {
                owner.fortune += combatDif - result;
                GameManager.Instance.ShowMessage("You Lose!", 3);
                if (GameManager.Instance.whoseTurn.playerName == "Eye")
                    EventManager.Instance.OnStartCombatAnimation("Defeat", "attackEye");
                if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                    EventManager.Instance.OnStartCombatAnimation("Defeat", "attackTeeth");
                LoseUnits();
            }
        }
        GameManager.Instance.ClearCombatTerritories();
        UpdateStats();
        if (owner != null)
            owner.UpdateStats();
        defTer.UpdateStats();
        if (defTer.owner != null)
            defTer.owner.UpdateStats();
    }

    private void LoseUnits() {
        int cultistLoss = Mathf.RoundToInt(units * GameManager.Instance.cultistLossPercentage);
        units -= cultistLoss;

        owner.UpdateStats();
        UpdateStats();
    }

    protected virtual void WinCombat(Territory defTer) {
        defTer.ChangeOwnership(owner);
        defTer.units += Mathf.RoundToInt(units / 2);
        units -= Mathf.RoundToInt(units / 2);
        owner.fortune = 0;
        if (!defTer.isTargetTer) {
            GameManager.Instance.ShowMessage("You Win!", 3);
            if (GameManager.Instance.whoseTurn.playerName == "Eye")
                EventManager.Instance.OnStartCombatAnimation("Victory", "attackEye");
            if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                EventManager.Instance.OnStartCombatAnimation("Victory", "attackTeeth");
        }
        else if (defTer.isTargetTer)
            GameManager.Instance.OnGameWin(owner);
    }
    #endregion

    #region Capture
    public bool StartCapture() {
        if (GameManager.Instance.AttackedTerritory != null &&
            GameManager.Instance.AttackingTerritory != null &&
            owner != GameManager.Instance.AttackedTerritory.owner &&
            owner == GameManager.Instance.whoseTurn &&
            neighbors.Contains(GameManager.Instance.AttackedTerritory)) {
            InitiateCapture(GameManager.Instance.AttackedTerritory);
            return true;
        }
        else if (owner == GameManager.Instance.AttackedTerritory.owner && owner == GameManager.Instance.whoseTurn) {
            GameManager.Instance.ShowErrorMessage("You can't capture your own cultist");
            return false;
        }
        else if (!neighbors.Contains(GameManager.Instance.AttackedTerritory) && owner == GameManager.Instance.whoseTurn) {
            GameManager.Instance.ShowErrorMessage("That territory is out of reach");
            return false;
        }
        else {
            GameManager.Instance.ShowErrorMessage("Cannot capture from that territory");
            return false;
        }
    }

    private void InitiateCapture(Territory defTer) {
        float combatDif = Atk / defTer.Def;

        if (combatDif <= 0.2) {
            GameManager.Instance.ShowMessage("You Lose!", 3);
            if (GameManager.Instance.whoseTurn.playerName == "Eye")
                EventManager.Instance.OnStartCombatAnimation("Defeat", "attackEye");
            if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                EventManager.Instance.OnStartCombatAnimation("Defeat", "attackTeeth");
        }
        else if (combatDif >= 1.5) {
            CaptureCultists(defTer, Settings.Instance.captureAmount);
            GameManager.Instance.ShowMessage("You Win!", 3);
            if (GameManager.Instance.whoseTurn.playerName == "Eye")
                EventManager.Instance.OnStartCombatAnimation("Victory", "attackEye");
            if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                EventManager.Instance.OnStartCombatAnimation("Victory", "attackTeeth");
        }
        else if (combatDif > 0.2 && combatDif < 1.5) {
            float result = Random.Range(0f, 1f);
            Debug.Log(result + owner.fortune + "/" + combatDif);
            if (result + owner.fortune >= combatDif) {
                CaptureCultists(defTer, captureAmount);
                GameManager.Instance.ShowMessage("You Win!", 3);
                if (GameManager.Instance.whoseTurn.playerName == "Eye")
                    EventManager.Instance.OnStartCombatAnimation("Victory", "attackEye");
                if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                    EventManager.Instance.OnStartCombatAnimation("Victory", "attackTeeth");
            }
            else if (result + owner.fortune < combatDif) {
                if (GameManager.Instance.whoseTurn.playerName == "Eye")
                    EventManager.Instance.OnStartCombatAnimation("Defeat", "attackEye");
                if (GameManager.Instance.whoseTurn.playerName == "Teeth")
                    EventManager.Instance.OnStartCombatAnimation("Defeat", "attackTeeth");
            }
        }
        GameManager.Instance.ClearCombatTerritories();
        UpdateStats();
        if (owner != null)
            owner.UpdateStats();
        defTer.UpdateStats();
        if (defTer.owner != null)
            defTer.owner.UpdateStats();
    }

    private void CaptureCultists(Territory defTer, int amount) {
        if (defTer.owner != null) {
            for (int i = 0; i < amount; i++) {
                defTer.Kill(amount);
                Summon(amount);
            }
            GameManager.Instance.ShowMessage(owner.playerName + " wins!", 3);
        }
        if (defTer.owner == null) {
            Summon(captureAmount);
        }
    }
    #endregion

    #region Forest, Flood & Fire
    public void OnPlayerUpkeep() {
        if (isFire) {
            if (units > 0)
                Kill(fireDeaths);
            if (owner != null) {
                owner.UpdateStats();
            }
            fireIndex++;
            turnsOnFire++;
            if (fireIndex >= fireTurnsToSpread && neighbors != null) {
                for (int i = 0; i < neighbors.Count; i++) {
                    if (!neighbors[i].isFire && !neighbors[i].isWater) {
                        neighbors[i].StartFire();
                        break;
                    }
                }
                fireIndex = 0;
            }
            if (turnsOnFire > turnsToFizzleOut) {
                isFire = false;
                fire.SetActive(false);
                turnsOnFire = 0;
            }
        }
        else if (!isFire)
            fireIndex = 0;
        if (isWater) {
            if (units > 0)
                Kill(floodDeaths);
            if (owner != null) {
                owner.UpdateStats();
            }
            waterIndex++;
            turnsFlooded++;
            if (waterIndex >= floodTurnsToSpread && neighbors != null) {
                for (int i = 0; i < neighbors.Count; i++) {
                    if (!neighbors[i].isWater && !neighbors[i].isForest) {
                        neighbors[i].StartFlood();
                        break;
                    }
                }
                waterIndex = 0;
            }
            if (turnsFlooded > turnsToFizzleOut) {
                isWater = false;
                water.SetActive(false);
                turnsFlooded = 0;
            }
        }
        else if (!isWater)
            waterIndex = 0;
        if (isForest) {
            forestIndex++;
            turnsForest++;
            if (forestIndex >= forestTurnsToSpread && neighbors != null) {
                for (int i = 0; i < neighbors.Count; i++) {
                    if (!neighbors[i].isFire && !neighbors[i].isForest) {
                        neighbors[i].StartForest();
                        break;
                    }
                }
                forestIndex = 0;
            }
            if (turnsForest > turnsToFizzleOut) {
                isForest = false;
                forest.SetActive(false);
                turnsForest = 0;
            }
        }
        else if (!isForest)
            waterIndex = 0;
    }

    public void StartFire() {
        isForest = false;
        foreach (GameObject tree in trees) {
            tree.GetComponent<Animator>().SetTrigger("Die");
        }
        isFire = true;
        forest.SetActive(false);
        fire.SetActive(true);
        if (units > 0) {
            Kill(fireDeaths);
        }
        if (owner != null) {
            owner.UpdateStats();
        }
    }

    public void StartFlood() {
        isFire = false;
        isWater = true;
        fire.SetActive(false);
        water.SetActive(true);
        if (units > 0) {
            Kill(floodDeaths);
        }
        if (owner != null) {
            owner.UpdateStats();
        }
    }

    public void StartForest() {
        isWater = false;
        isForest = true;
        water.SetActive(false);
        forest.SetActive(true);
        centerTerritory.baseAtk += forestLife;
        centerTerritory.baseDef += forestLife;
        centerTerritory.UpdateStats();
    }
    #endregion

    #region Other Cards
    private void Defend() {
        defending = 2;
        UpdateStats();
        if (owner != null)
            owner.UpdateStats();
    }

    private void Kill(int amount) {
        int killAmount = Mathf.Min(units, amount);
        units -= killAmount;
        UpdateStats();
        if (owner != null)
            owner.UpdateStats();
    }

    private void Summon(int amount) {
        units += amount;
        UpdateStats();
        if (owner != null)
            owner.UpdateStats();
        UpdateStats();
    }
    #endregion
}
