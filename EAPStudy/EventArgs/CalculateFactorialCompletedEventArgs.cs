using System;
using System.ComponentModel;

namespace EAPStudy.EventArgs
{
    public class CalculateFactorialCompletedEventArgs : AsyncCompletedEventArgs
    {
        private decimal resultValue = 0;
        public decimal ResultValue
        {
            get
            {
                //비동기가 실패 할 경우 예외 발생 
                base.RaiseExceptionIfNecessary();
                return resultValue;
            }
        }
        public CalculateFactorialCompletedEventArgs( decimal result , Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            resultValue = result;
        }
    }
}