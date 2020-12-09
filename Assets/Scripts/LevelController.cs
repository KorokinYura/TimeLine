using Newtonsoft.Json;
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
        CardsInHand = new List<GameObject>();
    }

    public static int CurrentLevelNumber { get; private set; }

    public static List<Card> AvailableCards { get; private set; }

    public static List<Level> Levels { get; private set; }

    private static List<GameObject> CardsInHand { get; set; }

    public static void LoadCurrentLevel()
    {
        var currentLevel = Levels.First(l => l.LevelNumber == CurrentLevelNumber);

        LoadHand(currentLevel.CardsAtBeginning);

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

    private static void LoadHand(int cards)
    {
        var cardPrefab = Resources.Load(Constants.CardPrefab) as GameObject;

        for (int i = 1; i <= cards; i++)
        {
            var card = InstansiateCard(cardPrefab);
            CardsInHand.Add(card);
            card.transform.localPosition = new Vector2((Screen.width / cards * i) - (Screen.width / (5f/3f)), -(Screen.height / 2.5f));
        }
    }

    private static GameObject InstansiateCard(GameObject cardPrefab)
    {
        var card = Object.Instantiate(cardPrefab);

        var cardInstance = card.GetComponent<CardInstance>();

        var cardIndex = Random.Range(0, AvailableCards.Count);
        cardInstance.Card = AvailableCards[cardIndex];
        AvailableCards.RemoveAt(cardIndex);

        cardInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>(Path.Combine(Constants.CardSprites, cardInstance.Card.SpriteName));

        card.transform.SetParent(GameController.Instance.Hand.transform);
        card.transform.localScale = cardPrefab.transform.localScale;

        return card;
    }
}
