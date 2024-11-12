using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _11_12_WCFChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri wsdl_uri = new Uri(ConfigurationManager.AppSettings["wsdl_uri"]);
            Uri nettcp_uri = new Uri(ConfigurationManager.AppSettings["nettcp_uri"]);
            //Contract-> Setting
            //Binding -> App.Config
            ServiceHost host = new ServiceHost(typeof(_11_12_WCFChatServer.ChatService));

            //오픈
            host.Open();
            Console.WriteLine("채팅 서비스를 시작합니다...");
            Console.WriteLine("WSDL_URI 주소 : " + wsdl_uri);
            Console.WriteLine("NETTCP_URI주소 : " + nettcp_uri);
            Console.WriteLine("멈추시려면 엔터를 눌러주세요..");
            Console.ReadLine();
            //서비스
            host.Abort();
            host.Close();
        }
    }
}
