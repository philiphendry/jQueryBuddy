using System;
using System.IO;
using System.Threading;

namespace jQueryBuddy.Utilities
{
    public class FileUtilities
    {
        public static bool WaitForExclusiveAccess(string filename, int timeout)
        {
            const int retryPause = 100;

            var startTime = DateTime.Now.Ticks;
            do
            {
                try
                {
                    // Try to open the file for exclusive access
                    using (new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        return true;
                    }
                }
                // Not ideal checking for an exception, especially one which may be raised for a number of
                // reasons but this may be the only way.
                catch (IOException)
                {
                    Thread.Sleep(retryPause);
                }
            } while (new TimeSpan(DateTime.Now.Ticks - startTime).TotalSeconds < timeout);

            return false;
        }
    }
}