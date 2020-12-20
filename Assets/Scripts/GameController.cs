using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject level;
    [SerializeField]
    private GameObject timeLine;
    [SerializeField]
    private GameObject hand;

    void Start()
    {
        Instance = this;
        CheckJsonFiles();
        LoadGame();
    }

    public static GameController Instance { get; private set; }

    public GameObject Level => level;

    public GameObject TimeLine => timeLine;

    public GameObject Hand => hand;


    private void LoadGame()
    {
        LevelController.LoadCurrentLevel();
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt(Constants.LevelNumberPrefs, LevelController.CurrentLevelNumber);
    }

    private void CheckJsonFiles()
    {
    }
}
