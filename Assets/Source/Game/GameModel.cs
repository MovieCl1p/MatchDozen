using System;

namespace Game
{
    public class GameModel
    {
        public event Action<int> OnScoreChanged; 

        public event Action<int> OnMaxScoreNumberChanged; 

        private int _score;
        private int _maxScoreNumber;

        public int Score
        {
            get { return _score; }
            set
            {
                _score = value;
                CallScoreChanged(_score);
            }
        }

        public int MaxScoreNumber
        {
            get { return _maxScoreNumber; }
            set
            {
                _maxScoreNumber = value;
                CallMaxScoreNumberChanged(_maxScoreNumber);
            }
        }

        private void CallMaxScoreNumberChanged(int maxScoreNumber)
        {
            if (OnMaxScoreNumberChanged != null)
            {
                OnMaxScoreNumberChanged(maxScoreNumber);
            }
        }

        private void CallScoreChanged(int score)
        {
            if (OnScoreChanged != null)
            {
                OnScoreChanged(score);
            }
        }
    }
}
