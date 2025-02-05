using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public enum LocationName
{
    Deck,
    Discard,
    PlayerHand,
    OpponentHand
}


public class AnimationController : MonoBehaviour
{
    [SerializeField]
    private List<Card> allCards = new List<Card>();

    [SerializeField]
    private Vector2 deckPosition = new Vector2(-2.5f, 0);
    [SerializeField]
    private Vector2 discardPosition = new Vector2(2.5f, 0);
    [SerializeField]
    private Vector2 playerHandPosition = new Vector2(0, -3f);
    [SerializeField]
    private Vector2 opponentHandPosition = new Vector2(0, 3f);
    [SerializeField]
    private float handSpacing = 1.5f; // Space between cards in hand

    [SerializeField]
    private float discardSpacing = 0.5f; // Space between cards showing on discard pile

    private HashSet<Card> animatingCards = new HashSet<Card>();

    public class Location {
        public Vector2 position;
        public float cardSpacing;

        public void Deconstruct(out Vector2 position, out float cardSpacing)
        {
            position = this.position;
            cardSpacing = this.cardSpacing;
        }
    }

    private Dictionary<LocationName, Location> locations = new Dictionary<LocationName, Location>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        locations.Add(LocationName.Deck, new Location { position = deckPosition, cardSpacing = 0 });
        locations.Add(LocationName.Discard, new Location { position = discardPosition, cardSpacing = discardSpacing });
        locations.Add(LocationName.PlayerHand, new Location { position = playerHandPosition, cardSpacing = handSpacing });
        locations.Add(LocationName.OpponentHand, new Location { position = opponentHandPosition, cardSpacing = handSpacing });
    }

    public void ResetDeck(List<Card> deck)
    {
        foreach (Card card in deck)
        {
            card.MoveToPosition(deckPosition, 0.5f);
            card.FaceDown(0.5f);
        }
    }

    public void DealCards(List<DealCardParams> dealParams)
    {
        Sequence dealSequence = DOTween.Sequence();
    
        foreach (DealCardParams dealParam in dealParams)
        {
           
            dealSequence.AppendCallback(() => {
                SendCardToLocation(dealParam);
            });
            
            dealSequence.AppendInterval(0.2f);  // Wait between deals
        }
    }

    public void FlipCardsInSequence(List<Card> cards)
    {
        Sequence flipSequence = DOTween.Sequence();
        foreach (Card card in cards)
        {
            flipSequence.AppendCallback(() => {
                card.FlipCard(0.3f);
            });
            flipSequence.AppendInterval(0.2f);
        }
    }

    private void RearrangeHand(List<Card> hand, Vector2 basePosition, float cardSpacing)
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

    private void SendCardToLocation(DealCardParams dealParams) {
        var (card, hand, location, flip) = dealParams;
        
        var (position, cardSpacing) = locations[location];

          // Animate the card
        card.MoveToPosition(position, 0.3f);
        if (flip)
        {
            card.FlipCard(0.3f);
        }
        card.SetRenderOrder(50 + hand.Count);

        RearrangeHand(hand, position, cardSpacing);
    }

    public void PrepStackForAction(List<Card> stack, LocationName location)
    {
        int i = 0;
        foreach(Card card in stack)
        {
            card.MoveToPosition(locations[location].position, 0.3f);
            // card.SetClickable(false);
            card.SetRenderOrder(i);
            i++;
        }
    }

    public void SendCardsToDiscard(List<Card> cards, bool flip)
    {
        foreach(Card card in cards)
        {
            card.MoveToPosition(discardPosition, 0.3f);
            if (flip)
            {
                card.FlipCard(0.3f);
            }
        }
        RearrangeHand(cards, discardPosition, discardSpacing);

    }

    public void SetCardAnimating(Card card, bool isAnimating)
    {
        if (isAnimating)
        {
            animatingCards.Add(card);
        }
        else
        {
            animatingCards.Remove(card);
        }
    }

    public bool IsCardAnimating(Card card)
    {
        return animatingCards.Contains(card);
    }

    public void SelectCard(Card card, bool deselect = false)
    {
        animatingCards.Add(card);
        card.MoveUp(0.5f * (deselect ? 1 : -1), () => animatingCards.Remove(card));
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

 public class DealCardParams{
        public Card card;
        public List<Card> hand;
        public LocationName location;
        public bool flip;

        public void Deconstruct(out Card card, out List<Card> hand, out LocationName location, out bool flip)
        {
            card = this.card;
            hand = this.hand;
            location = this.location;
            flip = this.flip;
        }
    }
