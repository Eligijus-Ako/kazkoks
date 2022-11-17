using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.IO;

namespace DeltaTCPClient
{
    public sealed class DeltaPLCModbus
    {
        private static DeltaPLCModbus instance = null;
        private static readonly object obj = new object();
        private static TcpClient tcpClient;
        static ConcurrentQueue<string> cmdQueue = new ConcurrentQueue<string>();
        static volatile bool poll = false;
        static readonly int modbusHeaderLength = 9;
        static byte[] PlcInputs;
        static byte[] PlcOutputs;
        static byte[] PlcRegisters;



        public void Connect(int pollingInterval = 100, string ipAddress = "192.168.1.5", int port = 502)
        {
            Task.Run(() => PollDelta(pollingInterval, ipAddress, port));
            poll = true;
            while (GetStatus() < 0)
            {
                System.Threading.Thread.Sleep(pollingInterval);
            }

        }

        public bool Connected()
        {
            return poll;
        }

        public void Disconnect()
        {
            poll = false;
        }
        static int plcStatus = -1;
        public int GetStatus()
        {
            return plcStatus;
        }

        static string lastError;
        public string GetLastError()
        {
            return lastError;
        }

        public int GetOutput(int address)
        {
            byte h = (byte)(address / 10);
            byte l = (byte)(address % 10);
            byte mask = (byte)(0b01 << l);
            int res = ((PlcOutputs[h] & mask)) > 0 ? 1 : 0;
            //Console.WriteLine(BitConverter.ToString(PlcOutputs).Replace("-", " "));
            return res;
        }
        public int GetInput(int address)
        {
            byte h = (byte)(address / 10);
            byte l = (byte)(address % 10);
            byte mask = (byte)(0b01 << l);
            int res = (PlcInputs[h] & mask) > 0 ? 1 : 0;
            //Console.WriteLine(BitConverter.ToString(PlcInputs).Replace("-", " "));
            return res;
        }

        public int GetRegister(int address)
        {
            // "00000000000000000000000000000000000003E900000000000000000000000000000002000107D2"
            int i = (address - 100) * 4;
            string hexStr = BitConverter.ToString(PlcRegisters).Replace("-", "");
            int res = Int32.Parse(hexStr.Substring(i, 4), System.Globalization.NumberStyles.HexNumber);
            //Console.WriteLine(BitConverter.ToString(PlcRegisters).Replace("-", " "));
            return res;
        }

        public int SetOutput(int address, bool value)
        {
            //00 05 00 00 00 06 00 05 05 00 ff 00
            int baseAddress = 1280;
            int addr = baseAddress + OctalToDecimal(address);
            var addrHex = addr.ToString("X4");
            string head = "00 05 00 00 00 06 00 05";
            string tail = value ? "FF 00" : "00 00";
            cmdQueue.Enqueue(head + addrHex + tail);
            return 0;
        }

        public int SetRegister(int address, int value)
        {
            //00 07 00 00 00 06 00 06 00 64 03 e8
            string head = "00 07 00 00 00 06 00 06";
            //int addr = index;
            var addr = address.ToString("X4");
            string tail = value.ToString("X4");
            cmdQueue.Enqueue(head + addr + tail);
            return 0;
        }

