using System.Runtime.InteropServices;
using System.Windows;

namespace BookEdit.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private const long INTERNET_CONNECTION_MODEM = 1;//Local system uses a modem to connect to the Internet.
        //private const long INTERNET_CONNECTION_LAN = 2; //Local system uses a local area network to connect to the Internet.
        //private const long INTERNET_CONNECTION_PROXY = 4;//Local system uses a proxy server to connect to the Internet.
        //private const long INTERNET_CONNECTION_MODEM_BUSY = 8;   //No longer used.
        //private const long INTERNET_CONNECTION_CONFIGURED = 64; //Local system has a valid connection to the Internet, but it might or might not be currently connected.
        //private const long INTERNET_CONNECTION_OFFLINE = 32; // Local system is in offline mode.
        //private const long INTERNET_RAS_INSTALLED = 16; //Local system has RAS installed.
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    long lfag;
        //    string strConnectionDev = "";
        //    if (InternetGetConnectedState(out lfag, 0))
        //        strConnectionDev = "网络连接正常!";
        //    else
        //        strConnectionDev = "网络连接不可用!";
        //    if ((lfag & INTERNET_CONNECTION_OFFLINE) > 0)
        //        strConnectionDev += "OFFLINE 本地系统处于离线模式。";
        //    if ((lfag & INTERNET_CONNECTION_MODEM) > 0)
        //        strConnectionDev += "Modem 本地系统使用调制解调器连接到互联网。";
        //    if ((lfag & INTERNET_CONNECTION_LAN) > 0)
        //        strConnectionDev += "LAN 本地系统使用的局域网连接到互联网。";
        //    if ((lfag & INTERNET_CONNECTION_PROXY) > 0)
        //        strConnectionDev += "a   Proxy";
        //    if ((lfag & INTERNET_CONNECTION_MODEM_BUSY) > 0)
        //        strConnectionDev += "Modem   but   modem   is   busy";
        //    MessageBox.Show(strConnectionDev); 
        //    base.OnStartup(e);
        //}

        ////定义（引用）API函数
        //[DllImport("wininet.dll")]
        //public static extern bool InternetGetConnectedState(out long lpdwFlags, long dwReserved);

    }
}
