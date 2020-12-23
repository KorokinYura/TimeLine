using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject level;
    [SerializeField]
    private GameObject timeLine;
    [SerializeField]
    private GameObject hand;
    [SerializeField]
    private GameObject levelNumberText;
    [SerializeField]
    private GameObject errorText;

    void Start()
    {
        Instance = this;
        CheckJsonFiles();
        LoadGame();
        ErrorText.GetComponent<Text>().text = "You should place cards in chronological order.";
    }

    public static GameController Instance { get; private set; }

    public GameObject Level => level;

    public GameObject TimeLine => timeLine;

    public GameObject Hand => hand;

    public GameObject LevelNumberText => levelNumberText;

    public GameObject ErrorText => errorText;


    public void LoadGame()
    {
        LevelController.LoadCurrentLevel();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(Constants.LevelNumberPrefs, LevelController.CurrentLevelNumber);
    }

    public void CheckButtonClicked()
    {
        LevelController.CheckLevelFinished();
    }

    private void CheckJsonFiles()
    {
        // TBD
    }
}
