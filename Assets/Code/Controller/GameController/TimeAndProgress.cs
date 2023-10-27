using System;
using System.Diagnostics;
using Code.Model.Dialogue.StoryDialogue;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Controller.GameController
{
    /// <summary>
    /// Calculates the elapsed Time and Progress
    /// </summary>
    /// <para name="author">Kevin von Ballmoos</para>
    /// <para name="date">27.04.2022</para>
    public static class TimeAndProgress
    {
        // Stopwatch, TimeSpan
        private static Stopwatch _stopwatch;
        private static TimeSpan _elapsedTime;
        // Object
        private static Object[] _stories;
        // Chapter Count
        private static int _chapterCount;
        // Chapter, Node Percentage
        private static double _chapterPercentage;
        private static double _nodePercentage;

        /// <summary>
        /// Starts the Time
        /// </summary>
        public static void StartTime()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        ///  Stops the Time
        /// </summary>
        public static void StopTime()
        {
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.Elapsed;
        }

        /// <summary>
        /// Returns the elapsed Time
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetElapsedTime()
        {
            return _elapsedTime;
        }

        /// <summary>
        ///  Calculates the Progress
        /// </summary>
        /// <param name="chapter">The chapter is used to calculate the progress</param>
        public static void CalculateProgress(string chapter)
        { 
            _stories = Resources.LoadAll($@"StoryAssets/", typeof(StoryAssetModel));
            _chapterPercentage = GetChapterPercentage(chapter);
            _nodePercentage = Math.Round(_chapterPercentage / GetStoryNodeCount(), 2);
        }

        /// <summary>
        /// Returns the progress
        /// </summary>
        /// <returns></returns>
        public static double GetProgress(string node)
        {
            var story = (StoryAssetModel)_stories[_chapterCount];
            return node.Equals(story.StoryNodes[0].name)? _chapterPercentage * (_chapterCount) : _nodePercentage;
        }

        /// <summary>
        /// Returns the percentage of the Chapter
        /// </summary>
        /// <param name="chapter"></param>
        private static int GetChapterPercentage(string chapter)
        {
            for (var i = 0; i < _stories.Length; i++)
            {
                if (!_stories[i].name.Equals(chapter)) continue;
                _chapterCount = i;
                break;
            }

            return 100 / _stories.Length;
        }

        /// <summary>
        /// Returns the percentage of the node
        /// </summary>
        /// <returns></returns>
        private static int GetStoryNodeCount()
        {
            var story = (StoryAssetModel)_stories[_chapterCount];
            var count = 0;
            foreach (var child in story.StoryNodes)
            {
                if (!child.IsChoiceNode)
                    count++;
            }
            return count -2;
        }
    }
}