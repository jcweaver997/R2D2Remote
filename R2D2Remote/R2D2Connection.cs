﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R2D2PI
{
    class R2D2Connection
    {

        public enum ConnectionType
        {
            R2D2, Controller
        }

        public enum Commands
        {
            SetLeftDriveMotor, SetRightDriveMotor, ReceivedMessage
        }

        public struct Command
        {
            public Commands commandID { get; set; }
            public byte[] param { get; set; }
            public Command(Commands comid, byte[] para)
            {
                param = para;
                commandID = comid;
            }
        }
        private Socket socket;
        private IPEndPoint other;
        public delegate void OnCommandRecieve(Command c);
        public ConnectionType connectionType { get; private set; }
        private const int port = 1296;
        private const int maxMessageSize = 10;
        private const string robotHostName = "R2D2.local";
        private const int timeout = 300;
        public OnCommandRecieve onCommandRecieve { get; set; }
        private Thread listenThread;
        private bool listenerRunning;
        private bool commandRecieved;
        private byte[] lastCommandValue;
        private bool connected = false;


        public R2D2Connection(ConnectionType type, OnCommandRecieve onCommandRecieve = null)
        {
            connectionType = type;
            this.onCommandRecieve = onCommandRecieve;
        }

        public void Connect()
        {
            switch (connectionType)
            {
                case ConnectionType.Controller:
                    ConnectController();
                    break;
                case ConnectionType.R2D2:
                    ConnectR2D2();
                    break;
                default:
                    Console.WriteLine("Connection type not defined");
                    break;
            }

        }
        private IPAddress GetRobotIP()
        {
            IPHostEntry entry = null;
            try
            {
                entry = Dns.GetHostEntry(robotHostName);
            }catch(Exception e)
            {
                Console.WriteLine("Can't find the robot in dns");
                Console.WriteLine("Trying again in 5 seconds...");
                System.Threading.Thread.Sleep(5000);

                return GetRobotIP();
            }

            if (entry.AddressList.Length <= 0)
            {
                Console.WriteLine("Can't find the robot in DNS");
                Console.WriteLine("Trying again in 5 seconds...");
                System.Threading.Thread.Sleep(5000);

                return GetRobotIP();

            }
            Console.WriteLine("DNS found:");
            int index = -1;
            for (int i = 0; i < entry.AddressList.Length; i++)
            {
                if (entry.AddressList[i].AddressFamily == AddressFamily.InterNetworkV6)
                {
                    Console.WriteLine("ipv6: " + entry.AddressList[i].ToString());
                }
                else if (entry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    index = i;
                    Console.WriteLine("ipv4: " + entry.AddressList[i].ToString());
                    break;
                }
            }
            if (index == -1)
            {
                Console.WriteLine("Can't find a good ip in dns");
                Console.WriteLine("Trying again in 5 seconds...");
                System.Threading.Thread.Sleep(5000);
                return GetRobotIP();
            }
            Console.WriteLine("Found R2D2's ip address "+ entry.AddressList[index].ToString());
            return entry.AddressList[index];
        }

        private void ConnectController()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);

            IPAddress serverAddr = GetRobotIP();

            other = new IPEndPoint(serverAddr, port);
            socket.Connect(other);
            listenThread = new Thread(ListenThread);
            listenerRunning = true;
            listenThread.Start();
        }

        private void ConnectR2D2()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            socket.Bind(new IPEndPoint(IPAddress.Any,port));
            other = new IPEndPoint(IPAddress.Any, port);
            Console.WriteLine("waiting for connection from controller...");
            listenThread = new Thread(ListenThread);
            listenerRunning = true;
            listenThread.Start();
        }

        private void ListenThread()
        {
            while (listenerRunning)
            {
                byte[] buffer = new byte[maxMessageSize];
                EndPoint remote = (EndPoint)other;
                int numReceived = 0;

                if (connectionType == ConnectionType.R2D2)
                {
                    remote = new IPEndPoint(IPAddress.Any, port);
                    numReceived = socket.ReceiveFrom(buffer, ref remote);
                    other = (IPEndPoint)remote;
                }
                else if (connectionType == ConnectionType.Controller)
                {
                    numReceived = socket.Receive(buffer);
                }

                if (numReceived != 5)
                {
                    Console.WriteLine("Packet size is wrong! "+numReceived+" from "+other.Address.ToString());
                    continue;
                }
                connected = true;

                try
                {
                    byte[] param = new byte[4];
                    Array.Copy(buffer, 1, param, 0, 4);
                    Command c = new Command((Commands)buffer[0], param);
                    if (c.commandID == Commands.ReceivedMessage)
                    {

                        if (commandRecieved == false && CheckSum(lastCommandValue, c.param))
                        {
                            commandRecieved = true;
                        }
                    }
                    else
                    {
                        onCommandRecieve(c);
                        c.commandID = Commands.ReceivedMessage;
                        SendCommand(c);

                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine("Error in message handling! "+e.Message);
                }
            }

        }

        private bool CheckSum(byte[] ar1, byte[] ar2)
        {
            int firstSum = 0, secondSum = 0;
            for (int i = 0; i < Math.Min(ar1.Length, ar2.Length); i++)
            {
                firstSum += ar1[i];
                secondSum += ar2[i];
                firstSum %= 256;
                secondSum %= 256;
            }
            return firstSum == secondSum;

        }

        public void SendCommand(Command c, bool waitForResponse = false)
        {
            byte[] buffer = new byte[5];
            buffer[0] = (byte)c.commandID;
            Array.Copy(c.param,0, buffer,1, 4);
            commandRecieved = false;

            if (connectionType == ConnectionType.R2D2)
            {
                if (!connected)
                {
                    return;
                }
                socket.SendTo(buffer, other);
            }else if (connectionType == ConnectionType.Controller)
            {
                socket.Send(buffer);
            }
            
            lastCommandValue = c.param;

            int counter = 0;
            while (waitForResponse && !commandRecieved)
            {
                counter++;
                Thread.Sleep(1);
                if (counter>timeout)
                {
                    Console.WriteLine((connectionType == ConnectionType.R2D2 ? "Cont:" : "R2D2:") + " No response" +"!");
                    break;
                }
            }

        }
    }
}
