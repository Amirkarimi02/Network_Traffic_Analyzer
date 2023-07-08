using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetworkTrafficAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get the available network interfaces
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                // Display the available network interfaces
                for (int i = 0; i < interfaces.Length; i++)
                {
                    Console.WriteLine($"Interface #{i + 1}: {interfaces[i].Name}");
                }

                // Prompt the user to select a network interface
                Console.Write("Enter the interface number to capture traffic: ");
                int userput = Convert.ToInt32(Console.ReadLine());
                int selectedInterfaceIndex = userput - 1;

                // Retrieve the selected network interface
                NetworkInterface selectedInterface = interfaces[selectedInterfaceIndex];

                // Start capturing network traffic on the selected interface
                CaptureNetworkTraffic(selectedInterface);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void CaptureNetworkTraffic(NetworkInterface networkInterface)
        {
            // Ensure the selected interface supports network packet capture
            if (networkInterface.Supports(NetworkInterfaceComponent.IPv4) &&
                networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled)
            {
                Console.WriteLine("Capturing network traffic...");

                // Create a socket to capture network traffic
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP))
                {
                    // Bind the socket to the selected network interface
                    socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0));

                    // Set the socket options to capture all incoming and outgoing packets
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
                    byte[] inBuffer = new byte[4] { 1, 0, 0, 0 };
                    byte[] outBuffer = new byte[4] { 1, 0, 0, 0 };
                    socket.IOControl(IOControlCode.ReceiveAll, inBuffer, outBuffer);

                    // Start receiving network packets
                    byte[] buffer = new byte[4096];
                    while (true)
                    {
                        int bytesRead = socket.Receive(buffer);
                        Console.WriteLine($"Received {bytesRead} bytes");
                    }
                }
            }
            else
            {
                Console.WriteLine("Selected interface does not support network packet capture.");
            }
        }
    }
}
