using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Code.Dialogue.Story;
using Code.Logger;

using UnityEngine.SceneManagement;

/// <summary>
/// Is in Control of the Story
/// </summary>
/// <para name="author">Kevin von Ballmoos</para>
/// <para name="date">11.01.2023</para>
public class GameManager : MonoBehaviour
{
    // Logger
    private readonly GameLogger _logger = new GameLogger("GameManager");
    // Story UI
    private StoryUI _storyUI;
    // StoryHolder
    private StoryHolder _selectedStory;
    // GameManager
    public static GameManager Gm;
    // Ending Screen
    public GameObject endingScreen;

    [NonSerialized] public bool IsGameOver;
    [NonSerialized] public bool IsEndOfChapter;
    [NonSerialized] public bool IsEndOfStory;

    private int _chapter;
    private int _part;
    private string _runPath;
    private string _storyPath;

    private void Start()
    {
        Gm = this;
        _runPath = $"{Application.dataPath}/Resources/";
        _storyUI = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>();
        _selectedStory = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();

        _chapter = 1;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// GameManager is static so only 1 GameManager can exist
    /// </summary>
    public static void LoadNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadSaveGame()
    {
        // Can be called from DataPersistanceManager
        // Get Story, Chapter and Node
    }

    /// <summary>
    /// Checks if its Game Over or end of Chapter
    /// </summary>
    private void Update()
    {
        if (IsEndOfChapter)
            LoadNextChapter();
        if (IsEndOfStory)
            LoadNextStoryPart();
        if (IsGameOver)
            LoadGameOverScreen();
    }

    /// <summary>
    /// Loads next chapter
    /// </summary>
    private void LoadNextChapter()
    {
        IsEndOfChapter = false;
        _part = GetPath();
        _chapter++;
        _storyPath = $@"Story/Part{_part}/Story{_part}Chapter{_chapter}.asset";
        Debug.Log(_storyPath);
        if (!File.Exists($@"{_runPath}{_storyPath}")) return;
        _selectedStory.selectedChapter = Resources.Load<Story>(_storyPath.Replace(".asset", ""));
        _selectedStory.Start();
        _logger.LogEntry("GameManager Log", $"Next chapter: Story{_part}Chapter{_chapter}", GameLogger.GetLineNumber());
    }

    // Load next Story / next scene
    private void LoadNextStoryPart()
    {
        IsEndOfStory = false;
        _part++;
        _logger.LogEntry("GameManager Log", $"Next Story Part: Story{_part}Chapter{_chapter}",
            GameLogger.GetLineNumber());
    }

    // Load GameOver Screen
    private void LoadGameOverScreen()
    {
        IsGameOver = false;
        endingScreen.SetActive(true);
        _logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());
    }

    /// <summary>
    /// When the next Chapter Button is clicked
    /// </summary>
    public void NextChapter_Click()
    {
        //ReStartScripts();
        _storyUI.Start();
    }

    /// <summary>
    /// When the next Chapter Button is clicked
    /// </summary>
    public void NextStory_Click()
    {
        SceneManager.LoadScene(_part);
        // Debug.Log(_storyPath.Replace(".asset", ""));
        // _selectedStory.StartScript();
        // _storyUI.Start();
    }

    private int GetPath()
    {
        var path = _selectedStory.selectedChapter.name;
        foreach (var t in path)
        {
            if (char.IsDigit(t))
                return int.Parse(t.ToString());
        }
        return 0;
    }
}
