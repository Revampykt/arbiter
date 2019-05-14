using System;

namespace Arbiter.Misc
{
    public class StringArgs : EventArgs
    {
        public string str;

        public StringArgs(string str)
        {
            this.str = str;
        }
    }
}