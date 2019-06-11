using System;
using System.Collections.Generic;

namespace Arbiter
{
    public class Solution
    {
        public string userName;
        public string realName;
        public Dictionary<string, Dictionary<string, Result>> results;
        public Dictionary<string, string> preliminary;
        public int total;
        public string language;
        public string source;
        public bool compilation;
        public DateTime time;
    }

    public class Result
    {
        public Verdict verdict;
        public float memoryUsed;
        public float elapsedTime;
    }
}