using EAPStudy.Delegate;
using EAPStudy.EventArgs;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;


namespace EAPStudy.EAP_Multi
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



        //진행 사항 사용 여부       
        public bool WorkerReportsProgress { get; set; }

       

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

        //비동기 연산 완료 함수 구현
        //생성된 비동기의 GUID 삭제 
        //완료 이벤트를 넘김 ( 해당 개체(asyncOp) 는 더 이상 사용할 수 없음 )
        private void CompletionMethod(decimal result, Exception error, bool canceled, AsyncOperation asyncOp)
        {
            //취소가 아니면 비동기 작업의 고유 id를 가지는 개체를 ist에서 삭제 
            if (!canceled)
            {
                lock (userStateToLifetime.SyncRoot)
                    userStateToLifetime.Remove(asyncOp.UserSuppliedState);
            }

            CalculateFactorialCompletedEventArgs e =
                new CalculateFactorialCompletedEventArgs(result, error, canceled, asyncOp.UserSuppliedState);

            asyncOp.PostOperationCompleted(onCompletedDelegate, e);

        }

        //비동기 작업 취소 되었는지 여부 확인
        private bool TaskCanceled(object taskId)
        {
            return (userStateToLifetime[taskId] == null);
        }

        //실제 계산 하는 함수 구현
        private void CalculateWorker(int input, AsyncOperation asyncOp)
        {
            decimal result = 0;
            Exception e = null;

            //해당 비동기 작업이 살아 있는지 확인
            if (!TaskCanceled(asyncOp.UserSuppliedState))
            {
                try
                {
                    result = this.Calculate(input, asyncOp);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }

            CompletionMethod(result, e, TaskCanceled(asyncOp.UserSuppliedState), asyncOp);
        }


        //외부 노출되는 동기 함수
        public int Calculate(int input, AsyncOperation asyncOp)
        {
            if (input <= 0)
                throw new InvalidOperationException("Can not calculate by input data!");

            int result = 1;
            for (int i = 1; i <= input; i++)
            {
                if (TaskCanceled(asyncOp.UserSuppliedState))
                    return result;

                result *= i;

                Thread.Sleep(1000);

                if (WorkerReportsProgress)
                {
                    int percentProgress = (int)((double)i / (double)input * 100);
                    CalculateProgressChangedEventArgs e =
                    new CalculateProgressChangedEventArgs(percentProgress, null);
                    asyncOp.Post(this.onProgressChangedDelegate, e);
                }
            }

            return result;
        }



        
        //외부 노출되는 비동기 함수
        public void CalculateAsync(int input, object taskId)
        {
            //비동기 함수가 호출될때 마다 생성
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(taskId);

            //userStateToLifetime에 생성된 AsyncOperation 을 담아준다.
            lock ( userStateToLifetime.SyncRoot )
            {
                if( userStateToLifetime.Contains( taskId ))
                {
                    throw new ArgumentException(
                            "task id는 유일 해야해!!",
                            "taskId");
                }

                userStateToLifetime[taskId] = asyncOp;

            }

            CalculateFactorialWorkerEventHandler calculateWokerDelegate = new CalculateFactorialWorkerEventHandler(CalculateWorker);
            calculateWokerDelegate.BeginInvoke(input, asyncOp, null, null);
        }


        public void CancelCalculateAsync(object taskId)
        {
            AsyncOperation asyncOp = userStateToLifetime[taskId] as AsyncOperation;
            if (asyncOp != null)
            {
                lock (userStateToLifetime.SyncRoot)
                {
                    userStateToLifetime.Remove(taskId);
                }
            }            
        }
    }
}
