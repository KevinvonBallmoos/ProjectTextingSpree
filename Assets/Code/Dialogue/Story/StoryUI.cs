using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.Dialogue.Story;
using Code.Logger;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Displays the Story in the GUI
    /// </summary>
    /// <para name="author">Kevin von Ballmoos></para>
    /// <para name="date">12.12.2022</para>
    public class StoryUI : MonoBehaviour
    {
        private StoryHolder _storyHolder;
        //[SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private TextMeshProUGUI story;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryUI");

        private void Start()
        {
            _storyHolder = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();
            _logger.LogEntry("Click", _storyHolder.GetRootNodeText(), _logger.GetLineNumber());
            //story.text = _storyHolder.GetRootNodeText();
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }
        
        private void Next()
        {
            _storyHolder.Next();
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            _logger.LogEntry("LogStart", "Null or not", _logger.GetLineNumber());

            if (!_storyHolder.IsNull())
            {
                if (_storyHolder.IsStoryNode())
                {
                    if (_storyHolder.HasNext())
                    {
                        nextButton.gameObject.SetActive(true);
                        choiceRoot.gameObject.SetActive(false);
                    }
                    else
                    {
                        NextChapter();
                    }
                }
                else if (!_storyHolder.IsStoryNode())
                {
                    _logger.LogEntry("LogStart", "Is Choice", _logger.GetLineNumber());
                    nextButton.gameObject.SetActive(false);
                    choiceRoot.gameObject.SetActive(true);
                    BuildChoiceList();
                }
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                choiceRoot.gameObject.SetActive(false);
                _logger.LogEntry("Click", "Next 40", _logger.GetLineNumber());

                NextChapter();
                // When no more Nodes are available
                // Continue with Game
            }
            story.text = _storyHolder.IsRootNode() ? _storyHolder.GetRootNodeText() : _storyHolder.GetParentNodeText();
        }

        private void NextChapter()
        {
            // If No more nodes then Button Text = "Next Chapter", and switch Listener
            nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
            nextButton.onClick.RemoveListener(Next);
            // Add new Listener
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (var choice in _storyHolder.GetChoices())
            {
                var choiceInstance = Instantiate(choicePrefab, choiceRoot);
                
                // Set Text
                var choiceText = choiceInstance.GetComponentInChildren<Text>();
                choiceText.text = choice.GetText();
                
                // Add listener
                var button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _storyHolder.Next(choice);
                    UpdateUI();
                });
            }
        }
    }
}

