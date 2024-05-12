using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NTPClient
{
    public DateTime GetNetworkTime()
    {
        const string ntpServer = "pool.ntp.org";
        var ntpData = new byte[48];
        ntpData[0] = 0x1B;

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;
        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        socket.SendTimeout = 3000;
        socket.ReceiveTimeout = 3000;

        socket.Connect(ipEndPoint);

        socket.Send(ntpData);
        socket.Receive(ntpData);

        byte offsetTransmitTime = 40;
        ulong intpart = 0;
        ulong fractpart = 0;

        for (var i = 0; i <= 3; i++)
            intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

        for (var i = 4; i <= 7; i++)
            fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

        ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

        socket.Close();

        var ntpDateTime = new DateTime(1900, 1, 1);
        ntpDateTime = ntpDateTime.AddMilliseconds((long)milliseconds);

        var networkDateTime = ntpDateTime.ToLocalTime();
        return networkDateTime;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var client = new NTPClient();

        while (true)
        {
            Console.WriteLine("Introduceti zona geografica (o cifră a zonei') sau 'exit' pentru a inchide programul: ");
            var userInput = Console.ReadLine();

            if (userInput == "exit")
                break;

            GetLocalTime(userInput, client);
        }
    }

    public static void GetLocalTime(string timeZone, NTPClient client)
    {
        try
        {
            var offset = int.Parse(timeZone.Replace("GMT", ""));

            if (offset > 12 || offset < -12)
            {
                Console.WriteLine("Introduceti o valoare intre -12 si +12");
                return;
            }

            DateTime ntpTime;
            try
            {
                ntpTime = client.GetNetworkTime();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Eroare la accesarea NTP: {e.Message}");
                return;
            }

            var localTime = ntpTime.AddHours(offset);
            Console.WriteLine($"Ora exacta pentru zona GMT {timeZone} este: {localTime.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Formatul zonei geografice nu este corect. Introduceti valori numerice.");
        }
    }
}
