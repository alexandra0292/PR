using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    static string userName = "";

    static void Main()
    {
        Console.WriteLine("Enter your username: ");
        userName = Console.ReadLine();

        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        int serverPort = 9000;

        using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            client.Connect(new IPEndPoint(serverIP, serverPort));
            Console.WriteLine($"Connected to the server as '{userName}'");

            // Thread for receiving messages
            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            while (true)
            {
                Console.WriteLine("Enter text to send to the server (or type 'exit' to quit): ");
                string text = Console.ReadLine() ?? "";

                if (text.ToLower() == "exit")
                    break;

                SendMessage(client, text);
            }
        }
    }

    static void SendMessage(Socket client, string message)
    {
        try
        {
            string fullMessage = $"{userName}: {message}";
            byte[] messageData = Encoding.UTF8.GetBytes(fullMessage);
            client.Send(messageData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    static void ReceiveMessages(Socket client)
    {
        try
        {
            while (true)
            {
                byte[] receivedBuffer = new byte[1024];
                int bytesRead = client.Receive(receivedBuffer);
                string receivedText = Encoding.UTF8.GetString(receivedBuffer, 0, bytesRead);

                if (!string.IsNullOrEmpty(receivedText))
                {
                    Console.WriteLine(receivedText);
                }
                else
                {
                    Console.WriteLine("Connection closed by the server.");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }
}

