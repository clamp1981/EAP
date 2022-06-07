using System.ComponentModel;

namespace EAPStudy.EventArgs
{
    public class CalculateProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public int Percent
        {
            get;
            private set;
        }

        public object UserState
        {
            get;
            private set;
        }
        public CalculateProgressChangedEventArgs( int percent, object userState ) 
            : base(percent, userState)
        {
            this.Percent = percent;
            this.UserState = userState;
        }
    }
}