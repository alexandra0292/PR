
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    static void Main()
    {
        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        int serverPort = 9000;

        Thread receiveThread = new Thread(() => ReceiveMessages(serverIP, serverPort));
        receiveThread.Start();

        while (true)
        {
            Console.WriteLine("Enter text to send to the server (or type 'exit' to quit): ");
            //?? provide a default value
            string text = Console.ReadLine() ?? "";

            if (text.ToLower() == "exit")
                break;

            SendMessage(serverIP, serverPort, text);
        }
    }

    static void SendMessage(IPAddress serverIP, int serverPort, string message)
    {
        try
        {
            using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                client.Connect(new IPEndPoint(serverIP, serverPort));
                Console.WriteLine("Connected to the server");

                byte[] messageData = Encoding.UTF8.GetBytes(message);
                client.Send(messageData);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    static void ReceiveMessages(IPAddress serverIP, int serverPort)
    {
        try
        {
            while (true)
            {
                using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    client.Connect(new IPEndPoint(serverIP, serverPort));
                    Console.WriteLine("Connected to the server for receiving messages");

                    string receivedText = "";

                    do
                    {
                        byte[] receivedBuffer = new byte[1024];
                        int bytesReceived = client.Receive(receivedBuffer);
                        receivedText += Encoding.UTF8.GetString(receivedBuffer, 0, bytesReceived);
                    } while (client.Available > 0);

                    Console.WriteLine($"Server: {receivedText}\n");
                }

                Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }
}



