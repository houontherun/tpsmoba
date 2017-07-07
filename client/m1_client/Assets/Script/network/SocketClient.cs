using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;

public class SocketClient
{
    private TcpClient client = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int messCountBytes = 4; // 消息长度字节数

    private const int MAX_READ = 60000;
    private byte[] byteBuffer = new byte[MAX_READ];
    
    private int timeoutMSec = 5000;
    private ManualResetEvent TimeoutObject = null;

    public string errorMsg { get; private set; }
    public string failedMsg { get; private set; }
    
    private Connection connection;
    // Use this for initialization
    public SocketClient(Connection con)
    {
        connection = con;
    }
    public void ConnectServer(string host, int port, int timeout)
    {
        Clear();
        TimeoutObject = new ManualResetEvent(false);
        TimeoutObject.Reset();
        timeoutMSec = timeout;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        Thread th = null;
        Debug.Log(string.Format("begin connect ip={0} port={1}", host, port));
        try
        {
            IAsyncResult ret = client.BeginConnect(host, port, new AsyncCallback(OnConnect), client);
            th = new Thread(new ThreadStart(StartWaitOne));
            th.Start();
        }
        catch (Exception ex)
        {
            Debug.Log("connect failed! msg=" + ex.Message);
            failedMsg = ex.Message;
            connection.state = ConnectState.Failed;
            TimeoutObject.Close();
            if (th != null)
            {
                th.Abort();
            }
        }
    }
    void StartWaitOne()
    {
        if (TimeoutObject.WaitOne(timeoutMSec, false)) // 当调用TimeoutObject.Set()时执行if语句, 当超时时, 执行else语句
        {
            //if (state == ConnectState.Success)
            //{
            //    Util.Log("Game", string.Format("connect ok"));
            //}
            //else
            //{
            //    Util.Log("Game", string.Format("connect failed"));
            //    state = ConnectState.Failed;
            //    Close();
            //}
        }
        else
        {
            if (connection.state != ConnectState.Success)
            {
                Debug.Log(string.Format("连接超时"));
                if (string.IsNullOrEmpty(failedMsg))
                {
                    failedMsg = "connect timeout";
                }
                client.Close();
                connection.state = ConnectState.Failed;
            }
        }
    }
    void OnConnect(IAsyncResult asr)
    {
        try
        {
            client.EndConnect(asr);
        }
        catch (Exception ex)
        {
            Debug.Log("connect failed! msg=" + ex.Message);
            failedMsg = ex.Message;
            connection.state = ConnectState.Failed;
            TimeoutObject.Set();
            return;
        }
        if (client.Connected)
        {
            Debug.Log("connect succeed!");
            connection.state = ConnectState.Success;
            TimeoutObject.Set();
            
            memStream = new MemoryStream();
            reader = new BinaryReader(memStream);
            ReadMessage();
        }
        else
        {
            Debug.Log("connect failed!");
            failedMsg = "unknown reason";
            connection.state = ConnectState.Failed;
            TimeoutObject.Set();
        }
    }

    void ReadMessage()
    {
        if (client.GetStream().CanRead)
        {
            Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
            try
            {
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
            catch (Exception ex)
            {
                errorMsg = string.Format("BeginRead error! msg={0}", ex.Message);
                connection.state = ConnectState.Error;
                Debug.Log(errorMsg);
                return;
            }
        }
    }
    void OnRead(IAsyncResult asr)
    {
        if (connection.state != ConnectState.Success)
        {
           Debug.Log( string.Format("OnRead failed! state={0}", connection.state));
            return;
        }
        if (!client.Connected)
        {
            Debug.Log(string.Format("OnRead failed! client.Connected is false"));
            return;
        }
        int bytesRead = 0;
        try
        {
            bytesRead = client.GetStream().EndRead(asr);
        }
        catch (Exception ex)
        {
            errorMsg = string.Format("EndRead error! msg={0}", ex.Message);
            connection.state = ConnectState.Error;
            Debug.Log(errorMsg);
            return;
        }
        if (bytesRead < 1)
        {
            errorMsg = string.Format("SERVER_CLOSE");
            connection.state = ConnectState.Error;
            Debug.Log(errorMsg);
            return;
        }
        OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
        ReadMessage();
    }
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > messCountBytes)
        {
            //int messageLen = reader.ReadInt16(); // 2字节
            int messageLen = reader.ReadInt32(); // 4字节
            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnMessage(ms);
            }
            else
            {
                //Back up the position
                memStream.Position = memStream.Position - messCountBytes;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }
    void OnMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

