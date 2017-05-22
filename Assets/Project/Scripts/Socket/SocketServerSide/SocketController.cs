using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SimpleJSON;

public class SocketInitializationData
{
    public string ipAddress;
    public int port;
    public int maxClients;
}

public class SocketController
{
    #region Event Data
    public event Action<string> onSocketResponseToMasterClient;
    #endregion

    #region Public Data
    public List<ClientData> listClients;                   // Lista pública com os dados dos clientes
    // É usada para armazenar a última resposta do
    // cliente para com o socket e também a próxima
    // resposta do socket para o cliente.
    #endregion

    #region Private Data
    private TcpListener _tcpListener;                       // Escuta as conexões tcps dos clientes

    private Thread _threadClientAcception;                  // Rêferencia a thread da aceitação de clientes

    private List<Thread> _listClientWaitForResponseThreads; // Lista com a referência as thread de espera de resposta dos clients

    private IPAddress _ipAdress;                            // Ip Address do socket

    private ClientData _masterClient;

    private int _maxClients = 0;                            // Número máximo de clientes
    private int _clientResponseCounter;                     // Contador de quantos clientes já responderam

    private bool _isServerRunning = false;                  // Se o socket está executando
    private bool _isAcceptingNewClients = true;             // Se o socket está aceitando conexões
    private bool _isWaitingStreamInLoop = false;            // Se o socket está aceitando respostas dos clientes sem pausa

    private byte[] _bytes;
    #endregion

    public struct ClientData
    {
        public TcpClient tcpClient;                     // Conexão do cliente do tipo TCP
        public bool isServer;                           // If client is server
        public string id;                               // Unique id do cliente
        public string clientToGetResponse;              // Dados enviados do client para o socket
        public string clientToSendResponse;             // Dados enviados do socket para o client
    }

    #region Initialization
    //Inicializa o socket recebendo os parametros "SocketInitializationData", deve ser chamado antes de qualquer outro método
    public void Initialize(SocketInitializationData p_socketData)
    {
        _ipAdress = IPAddress.Parse(p_socketData.ipAddress);
        _tcpListener = new TcpListener(_ipAdress, p_socketData.port);
        listClients = new List<ClientData>();
        _listClientWaitForResponseThreads = new List<Thread>();
        _bytes = new Byte[1024];
        _maxClients = p_socketData.maxClients;
        CreateClientServer();
    }
    #endregion

    private void CreateClientServer()
    {
        _masterClient = new ClientData();
        _masterClient.isServer = true;
        _masterClient.id = Guid.NewGuid().ToString();
        listClients.Add(_masterClient);
    }

    //Inicializa o socket, recebe um callback de sucesso e um callback de falha caso acontece algum erro na inicialização.
    public void StartSocket(Action p_callbackSuccess, Action p_callbackFailed)
    {
        try
        {
            if (_isServerRunning == true)
            {
                StopSocket();
            }
            _isServerRunning = true;
            _isAcceptingNewClients = true;
            _tcpListener.Start();

            if (p_callbackSuccess != null) p_callbackSuccess();
        }
        catch (SocketException p_socketException)
        {
            Console.WriteLine("SocketException: {0}", p_socketException);
            if (p_callbackFailed != null)
                p_callbackFailed();
        }
    }

    //Para o socket, o mesmo não irá mais receber dados e irá perder a referência aos seus Clients
    public void StopSocket()
    {
        Console.WriteLine("Socket Stopped");
        _isServerRunning = false;
        listClients.Clear();
        _tcpListener.Stop();
    }

    // Começa a aceitar conexões via thread do cliente para com o Socket.
    // Quando o número de clientes conectador for maior que o máximo, retornará um callback de finish
    public void StartAcceptTcpClientThread(Action p_callbackFinish)
    {
        if (_isAcceptingNewClients == false)
            return;
        Console.WriteLine("Waiting all {0} clients to connect...", _maxClients);
        _threadClientAcception = new Thread(new ParameterizedThreadStart(AcceptTcpClientThread));
        _threadClientAcception.Start(p_callbackFinish);
    }

    // Lógia da aceitação do cliente
    private void AcceptTcpClientThread(object p_callbackFinish)
    {
        while (_isAcceptingNewClients == true)
        {
            if (listClients.Count >= _maxClients)
            {
                Console.WriteLine("All clients connected, starting game...");
                _isAcceptingNewClients = false;

                if ((Action)p_callbackFinish != null)
                    ((Action)p_callbackFinish)();
            }
            else
            {
                ClientData __clientData = new ClientData();
                __clientData.tcpClient = _tcpListener.AcceptTcpClient();
                __clientData.id = Guid.NewGuid().ToString();
                __clientData.isServer = false;

                listClients.Add(__clientData);
                Console.WriteLine("New Client connected, {0} of {1}\n", listClients.Count, _maxClients);
            }
        }
    }

