using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    const int PORT_NO = 5000;
    const string SERVER_IP = "127.0.0.1";

    public static void Main()
    {
        StartServer();
    }

    static void StartServer()
    {
        Console.WriteLine("Starting echo server...");

        int port = 1234;
        TcpListener listener = new TcpListener(IPAddress.Loopback, port);
        try
        {
            listener.Start();
        }
        catch (Exception)
        {
            StartClient();
            throw;
        }
        
        Task<TcpClient> client;

        client = listener.AcceptTcpClientAsync();
        client.Wait();

        NetworkStream stream = client.Result.GetStream();
        StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
        StreamReader reader = new StreamReader(stream, Encoding.ASCII);

        while (true)
        {
            string inputLine = "";
            while (inputLine != null)
            {
                inputLine = reader.ReadLine();
                writer.WriteLine("Echoing string: " + inputLine);
                Console.WriteLine("Echoing string: " + inputLine);
            }
            Console.WriteLine("Server saw disconnect from client.");
        }

    }

    static void StartClient()
    {

        Console.WriteLine("Starting echo client...");

        int port = 1234;
        TcpClient client = new TcpClient();
        Task tsk = client.ConnectAsync("localhost", port);
        tsk.Wait();
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        while (true)
        {
            Console.Write("Enter text to send: ");
            string lineToSend = Console.ReadLine();
            Console.WriteLine("Sending to server: " + lineToSend);
            writer.WriteLine(lineToSend);
            string lineReceived = reader.ReadLine();
            Console.WriteLine("Received from server: " + lineReceived);
        }
    }
}
