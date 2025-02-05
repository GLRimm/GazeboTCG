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

        List<Card> orderedHand = hand.OrderByDescending(card => card.value).ToList();

        result.discards.Add(orderedHand[0]);

        for(int i = 1; i < orderedHand.Count; i++) {
            Card card = orderedHand[i];
            if(card.rank == orderedHand[0].rank) {
                result.discards.Add(card);
            }
        }

        List<Card> orderedDiscardPile = discardPile.OrderBy(card => card.value).ToList();

        if (result.discards[0].value >= orderedDiscardPile[0].value) {
            // If the rank of the card to discard is greater than or equal to the rank of the top card in the discard pile, discard the cards
            result.discardDraw = orderedDiscardPile[0];
        } else {
            result.deckDraw = true;
        }
        return result;
    }

    public GameTurnResult moderatePlayTurn(List<Card> hand, List<Card> discardPile)
    {
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

        Card discardDraw = null;

        List<Card> currentHighestValueSet = FindHighestValueSet(hand);
        List<Card> highestValueSet = new List<Card>(currentHighestValueSet);

        foreach (Card candidate in discardPile) {
            List<Card> newHand = new List<Card>(hand);
            newHand.Add(candidate);
            List<Card> newHighestValueSet = FindHighestValueSet(newHand);
            
            if (newHighestValueSet.Sum(c => c.value) > highestValueSet.Sum(c => c.value)) {
                highestValueSet = newHighestValueSet;
                discardDraw = candidate;
            }
        }

        
        if (discardDraw == null) {
            // no better set found, discard highest set and draw from deck
            result.discards = currentHighestValueSet;
            result.deckDraw = true;
            return result;
        } else {
            // better set found
            List<Card> handWithoutHighestSet = hand.Where(c => !highestValueSet.Contains(c)).ToList();
            if (handWithoutHighestSet.Count == 0) {
                // no other cards found, discard highest set and draw from deck
                result.discards = currentHighestValueSet;
                result.deckDraw = true;
                return result;
            } else {
                // other cards found, discard highest of those and take card from discard
                result.discards = FindHighestValueSet(handWithoutHighestSet);
                result.discardDraw = discardDraw;
                return result;
            }
        }
    }

    private List<Card> FindHighestValueSet(List<Card> cards)
    {
        if (cards.Count == 0) return new List<Card>();
        
        List<Card> bestSet = new List<Card>();
        int highestValue = 0;

        // Check Tuples
        var rankGroups = cards.GroupBy(c => c.rank);
        foreach (var group in rankGroups)
        {
            if (group.Count() >= 2)  // At least a pair
            {
                List<Card> tuple = group.ToList();
                int tupleValue = tuple.Sum(c => c.value);
                if (tupleValue > highestValue)
                {
                    highestValue = tupleValue;
                    bestSet = tuple;
                }
            }
        }

        // Check Runs
        var suitGroups = cards.Where(c => c.rank != 0)
                                .GroupBy(c => c.suit);
        int jokerCount = cards.Count(c => c.rank == 0);

        foreach (var suitGroup in suitGroups)
        {
            List<Card> samesuit = suitGroup.OrderBy(c => c.rank).ToList();
            
            for (int startIdx = 0; startIdx < samesuit.Count - 1; startIdx++)
            {
                int remainingJokers = jokerCount;
                List<Card> currentRun = new List<Card> { samesuit[startIdx] };
                
                for (int i = startIdx + 1; i < samesuit.Count; i++)
                {
                    int gap = samesuit[i].rank - samesuit[i-1].rank - 1;
                    if (gap > remainingJokers) break;
                    
                    remainingJokers -= gap;
                    currentRun.Add(samesuit[i]);
                }
                
                var jokers = cards.Where(c => c.rank == 0).Take(jokerCount - remainingJokers);
                currentRun.AddRange(jokers);
                
                int currentValue = currentRun.Sum(card => card.value);
                if (currentRun.Count >= 3 && currentValue > highestValue)
                {
                    highestValue = currentValue;
                    bestSet = currentRun;
                }
            }
        }
        
        // If no set found, return highest value single card
        if (bestSet.Count == 0)
        {
            Card highestCard = cards.OrderByDescending(c => c.value).First();
            return new List<Card> { highestCard };
        }
        
        return bestSet;
    }
}

public class GameTurnResult {
    public List<Card> discards;
    public Card discardDraw;
    public bool deckDraw;

    public bool isYaniv;
}