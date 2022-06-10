using EAPStudy.Delegate;
using EAPStudy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPStudy.EPA_Multi
{
   
    
    public class CalculateFactorial
    {
        public event CalculateFactorialCompletedEventHandler CalculateFactorialCompleted;
        protected virtual void OnCalculateFactorialCompleted(CalculateFactorialCompletedEventArgs e)
        {
            CalculateFactorialCompleted?.Invoke(this, e);
        }

        //progress 이벤트 추가
        public event CalculateProgressChangedEventHandler CalculateProgressChanged;
        protected virtual void OnProgressChanged(CalculateProgressChangedEventArgs e)
        {
            CalculateProgressChanged?.Invoke(this, e);
        }
    }
}
