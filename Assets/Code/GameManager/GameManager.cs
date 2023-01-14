using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Code.Dialogue.Story;
using Code.Logger;

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
        [SerializeField] private Story[] allStoryParts;
         private Story[] _allChapters;
        
        [SerializeField] private StoryHolder selectedStory;
        
        // GameManager
        public static GameManager Gm;
        public GameObject endingScreen;
        public bool gameOver;
        public bool endOfChapter;
        private int _activeStory = 0;
        private int _activeChapter;
        
        /// <summary>
        /// Start is called before the first frame update
        /// GameManager is static so only 1 GameManager can exist
        /// </summary>
        private void Start()
        {
            Gm = this;
            _activeChapter = 0;
            selectedStory.selectedChapter = allStoryParts[_activeStory];
            // Load Prolog or First Chapter
        }

        /// <summary>
        /// Checks if its Game Over or end of Chapter
        /// </summary>
        private void Update()
        {
            if (endOfChapter)
                LoadNextChapter(_activeChapter++);
            if (gameOver)
                LoadGameOverScreen();
        }

        
        private void LoadNextChapter(int chapter)
        {
            // Load next Chapter
            _logger.LogEntry("GameManager Log", $"Next chapter: {_allChapters[_activeChapter].name}", GameLogger.GetLineNumber());   
        }

        private void LoadNextStory()
        {
            // Load next Story
            _logger.LogEntry("GameManager Log", $"Next Story Part: {allStoryParts[_activeStory].name}", GameLogger.GetLineNumber());
        }
        
        
        private void LoadGameOverScreen()
        {
            // Load GameOver Screen
            endingScreen.SetActive(true);
            _logger.LogEntry("GameManager Log", $"Game Over!", GameLogger.GetLineNumber());  
        }
    }
}