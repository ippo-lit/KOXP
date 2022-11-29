using System.Runtime.InteropServices;

namespace KOXP_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        #region "Win32Api"
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hpProcess, IntPtr lpAddress, int dwSize, int flAllocationType, int flProtect);

        [DllImport("kernel32")]
        public static extern IntPtr VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);
        #endregion


        #region
        public const int MEM_COMMIT = 0x1000;
        public const int MEM_RESERVE = 0x2000;
        public const int MEM_DECOMMIT = 0x4000;
        public const int MEM_RELEASE = 0x8000;

        public const int PAGE_EXECUTE_READWRITE = 0x40;
        public const int PAGE_READWRITE = 0x4;
        #endregion


        #region "Read Memory Functions"

        public Int32 Read4Byte(IntPtr Address)
        {
            return Read4Byte(GameProcessHandle, Address);
        }

        public Int32 Read4Byte(long Address)
        {
            return Read4Byte(new IntPtr(Address));
        }
        #endregion


        #region "Write Memory Functions"
        public void Write4Byte(IntPtr Handle, IntPtr Address, Int32 Value)
        {
            WriteProcessMemory(Handle, Address, BitConverter.GetBytes(Value), 4, 0);
        }

        public void Write4Byte(long Address, Int32 Value)
        {
            Write4Byte(GameProcessHandle, new IntPtr(Address), Value);
        }
        #endregion


        #region Helper

        public Int32 Read4Byte(IntPtr Handle, IntPtr Address)
        {
            byte[] Buffer = new byte[4];
            ReadProcessMemory(Handle, Address, Buffer, 4, 0);
            return BitConverter.ToInt32(Buffer, 0);
        }

        public Int32 Read4Byte(IntPtr Handle, long Address)
        {
            return Read4Byte(Handle, new IntPtr(Address));
        }
        #endregion


        public void ExecuteRemoteCode(String Code)
        {
            IntPtr CodePtr = VirtualAllocEx(GameProcessHandle, IntPtr.Zero, 1, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
            byte[] CodeByte = StringToByte(Code);
            WriteProcessMemory(GameProcessHandle, CodePtr, CodeByte, CodeByte.Length, 0);

            IntPtr Thread = CreateRemoteThread(GameProcessHandle, IntPtr.Zero, 0, CodePtr, IntPtr.Zero, 0, IntPtr.Zero);
            if (Thread != IntPtr.Zero)
                WaitForSingleObject(Thread, uint.MaxValue);
            CloseHandle(Thread);
            VirtualFreeEx(GameProcessHandle, CodePtr, 0, MEM_RELEASE);
        }


        public byte[] StringToByte(string text)
        {
            var tmpbyte = new byte[text.Length / 2];
            var count = 0;
            for (int i = 0; i < text.Length; i += 2)
            {
                var val = byte.MinValue;
                try
                {
                    if (text.Substring(i, 2) != "XX")
                        val = byte.Parse(text.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);

                    tmpbyte[count] = val;
                    count++;
                }
                catch (Exception)
                {
                }
            }
            return tmpbyte;
        }

        public String AlignDWORD(long Value)
        {
            String ADpStr, ADpStr2, ADresultStr;

            ADpStr = Convert.ToString(Value, 16);
            ADpStr2 = "";

            Int32 ADpStrLength = ADpStr.Length;

            int i = 0;
            for (i = 0; i < 8 - ADpStrLength; i++)
            {
                ADpStr2 = ADpStr2.Insert(i, "0");
            }

            int j = 0;
            int t = i;
            for (i = t; i < 8; i++)
            {
                ADpStr2 = ADpStr2.Insert(i, ADpStr[j].ToString());
                j++;
            }

            ADresultStr = "";

            ADresultStr = ADresultStr.Insert(0, ADpStr2[6].ToString());
            ADresultStr = ADresultStr.Insert(1, ADpStr2[7].ToString());
            ADresultStr = ADresultStr.Insert(2, ADpStr2[4].ToString());
            ADresultStr = ADresultStr.Insert(3, ADpStr2[5].ToString());
            ADresultStr = ADresultStr.Insert(4, ADpStr2[2].ToString());
            ADresultStr = ADresultStr.Insert(5, ADpStr2[3].ToString());
            ADresultStr = ADresultStr.Insert(6, ADpStr2[0].ToString());
            ADresultStr = ADresultStr.Insert(7, ADpStr2[1].ToString());

            return ADresultStr.ToUpper();
        }

        #region "Send Packet"

        public void SendPacket(byte[] Packet, int ExecutionAfterWait = 0)
        {
            IntPtr PacketPtr = VirtualAllocEx(GameProcessHandle, IntPtr.Zero, 1, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

            WriteProcessMemory(GameProcessHandle, PacketPtr, Packet, Packet.Length, 0);
            ExecuteRemoteCode("608B0D" + AlignDWORD(KO_PTR_PKT) + "68" + AlignDWORD(Packet.Length) + "68"/*PUSH*/ + AlignDWORD((long)PacketPtr) + "BF" + AlignDWORD(KO_PTR_SND) + "FFD7C605" + AlignDWORD(KO_PTR_PKT + 0xC5) + "0061C3");
            VirtualFreeEx(GameProcessHandle, PacketPtr, 0, MEM_RELEASE);

            if (ExecutionAfterWait > 0)
                Thread.Sleep(ExecutionAfterWait);
        }

        public void Packet(String Packet, int ExecutionAfterWait = 0)
        {
            SendPacket(StringToByte(Packet), ExecutionAfterWait);
        }

        #endregion


        #region Addresses

        public int KO_OFF_HP = 0x680;
        public int KO_OFF_MP = 0xB28;
        public int KO_OFF_WH = 0x684;

        public int KO_PTR_CHR = 0xF6D278;
        public int KO_PTR_DLG = 0xF6D2D8;
        public int KO_PTR_PKT = 0xF6D2B0;
        public int KO_PTR_SND = 0x5F16A0;
        #endregion

        public int GamePID;
        public IntPtr GameProcessHandle;
        public IntPtr GameWindowHandle;

        bool AttachProccess(string WindowsName)
        {
            string GameWindowName = WindowsName;

            if (GetHandle(txtWindowsName.Text))
                if (GetGamePID(GameWindowHandle))
                    if (GetGameProcessHandle(GamePID))
                        return true;
                    else
                        MessageBox.Show("Knight Online'i multi client olarak aciniz!");
                else
                    MessageBox.Show("Islem PID'si alinamadi. Islem Kodu hatasi.");
            else
                MessageBox.Show("Islem bulunamadi, once Knight Online'i acin.");

            return false;
        }

        bool GetHandle(string WindowName)
        {
            GameWindowHandle = FindWindow(null, WindowName);
            return (GameWindowHandle != IntPtr.Zero);
        }

        bool GetGamePID(IntPtr Handle)
        {
            GamePID = 0;
            GetWindowThreadProcessId(Handle, ref GamePID);
            return (GamePID != 0);
        }

        bool GetGameProcessHandle(int GamePID)
        {
            GameProcessHandle = OpenProcess(0x1F0FFF, false, GamePID);
            return (GameProcessHandle != IntPtr.Zero);
        }

        private void txtWindowsName_DropDown(object sender, EventArgs e)
        {
            txtWindowsName.Items.Clear();

            System.Diagnostics.Process[] Proccess = System.Diagnostics.Process.GetProcessesByName("KnightOnline");

            for (int i = 0; i < Proccess.Length; i++)
                txtWindowsName.Items.Add(Proccess[i].MainWindowTitle);
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            if (AttachProccess(txtWindowsName.Text))
                StartKoxp();
        }

        private void StartKoxp()
        {
            Console.WriteLine(GetCharHP());
            Console.WriteLine(GetCharMP());
            WallHack();
        }

        int GetCharHP()
        {
            return Read4Byte(Read4Byte(KO_PTR_CHR) + KO_OFF_HP);
        }

        int GetCharMP()
        {
            return Read4Byte(Read4Byte(KO_PTR_CHR) + KO_OFF_MP);
        }

        void WallHack()
        {
            Write4Byte(Read4Byte(KO_PTR_CHR) + KO_OFF_WH, 0);
        }

        private void btnTown_Click(object sender, EventArgs e)
        {
            Packet("4800");
        }
    }
}