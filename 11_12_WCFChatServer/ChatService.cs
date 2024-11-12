using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _11_12_WCFChatServer
{
    public delegate void ChatDele(string type, string nickname, string msg, DateTime time);

    public class ChatService : IChat
    {
        private static Object syncObj = new Object();
        private static List<string> Chatter = new List<string>();
        private static List<IChatCallback> Callbacks = new List<IChatCallback>();

        private ChatDele MyChat;
        private static event ChatDele List;

        #region 클라이언트 -> 서비스
        public bool Join(string nickname, DateTime time)
        {
            lock (syncObj)
            {
                if (!Chatter.Contains(nickname))
                {
                    Chatter.Add(nickname);
                    var callback = OperationContext.Current.GetCallbackChannel<IChatCallback>();
                    Callbacks.Add(callback);

                    MyChat = new ChatDele(UserHandler);
                    List += MyChat;
                    BroadcastMessage("Join[입장]", nickname, $"{nickname}님이 입장하셨습니다.", time);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public void Leave(string nickname, DateTime time)
        {
            lock (syncObj)
            {
                int index = Chatter.IndexOf(nickname);
                if (index != -1)
                {
                    BroadcastMessage("Leave[퇴장]", nickname, $"{nickname}님이 퇴장하셨습니다.", time);
                    Chatter.RemoveAt(index);
                    Callbacks.RemoveAt(index); // 콜백 제거
                }
            }
        }
        public void Say(string nickname, string msg, DateTime time)
        {
            BroadcastMessage("Say[메시지]", nickname, msg, time);
        }
        #endregion
        private void BroadcastMessage(string msgType, string nickname, string msg, DateTime time)
        {
            MyChat.BeginInvoke(msgType, nickname, msg, time, new AsyncCallback(EndAsync), null);
        }
        private void UserHandler(string msgType, string nickname, string msg, DateTime time)
        {
            List<IChatCallback> faultedCallbacks = new List<IChatCallback>(); // Faulted 상태 콜백 저장용 리스트

            try
            {
                foreach (var callback in Callbacks.ToList()) // .ToList()로 현재 콜백 리스트 복사
                {
                    try
                    {
                        switch (msgType)
                        {
                            case "Join[입장]":
                                callback.Join_Ack(nickname, time);
                                break;
                            case "Leave[퇴장]":
                                callback.Leave_Ack(nickname, time);
                                break;
                            case "Say[메시지]":
                                callback.Say_Ack(nickname, msg, time);
                                break;
                        }
                    }
                    catch (CommunicationException)
                    {
                        // Faulted 상태 콜백을 별도의 리스트에 저장
                        faultedCallbacks.Add(callback);
                    }
                }
                // Faulted 상태 콜백을 foreach 루프가 끝난 후 한 번에 제거
                lock (syncObj)
                {
                    foreach (var faultedCallback in faultedCallbacks)
                    {
                        int index = Callbacks.IndexOf(faultedCallback);
                        if (index != -1)
                        {
                            Callbacks.RemoveAt(index);
                            Chatter.RemoveAt(index);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("{0} 에러", nickname);
            }
        }
        private void EndAsync(IAsyncResult ar)
        {
            ChatDele d = null;
            try
            {
                System.Runtime.Remoting.Messaging.AsyncResult asres = (System.Runtime.Remoting.Messaging.AsyncResult)ar;
                d = ((ChatDele)asres.AsyncDelegate);
                d.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
