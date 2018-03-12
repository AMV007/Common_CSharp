using System;

namespace CommonControls.CommonMethods.Archive.Tar
{
    public class TarException : Exception
    {
        public TarException(string message) : base(message)
        {
        }
    }
}