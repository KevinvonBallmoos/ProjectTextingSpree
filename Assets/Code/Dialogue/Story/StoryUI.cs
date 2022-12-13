using System;
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
        [SerializeField] private GameObject storyResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryUI");

        private void Start()
        {
            _storyHolder = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();
            // Add the Next Function Method as new Event
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }
        
        private void Next()
        {
            _logger.LogEntry("Click", "Start", _logger.GetLineNumber());
            _storyHolder.Next();
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            nextButton.gameObject.SetActive(!_storyHolder.IsChoosing());
            choiceRoot.gameObject.SetActive(_storyHolder.IsChoosing());

            if (_storyHolder.IsChoosing())
            {
                _logger.LogEntry("LogStart", "Is Choosing", _logger.GetLineNumber());
                BuildChoiceList();
            }
            else
            {
                _logger.LogEntry("LogStart", "Is not Choosing", _logger.GetLineNumber());

                storyResponse.GetComponent<TextMeshProUGUI>().text = _storyHolder.GetText();
                nextButton.enabled = _storyHolder.HasNext();
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (StoryNode choice in _storyHolder.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                // Get the TextMeshProUGUI Component from Children
                TextMeshProUGUI textMeshComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                // Set Choice Text
                textMeshComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                // Lambda Function
                button.onClick.AddListener(() =>
                {
                    _storyHolder.SelectChoice(choice);
                    UpdateUI();
                });
            }
        }
    }
}

