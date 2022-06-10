using EAPStudy.EPA_Multi;
using EAPStudy.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAPStudy.Delegate
{
    public delegate void CalculateCompletedEventHandler(object sender, CalculateCompletedEventAgrs e);
    public delegate void CalculateProgressChangedEventHandler(object sender, CalculateProgressChangedEventArgs e);    
    public delegate void CalculateWorkerEventHandler(object argument);

    //다중 호출 구조의 완료 메서드는 따로 만들고 진행사항( Progress ) 이벤트는 같이 쓰기로 하자!!
    public delegate void CalculateFactorialCompletedEventHandler(object sender, CalculateFactorialCompletedEventArgs e);
}
