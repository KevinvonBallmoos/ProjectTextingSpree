using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant _playerConversant;
        [SerializeField] private TextMeshProUGUI AIText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject AIResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        
        // Start is called before the first frame update
        private void Start()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            // Add the Next Function Method as new Event
            nextButton.onClick.AddListener(Next);
            UpdateUI();
        }

        /// <summary>
        /// When the Next Button is clicked
        /// </summary>
        private void Next()
        {
            _playerConversant.Next();
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI
        /// Rewrites the Text
        /// </summary>
        private void UpdateUI()
        {
            // Get the State, Either Choice or Next
            AIResponse.SetActive(!_playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing());

            if (_playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                AIText.text = _playerConversant.GetText();
                nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildChoiceList()
        {
            // Destroy Objects
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (DialogueNode choice in _playerConversant.GetChoices())
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
                    _playerConversant.SelectChoice(choice);
                    UpdateUI();
                });
            }
        }
        
        
    }
}

