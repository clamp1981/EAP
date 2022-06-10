using EAPStudy.Delegate;
using EAPStudy.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
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


        //AsyncOperation의 post() or PostOperationCompleted() 호출의 대리자 
        //이벤트가 발생하도록 하는 부분이 아래 델리게이트를 호출하는 부분
        private SendOrPostCallback onCompletedDelegate;
        private SendOrPostCallback onProgressChangedDelegate;

        //task id와 객체를 보관할 수 있는 collection ( 스레드 동기화 객체가 제공됨 )
        //GUID 와 AsyncOperation을 보관
        //동작 실행 시 추가 되어 지며, 완료되면 삭제 한다.
        //호출 하는 쪽에서 해당 GUID로 검색해서 검색되면 아직 동작중이라는 의미. 
        private HybridDictionary userStateToLifetime = new HybridDictionary();

        public CalculateFactorial()
        {
            InitializeDelegate();
        }


        private void InitializeDelegate()
        {
            this.onCompletedDelegate = new SendOrPostCallback(SendOperationCalculateCompleted);
            this.onProgressChangedDelegate = new SendOrPostCallback(SendOperationCalculateProgressChanged);
            

        }

        private void SendOperationCalculateProgressChanged(object state)
        {
            CalculateProgressChangedEventArgs e =
                state as CalculateProgressChangedEventArgs;
            OnProgressChanged(e);
        }

        private void SendOperationCalculateCompleted(object state)
        {
            CalculateFactorialCompletedEventArgs e =
                state as CalculateFactorialCompletedEventArgs;
            OnCalculateFactorialCompleted(e);

        }
    }
}
