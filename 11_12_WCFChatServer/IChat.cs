using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _11_12_WCFChatServer
{

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IChatCallback))]
    public interface IChat
    {
        [OperationContract(IsOneWay = false, IsInitiating = true, IsTerminating = false)]
        bool Join(string nickname, DateTime time); //참여

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = false)]
        void Say(string nickname, string msg, DateTime time); //메시지 전송

        [OperationContract(IsOneWay = true, IsInitiating = false, IsTerminating = true)]
        void Leave(string nickname, DateTime time); //퇴장
    }
    public interface IChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void Join_Ack(string nickname, DateTime time);
        [OperationContract(IsOneWay = true)]
        void Say_Ack(string nickname, string msg, DateTime time);

        [OperationContract(IsOneWay = true)]
        void Leave_Ack(string nickname, DateTime time);
    }
}
