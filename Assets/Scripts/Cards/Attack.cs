using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Attack : Card {

    public override bool CardPlayed() {
        if (GameManager.Instance.whoseTurn == owner && isActive) {
            if (GameManager.Instance.AttackingTerritory.StartCombat()) {
                owner.playedCards.Add(this);
                isActive = false;
                if (owner.playedCards.Count >= GameManager.Instance.cardsPerTurn) {
                    owner.EndStep();
                }
                return true;
            }
            return false;
        }
        return false;
    }
}
