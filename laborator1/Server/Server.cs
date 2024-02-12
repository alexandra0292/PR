using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static void Main()
    {
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        serverSocket.Bind(new IPEndPoint(ipAddress, 9000));
        serverSocket.Listen(5);

        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            Console.WriteLine($"Connection accepted from port {clientSocket.RemoteEndPoint}");

            // separate thread to handle each client
            Thread clientThread = new Thread(() => HandleClient(clientSocket));
            clientThread.Start();
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        try
        {
            string text = "";

            do
            {
                byte[] buffer = new byte[1024];
                int bytesRead = clientSocket.Receive(buffer);
                text += Encoding.UTF8.GetString(buffer, 0, bytesRead);
            } while (clientSocket.Available > 0);

            Console.WriteLine($"Received from {clientSocket.RemoteEndPoint}: {text}");

            byte[] response = Encoding.UTF8.GetBytes("Server received your message");
            clientSocket.Send(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}

