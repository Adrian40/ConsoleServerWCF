using System.Diagnostics;

namespace ConsoleServerWCF
{
    #region Logger class
    /// <summary>
    /// Alternative class for logging 
    /// </summary>
    class Logger
    {
        TextWriterTraceListener listener = new TextWriterTraceListener(@"C:\Users\User\Source\Repos\ClienServerApplicationWCF\ConsoleServerWCF\Logs\log.txt");
        TextWriterTraceListener errorlistener = new TextWriterTraceListener(@"C:\Users\User\Source\Repos\ClienServerApplicationWCF\ConsoleServerWCF\Logs\errorlog.txt");

        ///<param name="message"> 
        /// Message text!
        /// </param>
        public void Log(string message)
        {
            Trace.Listeners.Add(listener);
            Trace.Write("\n" + message);
            Trace.Flush();
            Trace.Listeners.Remove(listener);
        }
        public void Error(string error)
        {
            Trace.Listeners.Add(errorlistener);
            Trace.Write("\n" + error);
            Trace.Flush();
            Trace.Listeners.Remove(errorlistener);
        }
    }
    #endregion
    #region Owners class
    ///<summar>
    /// Alternative class for handling users
    /// </summar>
    class Owners
    {
        static string ownername;
        private static Owners owner = new Owners();
        protected static double workhours;

        private Owners()
        {
            workhours = 50;
        }
        /// <summary>
        /// Property where you can read back the points of users and modify the value of workpoints!
        /// </summary>
        public static double Workhours
        {
            get { return workhours; }
            set { workhours = value; }
        }
        /// <summary>
        /// Property where you can read back the name of user and you can write it in as a value!
        /// </summary>
        public static string Ownername
        {
            get { return ownername; }
            set { ownername = value; }
        }
    }
    #endregion
}