        /// <summary>
        /// Singleton
        /// </summary>
        public static DeltaPLCModbus Instance
        {
            get
            {
                lock (obj)
                {
                    if (instance == null)
                    {
                        instance = new DeltaPLCModbus();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Private
        /// </summary>

        /// Private constructor
        private DeltaPLCModbus()
        {

        }

        private static void PollDelta(int pollingInterval, string ipAddress, int port)
        {
            while (poll)
            {
                while (!PingDelta(ipAddress, port))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                int p6 = pollingInterval /6;
                tcpClient = new TcpClient();

                try
                {
                    tcpClient.Connect(ipAddress, port);
                    while (poll)
                    {
                        plcStatus = 0;

                        SendCommand(cmdQueue);
                        Pause(p6);
                        PollOutputs();
                        Pause(p6);

                        SendCommand(cmdQueue);
                        Pause(p6);
                        PollInputs();
                        Pause(p6);

                        SendCommand(cmdQueue);
                        Pause(p6);
                        PollRegisters();
                        Pause(p6);
                        //Pause(pollingInterval);
                    }
                }
                catch (Exception e)
                {
                    plcStatus = -1;
                    lastError = "PollDelta error: " + e.Message;
                    tcpClient?.Close();
                    tcpClient?.Dispose();
                    tcpClient = null;
                }
            }
            tcpClient?.Close();
            tcpClient?.Dispose();
            tcpClient = null;
        }







        private static void PollOutputs()
        {
            string hexCmd = "00 01 00 00 00 06 00 01 05_00 00_50";      // Read from address 0x500 64(0x50) outputs
                                                                        //00 01 00 00 00 0B 00 01 08 00_00_00_01_00_00_00_00
            var rcv = SendHexStringGetBytes(tcpClient, hexCmd);
            PlcOutputs = new byte[rcv.Length - modbusHeaderLength];
            Array.Copy(rcv, modbusHeaderLength, PlcOutputs, 0, rcv.Length - modbusHeaderLength);
        }

        private static void PollInputs()
        {
            string hexCmd = "00 02 00 00 00 06 00 02 04_00 00_50";       // Read from address 0x400 64(0x50) inputs
            var rcv = SendHexStringGetBytes(tcpClient, hexCmd);
            PlcInputs = new byte[rcv.Length - modbusHeaderLength];
            Array.Copy(rcv, modbusHeaderLength, PlcInputs, 0, rcv.Length - modbusHeaderLength);
        }

        private static void PollRegisters()
        {
            string hexCmd = "00 03 00 00 00 06 00 03 00_64 00_14";         // Read from address 0x64 20(0x14) registers
            var rcv = SendHexStringGetBytes(tcpClient, hexCmd);
            PlcRegisters = new byte[rcv.Length - modbusHeaderLength];
            Array.Copy(rcv, modbusHeaderLength, PlcRegisters, 0, rcv.Length - modbusHeaderLength);
        }

        private static void SendCommand(ConcurrentQueue<string> cmd)
        {
            string cmdToSend;
            if (cmdQueue.TryDequeue(out cmdToSend)) SendHexString(tcpClient, cmdToSend);
        }
        private static void Pause(int pause)
        {
            System.Threading.Thread.Sleep(pause);
        }

        private static byte[] SendHexStringGetBytes(TcpClient client, string command)
        {
            byte[] res = new byte[0];
            if (client != null && client.Connected)
            {
                Stream stm = client.GetStream();
                byte[] buf100 = new byte[100];
                byte[] cmd = HexStringToByteArray(command);
                stm.Write(cmd, 0, cmd.Length);
                int k = stm.Read(buf100, 0, 100);
                res = new byte[k];
                Array.Copy(buf100, res, k);
            }
            return res;
        }
        private static string SendHexString(TcpClient client, string command)
        {
            var bytes = SendHexStringGetBytes(client, command);
            string hex = BitConverter.ToString(bytes).Replace("-", " ");
            return hex;
        }
        private static byte[] HexStringToByteArray(string hex)
        {
            var h = hex.Replace(" ", "").Replace("_", "");
            //if (h.Length % 2 != 0) h = h.Substring(0, h.Length - 1);

            byte[] b;
            try
            {
                b = Enumerable.Range(0, h.Length)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(h.Substring(x, 2), 16))
                                 .ToArray();
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.ToString());
                b = new byte[0];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.ToString());
                b = new byte[0];
            }


            return b;
        }

        private bool IsBitSet(byte b, int bitIndex)
        {
            var res = (b & (1 << bitIndex - 1)) != 0;
            return res;
        }

        private static bool PingDelta(string ipAddress, int port)
        {
            System.Net.NetworkInformation.PingReply result;
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            result = ping.Send(ipAddress, port);
            return result.Status == System.Net.NetworkInformation.IPStatus.Success;
        }

        private static int OctalToDecimal(int octalNumber)
        {
            int decimalNumber = 0;
            int BASE = 1;
            int temp = octalNumber;
            while (temp > 0)
            {
                int lastDigit = temp % 10;
                temp /= 10;
                decimalNumber += lastDigit * BASE;
                BASE *= 8;
            }
            return decimalNumber;
        }
    }
}
