using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;

public class TCPConnector
{
    #region Public Data
    private bool _isConnected = false;
    #endregion

    #region Private Data
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamWriter _streamWriter;
    private StreamReader _streamReader;
    private SocketController _socket;
    private bool _isServer = false;

    private string _masterClientResponse = string.Empty;

    private struct SocketInitializeParameters
    {
        public string ipAddress;
        public int port;
        public int maxClients;
    }
    #endregion

    #region Const Data
    private const string _socketResourcesPath = @"Resources/ConsoleApplication1/ConsoleApplication1/bin/Debug/ConsoleApplication1.exe";
    private const string _socketParametersFilePath = @"Resources/ConsoleApplication1/ConsoleApplication1/bin/Debug/SocketParameters.txt";
    #endregion

    //Tenta iniciar a conexão com o server
    public void SetupSocket(string p_ipAddress, int p_port, Action p_callbackSuccess, Action p_callbackFailed)
    {
        try
        {
            _tcpClient = new TcpClient(p_ipAddress, p_port);
            _networkStream = _tcpClient.GetStream();
            _streamWriter = new StreamWriter(_networkStream);
            _streamReader = new StreamReader(_networkStream);
            _isConnected = true;
            _isServer = false;
            if (p_callbackSuccess != null)
                p_callbackSuccess();
        }
        catch (SocketException p_socketException)
        {
            UnityEngine.Debug.Log("Socket error:" + p_socketException);
            if (p_callbackFailed != null)
                p_callbackFailed();
        }
    }
    // Inicializa o server e tenta criar uma conexão. (SÓ DEVE SER UTILIZADO PELO SERVER/CLIENT)
    public void OpenAndSetupSocket(string p_ipAddress, int p_port, int p_maxClients, Action p_callbackSuccess)
    {
        Action __callbackSetupSocket = delegate
        {
            //_tcpClient = new TcpClient(p_ipAddress, p_port);
            //_networkStream = _tcpClient.GetStream();
            //_streamWriter = new StreamWriter(_networkStream);
            //_streamReader = new StreamReader(_networkStream);
            _isConnected = true;
            _isServer = true;
            if (p_callbackSuccess != null) p_callbackSuccess();
        };

        OpenSocket(p_ipAddress, p_port, p_maxClients, __callbackSetupSocket, null);
    }

    private void OpenSocket(string p_ipAddress, int p_port, int p_maxClients, Action p_callbackFinish, Action p_callbackFailed = null)
    {
        _socket = new SocketController();
        _socket.Initialize(CreateSocketInitializeParameters(p_ipAddress, p_port, p_maxClients));
        _socket.onSocketResponseToMasterClient += delegate (string p_response)
        {
            _masterClientResponse = p_response;
        }; 
    }

    private SocketInitializationData CreateSocketInitializeParameters(string p_ipAddress, int p_port, int p_maxClients)
    {
        SocketInitializationData __socketInitializeParameters = new SocketInitializationData();
        __socketInitializeParameters.ipAddress = p_ipAddress;
        __socketInitializeParameters.port = p_port;
        __socketInitializeParameters.maxClients = p_maxClients;
        return __socketInitializeParameters;
    }


    //Envia a mensagem para o socket.
    public void SendData(string p_string)
    {
        if (!_isConnected)
            return;

        if (_isServer == true)
        {
            _socket.SetMasterClientStream(p_string);
            return;
        }

        string __tempString = p_string;// +"\r\n";
        _streamWriter.Write(__tempString);
        _streamWriter.Flush();
        //_streamWriter.Dispose();
        //Send(_tcpClient, __tempString);
    }


    //Lê a mensagem recebida pelo servidor.
    public string ReceiveData()
    {
        string __response = string.Empty;
        try
        {
            if (_isServer == true)
            {
                if (_masterClientResponse != string.Empty)
                {
                    __response = _masterClientResponse;
                }
            }
            else if (_networkStream.DataAvailable == true)
            {
                byte[] __dataToRead = new Byte[_tcpClient.SendBufferSize];
                _networkStream.Read(__dataToRead, 0, __dataToRead.Length);

                __response = System.Text.Encoding.UTF8.GetString(__dataToRead);
            }
        }
        catch (SocketException p_socketException)
        {
            UnityEngine.Debug.LogError("Socket error:" + p_socketException);
        }
        return __response;
    }

    //Fecha a conexão com o socket
    public void CloseSocket()
    {
        if (_isConnected == false)
            return;
        _streamWriter.Close();
        _streamReader.Close();
        _tcpClient.Close();
        _isConnected = false;
    }

    //Reconecta ao socket
    /* public void ReconectToSocket()
     {
         if (_networkStream.CanRead == false)
         {
             SetupSocket(_hostIpAddress, _port);
         }
     }*/

    public bool GetIsConnected()
    {
        return _isConnected;
    }
}