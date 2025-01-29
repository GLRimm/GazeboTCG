using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class GameController : MonoBehaviour
{
    [SerializeField]
    private List<Card> allCards = new List<Card>();

    [SerializeField]
    private Vector2 deckPosition = new Vector2(-2.5f, 0);
    [SerializeField]
    private Vector2 discardPosition = new Vector2(2.5f, 0);
    [SerializeField]
    private Vector2 playerHandPosition = new Vector2(-3f, -3f);
    [SerializeField]
    private Vector2 opponentHandPosition = new Vector2(-3f, 3f);
    [SerializeField]
    private float cardSpacing = 2f; // Space between cards in hand


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
        deck.AddRange(allCards);
        foreach(Card card in deck)
        {
            card.MoveToPosition(deckPosition, 0.5f);
            card.FlipCard(0.5f);
        }
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
        Sequence dealSequence = DOTween.Sequence();
    
        for (int i = 0; i < 5; i++)
        {
            // Deal to player
            if (deck.Count > 0)
            {
                Card playerCard = deck[0];
               
                deck.RemoveAt(0);
                
                dealSequence.AppendCallback(() => {
                    SendCardToPlayerHand(playerCard);
                    
                });
                
                dealSequence.AppendInterval(0.2f);  // Wait between deals
            }

            // Deal to opponent
            if (deck.Count > 0)
            {
                Card opponentCard = deck[0];
                deck.RemoveAt(0);
                dealSequence.AppendCallback(() => {
                    SendCardToOpponentHand(opponentCard);
                    
                });
                
                dealSequence.AppendInterval(0.2f);
            }
        }
    }

    public void RearrangeHand(List<Card> hand, Vector2 basePosition)
{
    int cardCount = hand.Count;
    if (cardCount == 0) return;

    float totalWidth = cardSpacing * (cardCount - 1);
    float startX = basePosition.x - (totalWidth / 2);

    for (int i = 0; i < cardCount; i++)
    {
        Vector2 newPos = new Vector2(startX + (cardSpacing * i), basePosition.y);
        hand[i].MoveToPosition(newPos, 0.3f);
        hand[i].SetRenderOrder(50 + i);
    }
}

    private void SendCardToHand(Card card, List<Card> hand, Vector2 handPosition, bool flip) {
        hand.Add(card);
        
         // Calculate the new hand position
        float totalWidth = cardSpacing * (hand.Count - 1);
        float startX = handPosition.x - (totalWidth / 2);
        Vector2 newCardPos = new Vector2(startX + (cardSpacing * (hand.Count - 1)), handPosition.y);

          // Animate the card
        card.MoveToPosition(newCardPos, 0.3f);
        if (flip)
        {
            card.FlipCard(0.3f);
        }
        card.SetRenderOrder(50 + playerHand.Count);

        // Rearrange the hand
        RearrangeHand(hand, handPosition);
    }

    private void SendCardToPlayerHand(Card card) {
        SendCardToHand(card, playerHand, playerHandPosition, true);
        Debug.Log("Dealt: " + card.rank + " of " + card.suit + " to player");
    }

    private void SendCardToOpponentHand(Card card) {
        SendCardToHand(card, opponentHand, opponentHandPosition, false);
        Debug.Log("Dealt: " + card.rank + " of " + card.suit + " to opponent");
    }

    public void SelectCard(Card card) {
        if (playerSelected.Contains(card))
        {
            playerSelected.Remove(card);
            card.MoveUp(-0.5f);
        }
        else if (playerHand.Contains(card))
        {
            playerSelected.Add(card);
            card.MoveUp(0.5f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
