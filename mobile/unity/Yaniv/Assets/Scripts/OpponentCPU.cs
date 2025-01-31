using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class OpponentCPU: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
    }

    private int calculateScore(List<Card> hand) {
        return hand.Sum(card => card.value);
    }

    public GameTurnResult simplePlayTurn(List<Card> hand, List<Card> discardPile) {
        GameTurnResult result = new GameTurnResult() {
            discards = new List<Card>(),
            discardDraw = null,
            deckDraw = false,
            isYaniv = false
        };
        
        int startingScore = calculateScore(hand);

        if(startingScore <= 7) {
            // If the score of the hand is 7 or less, call Yaniv
            result.isYaniv = true;
            return result;
        }

        List<Card> orderedHand = hand.OrderByDescending(card => card.rank).ToList();

        result.discards.Add(orderedHand[0]);

        for(int i = 1; i < orderedHand.Count; i++) {
            Card card = orderedHand[i];
            if(card.rank == orderedHand[0].rank) {
                result.discards.Add(card);
            }
        }

        List<Card> orderedDiscardPile = discardPile.OrderBy(card => card.rank).ToList();

        if (result.discards[0].rank >= orderedDiscardPile[0].rank) {
            // If the rank of the card to discard is greater than or equal to the rank of the top card in the discard pile, discard the cards
            result.discardDraw = orderedDiscardPile[0];
        } else {
            result.deckDraw = true;
        }
        return result;
    }

    public GameTurnResult moderatePlayTurn(List<Card> hand, List<Card> discardPile)
    {
        throw new System.NotImplementedException();
    }
}

public class GameTurnResult {
    public List<Card> discards;
    public Card discardDraw;
    public bool deckDraw;

    public bool isYaniv;
}