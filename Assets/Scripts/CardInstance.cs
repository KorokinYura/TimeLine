using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardInstance : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 prevPosition;

    public Card Card { get; set; }

    public void OnDrag(PointerEventData eventData)
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().raycastTarget = false;
        prevPosition = transform.position;

        LevelController.SelectedCard = gameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        gameObject.GetComponent<Image>().raycastTarget = true;
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        var timeLine = results.FirstOrDefault(r => r.gameObject.GetComponent<TimeLineInstance>() != null).gameObject;

        if (timeLine != null)
        {
            timeLine.gameObject.GetComponent<TimeLineInstance>().PlaceCardOnTimeLine(gameObject);
        }
        else
        {
            transform.position = prevPosition;
            GameController.Instance.Hand.GetComponent<HandInstance>().PlaceCardInHand(gameObject);
        }

        LevelController.SelectedCard = null;
    }
}
