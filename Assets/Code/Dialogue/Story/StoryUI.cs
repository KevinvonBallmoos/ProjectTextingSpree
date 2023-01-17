using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Code.Logger;

namespace Code.Dialogue.Story
{
    /// <summary>
    /// Displays the Story in the GUI
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">12.12.2022</para>
    public class StoryUI : MonoBehaviour
    {
        // Logger
        private readonly GameLogger _logger = new GameLogger("StoryUI");
        // Story Holder
        private StoryHolder _storyHolder;
        // SerializedFields
        [SerializeField] private TextMeshProUGUI story;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject[] imageHolder;
        
        private Coroutine _coroutine;

        /// <summary>
        /// When the Game starts, gets the story, adds the Next button click Event and Updates the UI
        /// </summary>
        public void StartScript()
        {
            _storyHolder = GameObject.FindGameObjectWithTag("Story").GetComponent<StoryHolder>();
            if (_storyHolder.selectedChapter == null) return;
            
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }

        /// <summary>
        /// When the next button is clicked, it loads the next part of the story
        /// </summary>
        private void Next()
        {
            StopCoroutine(_coroutine);
            _storyHolder.Next();
            UpdateUI();
        }
        
        /// <summary>
        /// Updates the Story, loads the next part of story and the choices nodes
        /// </summary>
        private void UpdateUI()
        {
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
                    nextButton.gameObject.SetActive(false);
                    choiceRoot.gameObject.SetActive(true);
                    BuildChoiceList();
                }
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                choiceRoot.gameObject.SetActive(false);
                NextChapter();
                // When no more Nodes are available
                // Continue with Game
            }

            // Displays Text
            story.text = "";
            _coroutine = StartCoroutine(TextSlower(0.02f));
            // Displays Image
            if (!_storyHolder.GetImage().Equals(""))
            {
                imageHolder[0].SetActive(false);
                imageHolder[1].SetActive(true);
                imageHolder[1].GetComponent<Image>().sprite = Resources.Load <Sprite>("StoryImage/" + _storyHolder.GetImage());
            }
            else
            {
                imageHolder[1].SetActive(false);
                imageHolder[0].SetActive(true);
            }
        }

        /// <summary>
        /// Displays the text char by char, gives a visual effect
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator TextSlower(float time)
        {
            var text = _storyHolder.IsRootNode() ? _storyHolder.GetRootNodeText() : _storyHolder.GetParentNodeText();
            var strArray = text.Split(' ');
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
        
        /// <summary>
        /// Loads the next Chapter when the End of Chapter node is reached
        /// Or the GameOver Screen when the GameOver node is reached
        /// </summary>
        private void NextChapter()
        {
            if (_storyHolder.IsEndOfChapter())
            {
                _logger.LogEntry("UI log", "End of Chapter reached.", GameLogger.GetLineNumber());
                // If No more nodes then Button Text = "Next Chapter", and switch Listener
                nextButton.GetComponentInChildren<Text>().text = "Next Chapter";
                nextButton.onClick.RemoveListener(Next);

                GameManager.GameManager.Gm.endOfChapter = true;
                nextButton.onClick.AddListener(GameManager.GameManager.Gm.NextChapter_Click);
            }
            else if (_storyHolder.IsGameOver())
            {
                _logger.LogEntry("UI log", "Game Over reached.", GameLogger.GetLineNumber());
                nextButton.gameObject.SetActive(false);

                GameManager.GameManager.Gm.gameOver = true;
            }
        }

        /// <summary>
        /// Builds the choice list, depending on the count of the nodes
        /// </summary>
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

