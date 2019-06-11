using System.Collections.Generic;

namespace Arbiter
{
    public class Task
    {
        public string name;
        public long timeout;
        public long timeLimit;
        public long memoryLimit;
        public string inputFile;
        public string outputFile;
        public Dictionary<string, string> preliminary;
        public Dictionary<string, Subtask> testSuites;
    }

    public class Subtask
    {
        public string name;
        public Scoring scoring;
        public int testScore;
        public string results;
    }

    public enum Scoring
    {
        Partial,
        Entire
    }
}