    // Inicia a espera para receber os dados do cliente em loop via thread, não retorna um callback de finish
    public void StartWaitAllClientsStreamThread()
    {
        _isWaitingStreamInLoop = true;
        for (int i = 0; i < listClients.Count;i++)
        {
            if (listClients[i].isServer == false)
            {
                int __index = i;
                ThreadStart __callbackThreadStart = delegate
                {
                    WaitClientStreamThread(listClients[__index]);
                };
                Thread __newThread = new Thread(__callbackThreadStart);
                __newThread.Start();
                _listClientWaitForResponseThreads.Add(__newThread);            
            }
        }
    }

    public void SetMasterClientStream(string p_responseStream)
    {
        _masterClient.clientToGetResponse = p_responseStream;
        UnityEngine.Debug.LogFormat("Received data from Client [{0}]: \n{1}", _masterClient.id, p_responseStream);
        listClients[listClients.FindIndex(x => x.id == _masterClient.id)] = _masterClient;
    }

    public void StopWaitAllClientsStreamThread()
    {
        _isWaitingStreamInLoop = false;
        _listClientWaitForResponseThreads.Clear();
    }

    //Lógica de espera dos dados do cliente
    private void WaitClientStreamThread(ClientData p_clientData)
    {
        while (_isWaitingStreamInLoop && _isServerRunning)
        {
            NetworkStream __networkStream = p_clientData.tcpClient.GetStream();

            int __readCount = __networkStream.Read(_bytes, 0, _bytes.Length);
            if (__readCount != 0)
            {
                string __response = Encoding.ASCII.GetString(_bytes, 0, __readCount);

                Console.WriteLine("Received data from Client [{0}]: \n{1}", p_clientData.id, __response);
                UnityEngine.Debug.LogFormat("Received data from Client [{0}]: \n{1}", p_clientData.id, __response);
                p_clientData.clientToGetResponse = __response;

                listClients[listClients.FindIndex(x => x.id == p_clientData.id)] = p_clientData;

                __networkStream.Flush();
            }        
        }
    }

    // Inicia a espera para receber os dados do cliente via thread e retorná um callback de finish
    // quando todos os clientes responderem.
    public void StartWaitAllClientsStreamThenStopThread(Action p_callbackFinish)
    {
        if (_isWaitingStreamInLoop == true)
        {
            Console.WriteLine("You cannot use both methods.");
            return;
        }
        _clientResponseCounter = 0;
        for (int i = 0;i < listClients.Count;i++)
        {
            if (listClients[i].isServer == false)
            {
                int __index = i;
                ThreadStart __callbackThreadStart = delegate
                {
                    WaitClientStreamThenStopThread(listClients[__index], p_callbackFinish);
                };
                Thread __newThread = new Thread(__callbackThreadStart);
                __newThread.Start();
                _listClientWaitForResponseThreads.Add(__newThread);
            }
        }
    }

    // Lógica da espera da resposta do cliente, verifica se todos os clientes ja responderam. Se sim, retorna o callbackFinish
    private void WaitClientStreamThenStopThread(ClientData p_clientData, Action p_callbackFinish)
    {
        NetworkStream __networkStream = p_clientData.tcpClient.GetStream();
       
        int __readCount = __networkStream.Read(_bytes, 0, _bytes.Length);
        if (__readCount != 0)
        {
            string __response = Encoding.ASCII.GetString(_bytes, 0, __readCount);

            Console.WriteLine("Received data from Client [{0}]: \n{1}", p_clientData.id, __response);

            p_clientData.clientToGetResponse = __response;

            listClients[listClients.FindIndex(x => x.id == p_clientData.id)] = p_clientData;

            __networkStream.Flush();

            _clientResponseCounter++;

            if (_clientResponseCounter >= _maxClients)
            {
                if (p_callbackFinish != null) p_callbackFinish();
            }
        }
    }

    // Envia dados do socket para o cliente. O dado enviado precisa ser setado
    // na váriavel clientToSendResponse da struct ClientData
    public void StreamToClients(Action p_callbackFinish)
    {
        for (int i = 0;i < listClients.Count;i++)
        {
            Console.WriteLine("Sending data to Client [{0}]: \n{1}", listClients[i].id, listClients[i].clientToSendResponse);
            if (listClients[i].isServer == true)
            {
                if (onSocketResponseToMasterClient != null)
                    onSocketResponseToMasterClient(listClients[i].clientToSendResponse);
                continue;
            }
            string __response = listClients[i].clientToSendResponse;
            byte[] __dataToSend = Encoding.ASCII.GetBytes(__response);

            NetworkStream __networkStream = listClients[i].tcpClient.GetStream();
            __networkStream.Write(__dataToSend, 0, __dataToSend.Length);
            __networkStream.Flush();
        }
    }
}