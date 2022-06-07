using EAPStudy.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EAPStudy.EPA_Single
{
    public delegate void CalculateCompletedEventHandler(object sender, CalculateCompletedEventAgrs e);
    public delegate void CalculateProgressChangedEventHandler(object sender, CalculateProgressChangedEventArgs e);

    public delegate void CalculateWorkerEventHandler(object argument);
    public class CalculateFactorial
    {

        //종료 이벤트 추가 
        public event CalculateCompletedEventHandler CalculateCompleted;
        protected virtual void OnCalculateCompleted(CalculateCompletedEventAgrs e)
        {
            CalculateCompleted?.Invoke(this, e);
        }

        //progress 이벤트 추가
        public event CalculateProgressChangedEventHandler CalculateProgressChanged;
        protected virtual void OnProgressChanged(CalculateProgressChangedEventArgs e)
        {
            CalculateProgressChanged?.Invoke(this, e);
        }

        //비동기 작업에서 결과 및 과정 이벤트를 발생시키는 ojbect;
        private AsyncOperation asyncOperation;

        //AsyncOperation의 post() or PostOperationCompleted() 호출의 대리자 
        //이벤트가 발생하도록 하는 부분이 아래 델리게이트를 호출하는 부분
        private SendOrPostCallback onCompletedDelegate;
        private SendOrPostCallback onProgressChangedDelegate;

        //비동기 시작 델리게이트 선언
        private CalculateWorkerEventHandler calculateWokerDelegate;

        public bool IsBusy { get; private set; }

        //progress 사용 여부 
        public bool WorkerReportsProgress { get; set; }

        //cancel  사용여부
        public bool WorkerSupportsCancellation { get; set; }





        //중지 여부 
        public bool CancellationPending { get; private set; }

        public CalculateFactorial()
        {
            InitializeDelegate();
        }

        private void InitializeDelegate()
        {
            this.onCompletedDelegate = new SendOrPostCallback(SendOperationCalculateCompleted);
            this.onProgressChangedDelegate = new SendOrPostCallback(SendOperationCalculateProgressChanged);
            this.calculateWokerDelegate = new CalculateWorkerEventHandler(CalculateWorker);

        }

        

        public void CalculateAsync( object argument )
        {
            this.IsBusy = true;
            this.CancellationPending = false;
            this.asyncOperation = AsyncOperationManager.CreateOperation(null);
            this.calculateWokerDelegate.BeginInvoke(argument, null, null);

        }

        public void CancelAsync()
        {
            if( !this.WorkerSupportsCancellation)
                throw new InvalidOperationException("WorkerSupportsCancellation is false");

            this.CancellationPending = true;
        }


        //SendOrPostCallback onCompletedDelegate 과 연결
        private void SendOperationCalculateCompleted(object state)
        {
            this.IsBusy = false;
            this.CancellationPending = false;

            CalculateCompletedEventAgrs e =
                (CalculateCompletedEventAgrs)state;

            if (e == null)
                return;

            OnCalculateCompleted(e);


        }

        //SendOrPostCallback onProgressChangedDelegate 과 연결
        private void SendOperationCalculateProgressChanged(object state)
        {
            CalculateProgressChangedEventArgs e =
                 (CalculateProgressChangedEventArgs)state;

            if (e == null)
                return;

            OnProgressChanged(e);
        }

        private void CalculateWorker(object argument)
        {
            object result = null;
            Exception e = null;
            try
            {
                result = this.Calculate((int)argument, true);
            }
            catch( Exception ex )
            {
                e = ex;
            }

            this.asyncOperation.PostOperationCompleted(this.onCompletedDelegate
                , new CalculateCompletedEventAgrs(result, e, this.CancellationPending, null));
        }


        private void ReportProgress(int percentProgress)
        {
            if (!this.WorkerReportsProgress)
                throw new InvalidOperationException("WorkerReportsProgress is false");
            CalculateProgressChangedEventArgs e =
                new CalculateProgressChangedEventArgs(percentProgress, null);
            if (this.asyncOperation != null)
            {
                this.asyncOperation.Post(this.onProgressChangedDelegate, e);
            }
            else
                this.onProgressChangedDelegate(e);
        }
        

        //실제 작업 함수들
        //계산 
        public int Calculate( int p , bool isAsync = false )
        {
            if (p <= 0)
                throw new InvalidOperationException("Can not calculate by input data!");

            int result = 1;
            for( int i =  1; i <= p; i++)
            {
                if (this.CancellationPending)
                    return result;

                result *= i;

                Thread.Sleep(1000);

                if (isAsync)
                    this.ReportProgress((int)((double)i / (double)p * 100));
            }

            return result;
        }
    }
}
