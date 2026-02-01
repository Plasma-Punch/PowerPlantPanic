using UnityEngine;
using TMPro;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using System.Collections.Generic;

namespace LeaderboardCreatorDemo
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> _scores = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> _names = new List<TextMeshProUGUI>();
        [SerializeField] private TMP_InputField _usernameInputField;

        // Make changes to this section according to how you're storing the player's score:
        // ------------------------------------------------------------
        [SerializeField] private FloatReference _timer;
        [SerializeField] private TextMeshProUGUI _currentScore;

        private float Score => _timer.value;
        private bool _hasSubmitted;
        private int _entriesCount;
        // ------------------------------------------------------------

        private void Start()
        {
            int minutes = (int)_timer.value / 60;
            int seconds = (int)_timer.value % 60;
            _currentScore.text = "Current Score: " + string.Format("{0:00}:{1:00}", minutes, seconds);
            LoadEntries();
        }

        private void LoadEntries()
        {
            // Q: How do I reference my own leaderboard?
            // A: Leaderboards.<NameOfTheLeaderboard>

            Leaderboards.PowerPlantPanic.GetEntries(entries =>
            {
                foreach (var t in _names)
                    t.text = "";
                foreach (var t in _scores)
                    t.text = "";

                var length = Mathf.Min(_names.Count, entries.Length);
                _entriesCount = entries.Length;

                for (int i = 0; i < length; i++)
                {
                    _names[i].text = $"{entries[i].Rank}. {entries[i].Username}";
                    int minutes = (int)entries[i].Score / 60;
                    int seconds = (int)entries[i].Score % 60;
                    _scores[i].text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }
            });
        }

        public void UploadEntry()
        {
            if (_hasSubmitted) return;
            Leaderboards.PowerPlantPanic.UploadNewEntry(_usernameInputField.text, (int)Score, $"{_entriesCount}", isSuccessful =>
            {
                Leaderboards.PowerPlantPanic.ResetPlayer();
                if (isSuccessful)
                {
                    _hasSubmitted = true;
                    LoadEntries();
                }
            });
        }
    }
}
