using System.Collections.Generic;
using UnityEngine;

namespace Lexicon.Data
{
    [CreateAssetMenu(fileName = "PuzzleDatabase", menuName = "Lexicon/Puzzle Database")]
    public class PuzzleDatabase : ScriptableObject
    {
        [SerializeField] private List<PuzzleData> puzzles = new List<PuzzleData>();
        
        public List<PuzzleData> Puzzles => puzzles;
        
        public int TotalPuzzles => puzzles.Count;
        
        public PuzzleData GetPuzzle(int index)
        {
            if (index >= 0 && index < puzzles.Count)
                return puzzles[index];
            return null;
        }
        
        public int GetPuzzleIndex(PuzzleData puzzle)
        {
            return puzzles.IndexOf(puzzle);
        }
        
        public bool IsLastPuzzle(int index)
        {
            return index >= puzzles.Count - 1;
        }
        
        public PuzzleData GetNextPuzzle(int currentIndex)
        {
            int nextIndex = currentIndex + 1;
            return GetPuzzle(nextIndex);
        }
    }
}

