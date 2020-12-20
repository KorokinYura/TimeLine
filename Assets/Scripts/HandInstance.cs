using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInstance : MonoBehaviour
{
    public void PlaceCardInHand(GameObject card)
    {
        card.transform.SetParent(transform);
        UpdateCardsLocation();
    }

    public void UpdateCardsLocation()
    {
        var cards = LevelController.CardsInHand;

        if (cards.Count == 0)
        {
            return;
        }

        var cardWidthDelta = Screen.width / cards.Count;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.localPosition = new Vector2((cardWidthDelta * i) - (Screen.width / 2) + (cardWidthDelta / 2), -(Screen.height / 2.5f));
        }
    }
}
