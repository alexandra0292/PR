using System;
using System.Net;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pentru a rezolva un domeniu sau o adresă IP, utilizati comanda:");
            Console.WriteLine("1. resolve <domain sau ip>");
            Console.WriteLine("Pentru a schimba serverul DNS, utilizati comanda:");
            Console.WriteLine("2. use dns <ip>");
            Console.WriteLine();

            string currentDns = Dns.GetHostName();
            bool exit = false; //pentru controlul buclei while

            while (!exit)
            {
                string input = Console.ReadLine();

                string[] inputParts = input.Split(' ');

                if (inputParts[0].ToLower() == "resolve")
                {
                    if (inputParts.Length != 2)
                    {
                        Console.WriteLine("Comanda invalida. Folositi: resolve <domain sau ip>");
                    }
                    else
                    {
                        string query = inputParts[1]; //extrage domeniul sau IP-ul din input

                        try
                        {
                            if (IsIpAddress(query))
                            {
                                //daca este adresa IP, se obtine numele host-ului- domeniu
                                IPHostEntry hostEntry = Dns.GetHostEntry(query);
                                Console.WriteLine($"{query} -> {hostEntry.HostName}");
                            }
                            else
                            {
                                //domeniu - ip address
                                IPAddress[] addresses = Dns.GetHostAddresses(query);

                                foreach (IPAddress address in addresses)
                                {
                                    Console.WriteLine($"{query} -> {address}");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Eroare la rezolvarea DNS: {e.Message}");
                        }
                    }
                }
                else if (inputParts[0].ToLower() == "use" && inputParts[1].ToLower() == "dns")
                {
                    if (inputParts.Length != 3)
                    {
                        Console.WriteLine("Comanda invalida. Folositi: use dns <ip>");
                    }
                    else
                    {
                        string newDns = inputParts[2];
                        try
                        {
                            //verifica daca ip-ul este corect -> seteaza noul server la dns
                            Dns.GetHostEntry(newDns);
                            currentDns = newDns;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Eroare la setarea DNS-ului: {e.Message}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Comanda invalida.");
                }
            }
        }

        static bool IsIpAddress(string input)
        {
            return IPAddress.TryParse(input, out _);
        }
    }
}
