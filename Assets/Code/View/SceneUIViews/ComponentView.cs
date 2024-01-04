using System;
using System.Collections.Generic;
using System.Linq;
using Code.Controller.LocalizationController;
using Code.Model.GameData;
using Code.Model.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.View.SceneUIViews
{
    /// <summary>
    /// This class handles the logic of ui elements
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">02.01.2024</para>
    public class ComponentView : MonoBehaviour
    {
        private readonly List<string> WindowSizes = new () { "800x600", "1920x1080" , "3840x2160" };
        
        #region Messagebox
        
        /// <summary>
        /// Sets the properties of the MessageBox
        /// [0] Message box button left
        /// [1] Messagebox text
        /// </summary>
        /// <param name="messageBox">Message box component</param>
        /// <param name="eventMethod">Listener to add to the Button</param>
        /// <param name="buttonText">Button left text</param>
        /// <param name="text">Message Box text</param>
        public void SetMessageBoxProperties(GameObject[] messageBox, UnityAction eventMethod, string buttonText, string text)
        {
            messageBox[0].GetComponent<Button>().onClick.RemoveAllListeners();
            messageBox[0].GetComponent<Button>().onClick.AddListener(eventMethod);
            messageBox[0].GetComponentInChildren<Text>().text = buttonText;
            messageBox[1].GetComponent<Text>().text = text;
        }
        
        /// <summary>
        /// When continue is clicked, the User can choose a save to override the old data with the new Game
        /// </summary>
        public void ContinueAction()
        {
            GameDataInfoModel.IsOverride = true;
            GameManager.Gm.LoadScene();
        }
        
        /// <summary>
        /// Action to disable the Messagebox
        /// </summary>
        /// <param name="messageBox">Message box</param>
        public void CancelAction(GameObject messageBox)
        {
            messageBox.SetActive(false);
        }

        /// <summary>
        /// Enables the messagebox with the according text, to remove a selected save
        /// </summary>
        /// <param name="messageBox">Message box</param>
        /// <param name="holders">Controls, that hold images</param>
        /// <param name="errorLabel">The error label game object</param>
        public void RemoveDataAction(GameObject messageBox, Image[] holders, GameObject errorLabel)
        {
            GameDataInfoModel.SetPlaceholderNum(holders);
            var placeholder = GameDataInfoModel.Placeholder;
            if (placeholder == -1)
            {
                errorLabel.SetActive(true);
                errorLabel.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.GetLocalizedValue(LocalizationKeyController.SaveFileErrorLabelRemoveCaptionKey);
                return;
            }

            errorLabel.SetActive(false);
            messageBox.SetActive(true);
        }
        
        #endregion
        
        #region GameBook Story Image

        public void SwitchToStoryImage(GameObject[] menuGroupObjects)
        {
            menuGroupObjects[0].GetComponent<Image>().enabled = true;
        }
        
        #endregion
        
        #region Settings Properties

        /// <summary>
        /// Creates prefabs for the Audio Setting
        /// </summary>
        /// <param name="settingsPropertiesRoot">root of the all setting prefabs</param>
        /// <param name="settingsPrefabs">prefab variants</param>
        public void DisplayInGameSettings(GameObject settingsPropertiesRoot, GameObject[] settingsPrefabs)
        {
            foreach (Transform item in settingsPropertiesRoot.transform)
                Destroy(item.gameObject);
            
            foreach (var setting in SettingsInfoModel.inGameList.Settings)
            {
                var settingInstance = Instantiate(settingsPrefabs[0], settingsPropertiesRoot.transform);
                // Text
                var textComponents = settingInstance.GetComponentsInChildren<Text>();
                textComponents[0].text = setting.SettingName;
                textComponents[1].text = setting.InfoLabelText;
                textComponents[2].text = setting.SettingValue.ToString();
                // Toggle
                var toggle = settingInstance.GetComponentInChildren<Toggle>();
                toggle.isOn = setting.SettingValue;
                toggle.onValueChanged.AddListener(newValue =>
                {
                    setting.SettingValue = newValue;
                    textComponents[2].text = setting.SettingValue.ToString();
                    // AppendNewValueToGame(setting.SettingName);
                });
            }
        }
        
        /// <summary>
        /// Creates prefabs for the Audio Setting
        /// </summary>
        /// <param name="settingsPropertiesRoot">root of the all setting prefabs</param>
        /// <param name="settingsPrefabs">prefab variants</param>
        public void DisplayVideoSettings(GameObject settingsPropertiesRoot, GameObject[] settingsPrefabs)
        {
            foreach (Transform item in settingsPropertiesRoot.transform)
                Destroy(item.gameObject);
            
            foreach (var setting in SettingsInfoModel.videoList.Settings)
            {
                var settingInstance = Instantiate(settingsPrefabs[1], settingsPropertiesRoot.transform);
                var textComponents = settingInstance.GetComponentsInChildren<Text>();
                textComponents[0].text = setting.SettingName;
                textComponents[1].text = setting.InfoLabelText;
                textComponents[2].text = setting.SettingValue.ToString();
            }
            foreach (var setting in SettingsInfoModel.videoList2.Settings)
            {
                var settingInstance = Instantiate(settingsPrefabs[3], settingsPropertiesRoot.transform);
                // Text
                var textComponents = settingInstance.GetComponentsInChildren<Text>();
                textComponents[0].text = setting.SettingName;
                textComponents[1].text = setting.InfoLabelText;
                //textComponents[2].text = setting.SettingValue;
                // Dropdown
                var dropDown = settingInstance.GetComponentInChildren<Dropdown>();
                dropDown.ClearOptions();
                
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                foreach (var option in WindowSizes)
                {
                    options.Add(new Dropdown.OptionData(text: option));
                }
                //var options = WindowSizes.Select(option => new Dropdown.OptionData(option, Resources.Load<Sprite>("Sprites/Controls/Button_Layout_cropped"))).ToList();
                dropDown.AddOptions(options);
                dropDown.value = WindowSizes.IndexOf(setting.SettingValue);
            }
        }
        
        /// <summary>
        /// Creates prefabs for the Audio Setting
        /// </summary>
        /// <param name="settingsPropertiesRoot">root of the all setting prefabs</param>
        /// <param name="settingsPrefabs">prefab variants</param>
        public void DisplayAudioSettings(GameObject settingsPropertiesRoot, GameObject[] settingsPrefabs)
        {
            foreach (Transform item in settingsPropertiesRoot.transform)
                Destroy(item.gameObject);
            
            foreach (var setting in SettingsInfoModel.audioList.Settings)
            {
                var settingInstance = Instantiate(settingsPrefabs[2], settingsPropertiesRoot.transform);
                // Text
                var textComponents = settingInstance.GetComponentsInChildren<Text>();
                textComponents[0].text = setting.SettingName;
                textComponents[1].text = setting.InfoLabelText;
                textComponents[2].text = setting.SettingValue.ToString();
                // Slider
                var slider = settingInstance.GetComponentInChildren<Slider>();
                slider.value = setting.SettingValue;
                slider.onValueChanged.AddListener(newValue  =>
                {
                    textComponents[2].text = Convert.ToInt32(newValue).ToString();
                    setting.SettingValue = Convert.ToInt32(newValue);
                    // Adjust the loudness // TODO Create VolumeControl class
                });
            }
        }
        
        #endregion
    }
}