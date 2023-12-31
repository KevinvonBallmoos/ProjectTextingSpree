using System.Collections.Generic;
using System.Linq;
using Code.Controller.DialogueController.StoryDialogueController;
using UnityEngine;

namespace Code.Model.Dialogue.StoryModel
{
    /// <summary>
    /// This class provides the story asset files
    /// </summary>
    public static class StoryAssetModel
    {
        /// <summary>
        /// Class to store story asset files
        /// </summary>
        public class StoryAssetModelObject
        {
            public string Filename { get; set; }
            public StoryAssetController Asset { get; set; }
        }

        /// <summary>
        /// The Current active asset
        /// </summary>
        public static StoryAssetController CurrentAsset { get; set; }

        /// <summary>
        /// Story files property
        /// </summary>
        public static List<StoryAssetModelObject> StoryAssets { get; set; } = new ();

        /// <summary>
        /// Gets all story asset files
        /// Adds them to the StoryFiles list
        /// </summary>
        public static void GetAllStoryFiles()
        {
            var assets = Resources.LoadAll<StoryAssetController>("StoryAssets");
            foreach (var asset in assets)
                StoryAssets.Add(new StoryAssetModelObject{ Filename = asset.name, Asset = asset});
        }

        /// <summary>
        /// Gets the according asset
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns>asset file</returns>
        public static StoryAssetController GetAsset(string assetName)
        {
            var currentAsset = (from asset in StoryAssets where asset.Filename.Equals(assetName) select asset.Asset)
                .FirstOrDefault();
            CurrentAsset = currentAsset;
            return currentAsset;
        }
    }
}