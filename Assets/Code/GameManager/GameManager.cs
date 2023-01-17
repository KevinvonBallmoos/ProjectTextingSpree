using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Code.Dialogue.Story;
using Code.Logger;

using UnityEngine.SceneManagement;

namespace Code.GameManager
{
    /// <summary>
    /// Is in Control of the Story
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">11.01.2023</para>
    public class GameManager : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("GameManager");
        // Story, Chapter
        private List<string> _allStoryParts;

        private StoryUI _storyUI;
        private StoryHolder _selectedStory;
        
        // GameManager
        public static GameManager Gm;
        public GameObject endingScreen;
        [NonSerialized] public bool gameOver;
        [NonSerialized] public bool endOfChapter;
        private int _part;
        private int _chapter;
        private string _runPath;
        private string _storyPath;

        /// <summary>
        /// Start is called before the first frame update
        /// GameManager is static so only 1 GameManager can exist
        /// </summary>
        public void StartScript()
        {
            Gm = this;
            _runPath = $"{Application.dataPath}/Resources/";
            _storyUI = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>();
            _selectedStory = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();

            
            if (LoadGame())
            {
                foreach (var folder in Directory.GetDirectories(@$"{Application.dataPath}/Story/"))
                    _allStoryParts.Add(folder);
                // Get active story and active chapter
            }
            else
            {
                _selectedStory.selectedChapter = Resources.Load <Story> ("Story/Part1/Story1Chapter1");
                _selectedStory.StartScript();
                SetPartAndChapter();
            }
            
            _storyUI.StartScript();
        }

        /// <summary>
        /// Checks if its Game Over or end of Chapter
        /// </summary>
        private void Update()
        {
            if (endOfChapter)
                LoadNextChapter();
            if (gameOver)
                LoadGameOverScreen();
        }

        private void SetPartAndChapter()
        {
            _part = 1;
            _chapter = 1;
        }

        private static bool LoadGame()
        {
            // Get Story, Chapter and Node
            return false;
        }

        /// <summary>
        /// Loads next chapter
        /// </summary>
        private void LoadNextChapter()
        {
            endOfChapter = false;
            _chapter++;
            _storyPath = $@"Story/Part{_part}/Story{_part}Chapter{_chapter}.asset";

            if (File.Exists($@"{_runPath}{_storyPath}"))
            {
                _logger.LogEntry("GameManager Log", $"Next chapter: {_storyPath.Replace(".asset", "")}", GameLogger.GetLineNumber());
            }
            else
            {
                _part++;
                LoadNextStoryPart();
            }
        }

        private void LoadNextStoryPart()
        {
            // Load next Story
            var story = _allStoryParts[_part];
            SceneManager.LoadScene(1); // Next scene
            SetPartAndChapter();
            _logger.LogEntry("GameManager Log", $"Next Story Part: {story.Substring(story.Length -15, story.Length -1)}", GameLogger.GetLineNumber());
        }
        
        
        private void LoadGameOverScreen()
        {
            gameOver = false;
            // Load GameOver Screen
            //endingScreen.GetComponent<TextMeshPro>().enabled = true;
            endingScreen.SetActive(true);
            //Update();
            _logger.LogEntry("GameManager Log", $"Game Over! ", GameLogger.GetLineNumber());  
        }

        /// <summary>
        /// When the next Chapter Button is clicked
        /// </summary>
        public void NextChapter_Click()
        {
            ReStartScripts();
            _selectedStory.selectedChapter = Resources.Load <Story> (_storyPath.Replace(".asset", ""));
            _selectedStory.StartScript();
            _storyUI.StartScript();
        }


        private static void ReStartScripts()
        {
            GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>().enabled = false;
            GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().enabled = false;
            
            GameObject.FindGameObjectWithTag("Story").GetComponent<StoryUI>().enabled = true;
            GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>().enabled = true;
        }
    }
}