using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace samuilx86ez
{
    public class sam86
    {
        #region Import

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        #endregion

        #region Globals

        private IntPtr processHandle;
        public IntPtr BaseAddress { get; private set; }

        #endregion

        #region Constructor

        public sam86(string processName)
        {
            Process proc = Process.GetProcessesByName(processName)[0];
            processHandle = OpenProcess(0x1F0FFF, false, proc.Id);
            BaseAddress = proc.MainModule.BaseAddress;
        }

        #endregion

        #region Memory Read

        public IntPtr ReadPointer(IntPtr address)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(processHandle, address, buffer, buffer.Length, IntPtr.Zero);
            return (IntPtr)BitConverter.ToInt64(buffer, 0);
        }

        public IntPtr ReadPointer(IntPtr address, int offset)
        {
            return ReadPointer(address + offset);
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            byte[] buffer = new byte[size];
            ReadProcessMemory(processHandle, address, buffer, size, IntPtr.Zero);
            return buffer;
        }

        public short ReadShort(IntPtr address, int offset = 0) => BitConverter.ToInt16(ReadBytes(address + offset, 2), 0);
        public ushort ReadUShort(IntPtr address, int offset = 0) => BitConverter.ToUInt16(ReadBytes(address + offset, 2), 0);
        public int ReadInt(IntPtr address, int offset = 0) => BitConverter.ToInt32(ReadBytes(address + offset, 4), 0);
        public uint ReadUInt(IntPtr address, int offset = 0) => BitConverter.ToUInt32(ReadBytes(address + offset, 4), 0);
        public long ReadLong(IntPtr address, int offset = 0) => BitConverter.ToInt64(ReadBytes(address + offset, 8), 0);
        public ulong ReadULong(IntPtr address, int offset = 0) => BitConverter.ToUInt64(ReadBytes(address + offset, 8), 0);
        public float ReadFloat(IntPtr address, int offset = 0) => BitConverter.ToSingle(ReadBytes(address + offset, 4), 0);
        public double ReadDouble(IntPtr address, int offset = 0) => BitConverter.ToDouble(ReadBytes(address + offset, 8), 0);
        public bool ReadBool(IntPtr address, int offset = 0) => BitConverter.ToBoolean(ReadBytes(address + offset, 1), 0);
        public char ReadChar(IntPtr address, int offset = 0) => BitConverter.ToChar(ReadBytes(address + offset, 2), 0);
        public string ReadString(IntPtr address, int length, int offset = 0) => Encoding.UTF8.GetString(ReadBytes(address + offset, length));

        public Vector3 ReadVec(IntPtr address, int offset = 0)
        {
            byte[] bytes = ReadBytes(address + offset, 12);
            return new Vector3(
                BitConverter.ToSingle(bytes, 0),
                BitConverter.ToSingle(bytes, 4),
                BitConverter.ToSingle(bytes, 8)
            );
        }

        public float[] ReadMatrix(IntPtr address)
        {
            byte[] bytes = ReadBytes(address, 64);
            float[] matrix = new float[16];
            for (int i = 0; i < 16; i++)
                matrix[i] = BitConverter.ToSingle(bytes, i * 4);
            return matrix;
        }

        #endregion

        #region Memory Write

        public bool WriteBytes(IntPtr address, byte[] data, int offset = 0)
        {
            return WriteProcessMemory(processHandle, address + offset, data, data.Length, IntPtr.Zero);
        }

        public bool WriteShort(IntPtr address, short value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteUShort(IntPtr address, ushort value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteInt(IntPtr address, int value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteUInt(IntPtr address, uint value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteLong(IntPtr address, long value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteULong(IntPtr address, ulong value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteFloat(IntPtr address, float value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteDouble(IntPtr address, double value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteBool(IntPtr address, bool value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteChar(IntPtr address, char value, int offset = 0) => WriteBytes(address, BitConverter.GetBytes(value), offset);
        public bool WriteString(IntPtr address, string value, int offset = 0) => WriteBytes(address, Encoding.UTF8.GetBytes(value), offset);

        public bool WriteVec(IntPtr address, Vector3 vec, int offset = 0)
        {
            byte[] bytes = new byte[12];
            BitConverter.GetBytes(vec.X).CopyTo(bytes, 0);
            BitConverter.GetBytes(vec.Y).CopyTo(bytes, 4);
            BitConverter.GetBytes(vec.Z).CopyTo(bytes, 8);
            return WriteBytes(address, bytes, offset);
        }

        #endregion
    }
}
