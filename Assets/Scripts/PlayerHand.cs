using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHand
{
    public const int DEFAULT_CARD_AMOUNT = 12;
    private int generatedCards = 0;

    public List<Card> cardsInHand = new List<Card>();

    public const int DISPLAYED_CARD_AMOUNT = 5;

    public PlayerHand()
    {
        cardsInHand = Generate(DISPLAYED_CARD_AMOUNT);
    }

    public void TryGetCard(List<Card> l, int index, out Card c)
    {
        c = null;
        if (l[index] != null)
            c = l[index];
    }

    public List<Card> Generate(int cardNum)
    {
        if (cardNum <= 0)
            throw new System.ArgumentException("Cannot generate <0 cards.");

        var list = new List<Card>();
        for (int i = 0; i < cardNum; i++)
        {
            CardType type = (CardType)Random.Range(0, (int)System.Enum.GetValues(typeof(CardType))
                .Cast<CardType>()
                .Max() + 1);

            var c = PlayScreenState.Instance.CreateNewCard(type);

            list.Add(c);
            generatedCards++;
            Debug.Log($"generated {c.cardType}");
        }
        return list;
    }

    public List<Card> Generate()
        => Generate(DISPLAYED_CARD_AMOUNT);

    public void RemoveCard(Card currentCard)
    {
        currentCard.Destroy();
        cardsInHand.Remove(currentCard);
    }

    public Card DrawCard()
    {
        Card c = null;
        if (generatedCards <= DEFAULT_CARD_AMOUNT)
        {
            c = Generate(1)[0];
            cardsInHand.Add(c);
        }
        return c;
    }
}