        // 避开流水号
        int index = 0;
        UInt16 serialNum = BitConverter.ToUInt16(message, index);
        index += 2;
        UInt16 serviceType = BitConverter.ToUInt16(message, index);
        index += 2;
        UInt16 action = BitConverter.ToUInt16(message, index);
        index += 2;
        MemoryStream stream = new MemoryStream(message, index, message.Length - index);

       
    }
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    
    string getBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        return returnStr;
    }
    
    void OnWrite(IAsyncResult r)
    {
        if (connection.state != ConnectState.Success)
        {
            Debug.Log(string.Format("OnWrite failed! state={0}", connection.state));
            return;
        }
        if (!client.Connected)
        {
            Debug.Log(string.Format("OnWrite failed! client.Connected is false"));
            return;
        }
        try
        {
            client.GetStream().EndWrite(r);
        }
        catch (Exception ex)
        {
            errorMsg = string.Format("EndWrite error! msg={0}", ex.Message);
            connection.state = ConnectState.Error;
            Debug.Log(errorMsg);
            return;
        }
    }
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(message);
            writer.Flush();
            byte[] payload = ms.ToArray();
            try
            {
                client.GetStream().BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            catch(Exception ex)
            {
                errorMsg = string.Format("BeginWrite error! msg={0}", ex.Message);
                connection.state = ConnectState.Error;
                Debug.Log(errorMsg);
                return;
            }
        }
    }

    public void SendMessage(int action, byte[] data, int serviceType = 0)
    {
        if(connection.state != ConnectState.Success)
        {
            Debug.Log(string.Format("SendMessage failed. state={0}", connection.state));
            return;
        }
        Int32 dataSize = (Int32)(2 + 2 + 2);
        if (data != null)
        {
            dataSize += (Int32)data.Length; // 总长度
        }

        //Int16 dataSize = (Int16)(2 + 2 + 2);
        //if (data != null)
        //{
        //    dataSize += (Int16)data.Length; // 总长度
        //}

        byte[] buf = new byte[dataSize + messCountBytes];

        int index = 0;
        byte[] buf_data_size = BitConverter.GetBytes(dataSize); // 包长度，分包用
        Array.Copy(buf_data_size, 0, buf, index, messCountBytes);
        index += messCountBytes;

        byte[] buf_serial_num = BitConverter.GetBytes(Helper.tPackageSerialNumber++);
        Array.Copy(buf_serial_num, 0, buf, index, 2);
        index += 2;

        byte[] buf_service_type = BitConverter.GetBytes((UInt16)serviceType);
        Array.Copy(buf_service_type, 0, buf, index, 2);
        index += 2;

        byte[] buf_action = BitConverter.GetBytes((UInt16)action);
        Array.Copy(buf_action, 0, buf, index, 2);
        index += 2;

        if (data != null)
        {
            Array.Copy(data, 0, buf, index, data.Length);
        }
        WriteMessage(encodeWP(buf));
    }

    private void Clear()
    {
        if (client != null)
        {
            if(client.Client!= null && client.Connected)
            {
                client.GetStream().Close();
            }
            client.Close();
        }
        client = null;
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
        if (memStream != null)
        {
            memStream.Close();
            memStream = null;
        }
        if(TimeoutObject != null)
        {
            TimeoutObject.Close();
            TimeoutObject = null;
        }
        
        failedMsg = "";
        errorMsg = "";
    }
    public void Close()
    {
        if (client == null || connection.state == ConnectState.Close)
        {
            return;
        }
        Clear();

        Debug.Log(string.Format("socket client close"));
        connection.state = ConnectState.Close;
    }
    
    private byte[] encodeWP(byte[] buffer)
    {
        const int offset = messCountBytes;
        for (int i = offset; i <= buffer.Length - 2; ++i)
        {
            buffer[i] ^= buffer[i + 1];
        }
        buffer[buffer.Length - 1] ^= 0x3A;
        return buffer;
    }

    // gzip加密
    private static byte[] compress(byte[] buffer)
    {
        MemoryStream ms = new MemoryStream();
        GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
        zip.Write(buffer, 0, buffer.Length);
        zip.Close();
        ms.Position = 0;

        byte[] compressed = new byte[ms.Length];
        ms.Read(compressed, 0, compressed.Length);

        byte[] gzBuffer = new byte[compressed.Length + 4];
        Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
        return gzBuffer;
    }
}
