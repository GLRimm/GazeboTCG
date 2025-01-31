using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


public class GameController : MonoBehaviour
{
    [SerializeField]
    private List<Card> allCards = new List<Card>();

    [SerializeField]
    private AnimationController animationController;


    private List<Card> deck = new List<Card>();
    private List<Card> playerHand = new List<Card>();

    private List<Card> playerSelected = new List<Card>();
    private List<Card> opponentHand = new List<Card>();
    private List<Card> discard = new List<Card>();

    private int turnCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeDeck();
        Invoke("DealCards", 0.6f);
    }

    void InitializeDeck()
    {
        allCards.ForEach(card => card.SetClickable(false));
        deck.AddRange(allCards);
        animationController.ResetDeck(deck);
    
        // Shuffle the deck
        System.Random r = new System.Random();
        for (int n = deck.Count - 1; n > 0; --n)
        {
            int k = r.Next(n + 1);
            Card temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
            deck[k].SetRenderOrder(k);
        }
    }

    void DealCards()
    {
        List <DealCardParams> cardsToDeal = new List<DealCardParams>();
    
        for (int i = 0; i < 5; i++)
        {
            // Deal to player
            if (deck.Count > 0)
            {
                Card playerCard = deck[0];
                deck.RemoveAt(0);
                playerHand.Add(playerCard);
                playerCard.SetClickable(true);
                cardsToDeal.Add(new DealCardParams{
                    card = playerCard,
                    hand = new List<Card>(playerHand),
                    location = LocationName.PlayerHand,
                    flip = true
                });
            }

            // Deal to opponent
            if (deck.Count > 0)
            {
                Card opponentCard = deck[0];
                deck.RemoveAt(0);
                opponentHand.Add(opponentCard);
                cardsToDeal.Add(new DealCardParams{
                    card = opponentCard,
                    hand = new List<Card>(opponentHand),
                    location = LocationName.OpponentHand,
                    flip = false
                });
            }
        }

        Card discardCard = deck[0];
        deck.RemoveAt(0);
        discard.Add(discardCard);
        discardCard.SetClickable(true);
        cardsToDeal.Add(new DealCardParams{
            card = discardCard,
            hand = new List<Card>(discard),
            location = LocationName.Discard,
            flip = true
        });
        deck[0].SetClickable(true);
        animationController.DealCards(cardsToDeal);
    }

    public void SelectCard(Card card) {
        if (animationController.IsCardAnimating(card)) return;
        if (playerSelected.Contains(card))
        {
            playerSelected.Remove(card);
            animationController.SelectCard(card, false);
        }
        else if (playerHand.Contains(card))
        {
            playerSelected.Add(card);
            animationController.SelectCard(card, true);
        }
        else if (deck.Contains(card) || discard.Contains(card))
        {
            if (playerSelected.Count == 0 || !IsSelectionValid()) return;
            animationController.PrepStackForAction(discard, LocationName.Discard);
            Swap(card, deck.Contains(card));
        }
    }

    private void Swap(Card card, bool fromDeck = false) {
        List<Card> cardsToDiscard = new List<Card>(playerSelected);
        List<Card> deckOrDiscard = fromDeck ? deck : discard;
        deckOrDiscard.Remove(card);
        playerHand.Add(card);

        foreach (Card selectedCard in cardsToDiscard)
        {
            playerSelected.Remove(selectedCard);
            playerHand.Remove(selectedCard);
            discard.Add(selectedCard);
            card.SetClickable(true);
        }

         DealCardParams dealCard = new DealCardParams{
            card = card,
            hand = new List<Card>(playerHand),
            location = LocationName.PlayerHand,
            flip = fromDeck
        };
  
        animationController.DealCards(new List<DealCardParams>{dealCard});
        animationController.SendCardsToDiscard(cardsToDiscard, false);

        deck[0].SetClickable(true);
    }

    private bool IsSelectionValid() {
        if (playerSelected.Count == 0) return false;
        
        if (playerSelected.Count == 1) return true;
        
        // Check for tuples of same rank
        int firstRank = playerSelected[0].rank;
        if(playerSelected.All(card => card.rank == firstRank)) return true;
        
        // Runs must be at least 3 cards
        if(playerSelected.Count > 2) {
            List<Card> nonJokers = playerSelected
                .Where(card => card.rank != 0)
                .OrderBy(c => c.rank)
                .ToList();
            
            // Runs must be flush
            CardSuit firstSuit = nonJokers[0].suit;
            if (nonJokers.Any(card => card.suit != firstSuit)) return false;

            int remainingJokers = playerSelected.Count - nonJokers.Count;

            // Check for rank gaps
            for(int i = 1; i < nonJokers.Count; i++) {
                int gap = nonJokers[i].rank - nonJokers[i - 1].rank -1;
                if (gap > remainingJokers) return false;
                remainingJokers -= gap;
            }
            
            return true;
        }

        return false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
