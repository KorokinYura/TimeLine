using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimeLineInstance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private bool pointerIsInside;

    private List<Transform> ActualCardsPositions { get; set; }

    private void Update()
    {
        if (pointerIsInside && LevelController.SelectedCard != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.FirstOrDefault(r => r.gameObject.name == "TimeLine").gameObject != null)
            {
                OnPointerOver();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsInside = true;
        var cards = LevelController.CardsInTimeLine;
        ActualCardsPositions = cards.Select(c => c.transform).ToList();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsInside = false;
        if (ActualCardsPositions != null && LevelController.SelectedCard != null)
        {
            ActualCardsPositions.Remove(LevelController.SelectedCard.transform);
            SetCardsPositions(ActualCardsPositions);
        }

        ActualCardsPositions = null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ActualCardsPositions != null && LevelController.SelectedCard != null)
        {
            ActualCardsPositions.Add(LevelController.SelectedCard.transform);
        }
    }

    public void PlaceCardOnTimeLine(GameObject card)
    {
        card.transform.SetParent(transform);
        GameController.Instance.Hand.GetComponent<HandInstance>().UpdateCardsLocation();

        UpdateCardsPositionsInHierarchy();
        SetCardsPositions(LevelController.CardsInTimeLine.Select(c => c.transform).ToList());
    }

    private void OnPointerOver()
    {
        var cards = LevelController.CardsInTimeLine;

        if (cards.Count == 0)
        {
            return;
        }

        UpdateCardsTempPostitions(cards);
    }

    private void UpdateCardsTempPostitions(List<GameObject> cards)
    {
        SetCardIndex(cards, LevelController.SelectedCard);
        SetCardsTempPositions(cards.Select(c => c.transform).ToList());
    }

    private void SetCardIndex(List<GameObject> cards, GameObject card)
    {
        if (!cards.Contains(card))
        {
            cards.Add(card);
        }

        cards.Remove(card);
        cards.Insert(0, card);

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].transform.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x)
            {
                cards.Remove(card);
                cards.Insert(i, card);
            }
        }
    }

    private void SetCardsTempPositions(List<Transform> transforms)
    {
        var cardWidthDelta = Screen.width / transforms.Count;
        for (int i = 0; i < transforms.Count; i++)
        {
            if (transforms[i] == LevelController.SelectedCard.transform)
            {
                continue;
            }
            transforms[i].localPosition = new Vector2((cardWidthDelta * i) - (Screen.width / 2) + (cardWidthDelta / 2), 0);
        }
    }

    private void SetCardsPositions(List<Transform> transforms)
    {
        if (transforms.Count == 0)
        {
            return;
        }

        var cardWidthDelta = Screen.width / transforms.Count;
        for (int i = 0; i < transforms.Count; i++)
        {
            transforms[i].localPosition = new Vector2((cardWidthDelta * i) - (Screen.width / 2) + (cardWidthDelta / 2), 0);
        }
    }

    private void UpdateCardsPositionsInHierarchy()
    {
        var cards = LevelController.CardsInTimeLine;

        cards = cards.OrderBy(c => c.transform.position.x).ToList();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
        }
    }
}
