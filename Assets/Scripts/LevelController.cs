using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class LevelController
{
    static LevelController()
    {
        CurrentLevelNumber = PlayerPrefs.GetInt(Constants.LevelNumberPrefs);
        CurrentLevelNumber = CurrentLevelNumber == 0 ? 1 : CurrentLevelNumber;

        AvailableCards = JsonConvert.DeserializeObject<List<Card>>((Resources.Load(Constants.CardsJsonFilePath) as TextAsset).text).ToList();
        Levels = JsonConvert.DeserializeObject<List<Level>>((Resources.Load(Constants.LevelsJsonFilePath) as TextAsset).text).ToList();
    }

    public static int CurrentLevelNumber { get; private set; }

    public static List<Card> AvailableCards { get; private set; }

    public static List<Level> Levels { get; private set; }

    public static List<GameObject> CardsInHand
    {
        get
        {
            var cards = new List<GameObject>();

            foreach (Transform child in GameController.Instance.Hand.transform)
            {
                if (child.GetComponent<CardInstance>() != null)
                {
                    cards.Add(child.gameObject);
                }
            }

            return cards;
        }
    }

    public static List<GameObject> CardsInTimeLine
    {
        get
        {
            var cards = new List<GameObject>();

            foreach (Transform child in GameController.Instance.TimeLine.transform)
            {
                if (child.GetComponent<CardInstance>() != null)
                {
                    cards.Add(child.gameObject);
                }
            }

            return cards;
        }
    }

    public static GameObject SelectedCard { get; set; }

    public static void LoadCurrentLevel()
    {
        var currentLevel = Levels.First(l => l.LevelNumber == CurrentLevelNumber);
        GameController.Instance.LevelNumberText.GetComponent<Text>().text = CurrentLevelNumber.ToString();

        LoadHand(currentLevel.CardsAtBeginning);

        // Additional rules for different level difficulties
        switch (currentLevel.LevelDifficulty)
        {
            case LevelDifficulty.Easy:
                break;
            case LevelDifficulty.Medium:
                break;
            case LevelDifficulty.Hard:
                break;
            case LevelDifficulty.Insane:
                break;
        }
    }

    private static void ClearLevel()
    {
        var cards = CardsInTimeLine;
        cards.AddRange(CardsInHand);

        cards.ForEach(c => UnityEngine.Object.Destroy(c));
    }

    private static void LoadHand(int cards)
    {
        var cardPrefab = Resources.Load(Constants.CardPrefab) as GameObject;
        var handInstance = GameController.Instance.Hand.GetComponent<HandInstance>();

        for (int i = 1; i <= cards; i++)
        {
            InstansiateCard(cardPrefab);
        }

        handInstance.UpdateCardsLocation();
    }

    private static GameObject InstansiateCard(GameObject cardPrefab)
    {
        var card = UnityEngine.Object.Instantiate(cardPrefab);

        var cardInstance = card.GetComponent<CardInstance>();

        var cardIndex = UnityEngine.Random.Range(0, AvailableCards.Count);
        cardInstance.Card = AvailableCards[cardIndex];
        AvailableCards.RemoveAt(cardIndex);

        cardInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>(Path.Combine(Constants.CardSprites, cardInstance.Card.SpriteName));

        card.transform.SetParent(GameController.Instance.Hand.transform);
        card.transform.localScale = cardPrefab.transform.localScale;

        return card;
    }

    public static void CheckLevelFinished()
    {
        if (CardsInHand.Count != 0)
        {
            GameController.Instance.ErrorText.GetComponent<Text>().text = "Hand must be empty before end of the level.";
            return;
        }

        try
        {
            var cards = CardsInTimeLine;
            var curDate = cards[0].GetComponent<CardInstance>().Card.Date;

            foreach (var card in cards)
            {
                var cardInstance = card.GetComponent<CardInstance>();
                if (curDate <= cardInstance.Card.Date)
                {
                    curDate = cardInstance.Card.Date;
                }
                else
                {
                    GameController.Instance.ErrorText.GetComponent<Text>().text = "Incorrect cards order.";
                    return;
                }
            }

            CurrentLevelNumber++;

            GameController.Instance.SaveGame();
            ClearLevel();
            LoadCurrentLevel();
        }
        catch (Exception ex)
        {
            GameController.Instance.GetComponent<Text>().text = $"Unhandled error occured - Exception type: {ex.GetType()}";
            Debug.Log(ex);
        }
    }
}
