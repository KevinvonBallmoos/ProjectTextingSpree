using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Code.Dialogue.Story;
using Code.Logger;
using Debug = UnityEngine.Debug;

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
        private Image _imageHolder;
        private GameObject _MapHolder;
        
        //[SerializeField] private TextMeshProUGUI storyText;
        [SerializeField] private GameObject menu;
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
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }
        
        private void Next()
        {
            //StopCoroutine(TextSlower(0f));
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

            story.text = "";
            StartCoroutine(TextSlower(0.02f));
            if (!_storyHolder.GetImage().Equals(""))
            {
                GameObject.FindGameObjectWithTag("Map").GetComponent<Image>().enabled = false;
                GameObject.FindGameObjectWithTag("Image").GetComponent<Image>().enabled = true;
                GameObject.FindGameObjectWithTag("Image").GetComponent<Image>().sprite = Resources.Load <Sprite>(_storyHolder.GetImage());;
                
            }
            else
            {
                GameObject.FindGameObjectWithTag("Image").GetComponent<Image>().enabled = false;
                GameObject.FindGameObjectWithTag("Map").GetComponent<Image>().enabled = true;
            }
        }

        private IEnumerator TextSlower(float time)
        {
            string text = _storyHolder.IsRootNode() ? _storyHolder.GetRootNodeText() : _storyHolder.GetParentNodeText();
            string[] strArray = text.Split(' ');
            foreach (var t in strArray)
            {
                foreach (var c in t)
                {
                    story.text += c;
                    yield return new WaitForSeconds(time);
                }

                story.text += " ";
            }
        }
        
        private void NextChapter()
        {
            if (_storyHolder.IsEndOfChapter())
            {
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.onClick.RemoveListener(Next);
                // Add new Listener - Gamemanager
            }
            else if (_storyHolder.IsGameOver())
            {
                //Load GameOver scene
            }
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

