using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private static readonly string host = "192.168.0.28";
    private static readonly int port = 8500;
    private static readonly Dictionary<string, IPEndPoint> userAddresses = new Dictionary<string, IPEndPoint>();
    private static Socket serverSocket;
    private static string username;

    static void Main(string[] args)
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        serverSocket.EnableBroadcast = true;
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));

        Console.Write("Enter your username: ");
        username = Console.ReadLine();
        Console.WriteLine($"Welcome, {username}!");
        Console.WriteLine("To send a broadcast message, type 'b:message'.");
        Console.WriteLine("To send a message to a specific user, type 'u:user:message'.");

        SendHelloWorld();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.IsBackground = true;
        receiveThread.Start();

        SendMessages();
    }

    static void SendHelloWorld()
    {
        byte[] data = Encoding.ASCII.GetBytes($"new:u:{username}");
        serverSocket.SendTo(data, new IPEndPoint(IPAddress.Broadcast, port));
    }

    static void ReceiveMessages()
    {
        while (true)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[1024];
            int bytesRead = serverSocket.ReceiveFrom(buffer, ref remoteEP);
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            string[] parts = message.Split(':');
            string state = parts[0];
            string bOrU = parts[1];
            string name = parts[2];

            if (((IPEndPoint)remoteEP).Address.ToString() == host)
                continue;

            if (state == "new")
            {
                AddNewUser(name, (IPEndPoint)remoteEP);
                continue;
            }

            if (state == "exit")
            {
                ExitUser(name);
                continue;
            }

            Console.WriteLine(message);
        }
    }

    static void AddNewUser(string name, IPEndPoint address)
    {
        if (userAddresses.ContainsKey(name))
            return;

        userAddresses[name] = address;
        byte[] data = Encoding.ASCII.GetBytes($"new:u:{username}");
        serverSocket.SendTo(data, address);
        Console.WriteLine($"New user {name}");
    }

    static void ExitUser(string name)
    {
        userAddresses.Remove(name);
        Console.WriteLine($"User {name} has left");
        //Environment.Exit();
    }

    static void SendMessages()
    {
        while (true)
        {
            string message = Console.ReadLine();

            if (message.StartsWith("b:"))
            {
                byte[] data = Encoding.ASCII.GetBytes($"From broadcast:b:{username}:{message.Split(':')[1]}");
                serverSocket.SendTo(data, new IPEndPoint(IPAddress.Broadcast, port));
            }
            else if (message.StartsWith("u:"))
            {
                string[] parts = message.Split(':', 3);
                string targetUsername = parts[1];
                string messageContent = parts[2];
                if (userAddresses.ContainsKey(targetUsername))
                {
                    IPEndPoint targetAddress = userAddresses[targetUsername];
                    byte[] data = Encoding.ASCII.GetBytes($"From user:u:{username}:{messageContent}");
                    serverSocket.SendTo(data, targetAddress);
                }
                else
                {
                    Console.WriteLine($"User '{targetUsername}' does not exist or is not connected.");
                }
            }
            else if (message == "exit")
            {
                byte[] data = Encoding.ASCII.GetBytes($"exit:b:{username}");
                serverSocket.SendTo(data, new IPEndPoint(IPAddress.Broadcast, port));
            }
            else
            {
                Console.WriteLine("Invalid command. Please start your message with 'b:' for broadcast or 'u:' for user-specific message.");
            }
        }
    }
}
