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
    private float cardSpacing = 1.5f; // Space between cards in hand


    private List<Card> deck = new List<Card>();
    private List<Card> playerHand = new List<Card>();
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
                playerHand.Add(playerCard);
                deck.RemoveAt(0);
                Vector2 playerCardPos = playerHandPosition + new Vector2(cardSpacing * i, 0);
                
                dealSequence.AppendCallback(() => {
                    playerCard.MoveToPosition(playerCardPos, 0.3f);
                    playerCard.FlipCard(0.3f);  // Flip the card face up
                    playerCard.SetRenderOrder(50 + i);  // Ensure dealt cards are above deck
                    
                });
                
                dealSequence.AppendInterval(0.2f);  // Wait between deals
            }

            // Deal to opponent
            if (deck.Count > 0)
            {
                Card opponentCard = deck[0];
                opponentHand.Add(opponentCard);
                deck.RemoveAt(0);
                Vector2 opponentCardPos = opponentHandPosition + new Vector2(cardSpacing * i, 0);
                dealSequence.AppendCallback(() => {
                    opponentCard.MoveToPosition(opponentCardPos, 0.3f);
                    opponentCard.SetRenderOrder(50 + i);
                    
                });
                
                dealSequence.AppendInterval(0.2f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
