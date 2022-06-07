using System;
using System.ComponentModel;

namespace EAPStudy.EventArgs
{
    public class CalculateCompletedEventAgrs : AsyncCompletedEventArgs
    {
        private object result; 
        public object Result
        {
            get
            {
                //비동기가 실패 할 경우 예외 발생 
                base.RaiseExceptionIfNecessary();
                return result;
            }
        }

        //[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        //public object UserState
        //{
        //    get
        //    {

        //        return base.UserState;
        //    }

        //}
        


        public CalculateCompletedEventAgrs( object result, Exception error, bool cancelled, object userState ) 
            : base ( error, cancelled, userState )
        {
            this.result = result;
        }
    }
}