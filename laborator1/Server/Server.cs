using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static List<Socket> clients = new List<Socket>();

    static void Main()
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        serverSocket.Bind(new IPEndPoint(ipAddress, 9000));
        serverSocket.Listen(5);

        Console.WriteLine("Waiting for a connection...");

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clients.Add(clientSocket);
            Console.WriteLine($"Connection accepted from {clientSocket.RemoteEndPoint}");

            Thread clientThread = new Thread(() => HandleClient(clientSocket));
            clientThread.Start();
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        try
        {
            string userName = $"User {clientSocket.RemoteEndPoint}";

            while (true)
            {
                string receivedText = "";

                do
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    receivedText += Encoding.UTF8.GetString(buffer, 0, bytesRead);
                } while (clientSocket.Available > 0);

                Console.WriteLine($"{userName} - {receivedText}");

                BroadcastMessage(userName, receivedText);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            clients.Remove(clientSocket); // Remove the disconnected client from the list
            Console.WriteLine($"Client {clientSocket.RemoteEndPoint} disconnected.");
            clientSocket.Close();
        }
    }

    static void BroadcastMessage(string userName, string message)
    {
        foreach (var client in clients)
        {
            try
            {
                string fullMessage = $"{userName}: {message}";
                byte[] messageData = Encoding.UTF8.GetBytes(fullMessage);
                client.Send(messageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting message: {ex.Message}");
            }
        }
    }
}

