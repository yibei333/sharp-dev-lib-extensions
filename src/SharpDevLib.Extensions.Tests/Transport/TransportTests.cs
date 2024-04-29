using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDevLib.Extensions.Transport;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SharpDevLib.Extensions.Tests.Transport;

[TestClass]
public class TransportTests
{
    private readonly ITcpFactory _tcpFactory;
    private readonly IUdpFactory _udpFactory;
    private readonly Guid _id;

    public TransportTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSocket();
        var serviceProvider = services.BuildServiceProvider();
        _tcpFactory = serviceProvider.GetRequiredService<ITcpFactory>();
        _udpFactory = serviceProvider.GetRequiredService<IUdpFactory>();
        _id = Guid.NewGuid();
    }

    [TestMethod]
    public void UdpTest()
    {
        IPEndPoint clientEndPoint = null;
        var server = _udpFactory.Create(new UdpClientOptions(new IPEndPoint(IPAddress.Any, 1000)));
        server.OnError += (sender, arg) => WriteLog($"server OnError", arg);
        server.OnSended += (sender, arg) => WriteLog($"server OnSended", arg);
        server.OnSendFailed += (sender, arg) => WriteLog($"server OnSendFailed", arg);
        server.OnReceive += (sender, arg) =>
        {
            clientEndPoint = arg.RemoteEndPoint as IPEndPoint;
            WriteLog($"server OnReceive", arg);
        };
        server.BeginReceive();

        var client = _udpFactory.Create(new UdpClientOptions());
        client.OnError += (sender, arg) => WriteLog($"client OnError", arg);
        client.OnReceive += (sender, arg) => WriteLog($"client OnReceive", arg);
        client.OnSendFailed += (sender, arg) => WriteLog($"client OnSendFailed", arg);
        client.OnSended += (sender, arg) => WriteLog($"client OnSended", arg);
        client.BeginReceive();

        client.Send(new IPEndPoint(IPAddress.Loopback, 1000), new byte[4097]);
        for (int i = 0; i < 5; i++)
        {
            client.Send(new IPEndPoint(IPAddress.Loopback, 1000), Encoding.UTF8.GetBytes($"client send message:{i}"));
            Thread.Sleep(1000);
        }

        for (int i = 10; i < 15; i++)
        {
            if (clientEndPoint.IsNull()) continue;
            server.Send(clientEndPoint!, Encoding.UTF8.GetBytes($"server send message:{i}"));
            Thread.Sleep(1000);
        }

        Thread.Sleep(1000);
        client.Dispose();
        server.Dispose();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(4097)]
    [DataRow(10000)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void UdpExceptionTest1(int bufferSize)
    {
        _ = _udpFactory.Create(new UdpClientOptions(bufferSize));
    }

    [TestMethod]
    public void TcpTest()
    {
        var listener = _tcpFactory.Create(new TcpListenerOptions(new IPEndPoint(IPAddress.Loopback, 2000), 4));
        listener.OnListening += (sender, e) => WriteLog("server OnListening", e);
        listener.OnAccept += (sender, e) => WriteLog("server OnAccept", e);
        listener.OnSendFailed += (sender, e) => WriteLog("server OnSendFailed", e);
        listener.OnSended += (sender, e) => WriteLog("server OnSended", e);
        listener.OnReceive += (sender, e) => WriteLog("server OnReceive", e);
        listener.OnDisConnected += (sender, e) => WriteLog("server OnDisConnected", e);
        listener.OnError += (sender, e) => WriteLog("server OnError", e);
        listener.BeginLinsten();

        var client = _tcpFactory.Create(new TcpClientOptions(new IPEndPoint(IPAddress.Loopback, 2000), 4));
        client.OnError += (sender, e) => WriteLog("client OnError", e);
        client.OnConnected += (sender, e) => WriteLog("client OnConnected", e);
        client.OnSended += (sender, e) => WriteLog("client OnSended", e);
        client.OnSendFailed += (sender, e) => WriteLog("client OnSendFailed", e);
        client.OnDisConnected += (sender, e) => WriteLog("client OnDisConnected", e);
        client.OnReceive += (sender, e) => WriteLog("client OnReceive", e);
        client.BeginConnect();

        for (int i = 0; i < 5; i++)
        {
            client.Send(Encoding.UTF8.GetBytes($"client send message:{i}"));
            Thread.Sleep(1000);
        }

        listener.SendTo(listener.Connections[0].Id, Encoding.UTF8.GetBytes("server send"));
        listener.SendToAll(Encoding.UTF8.GetBytes("server broadcast"));

        Thread.Sleep(1000);
        client.Close();
        listener.Close();

        listener.SendTo(listener.Connections[0].Id, Encoding.UTF8.GetBytes("server send1"));
        listener.SendToAll(Encoding.UTF8.GetBytes("server broadcast1"));
        client.Send(Encoding.UTF8.GetBytes($"client send message xyz"));

        Thread.Sleep(1000);
    }

    private void WriteLog(string prefixMessage, SocketArgs arg)
    {
        var builder = new StringBuilder();
        builder.Append($"id:{_id},{prefixMessage}");
        if (arg.LocalEndPoint.NotNull()) builder.Append($",local:{arg.LocalEndPoint}");
        if (arg.RemoteEndPoint.NotNull()) builder.Append($",remote:{arg.RemoteEndPoint}");
        if (arg.Data.NotNull()) builder.Append($",data:{Encoding.UTF8.GetString(arg.Data!)}");
        if (arg.ErrorMessage.NotNull()) builder.Append($",errorMesssage:[{arg.ErrorType}],{arg.ErrorMessage}");
        //if (arg.Error.NotNull()) builder.Append($",error:{arg.Error!.StackTrace}");
        Debug.WriteLine(builder.ToString());
        Debug.WriteLine($"---------------------------------");
    }

    [TestMethod]
    [DataRow(SocketProtocol.UDP, 10, 20)]
    [DataRow(SocketProtocol.UDP, 10000, 11000)]
    [DataRow(SocketProtocol.TCP, 10, 20)]
    [DataRow(SocketProtocol.TCP, 10000, 11000)]
    public void GetUsablePortTest(SocketProtocol type, int min, int max)
    {
        if (type == SocketProtocol.UDP)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, min));
        }
        else
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, min));
            socket.Listen();
        }

        var port = type.GetUsablePort(min, max);
        Console.WriteLine(port);
        Assert.IsTrue(port > min && port <= max);
    }

    [TestMethod]
    [DataRow(2, 1)]
    [DataRow(-1, 1)]
    [DataRow(67000, 1)]
    [DataRow(1, -1)]
    [DataRow(1, 67000)]
    [DataRow(-1, -1)]
    [DataRow(-1, 67000)]
    [ExpectedException(typeof(InvalidDataException))]
    public void GetUsablePortExceptionTest1(int min, int max)
    {
        SocketProtocol.TCP.GetUsablePort(min, max);
    }

    [TestMethod]
    [DataRow(SocketProtocol.UDP, 3000)]
    [DataRow(SocketProtocol.TCP, 3000)]
    [ExpectedException(typeof(Exception))]
    public void GetUsablePortExceptionTest2(SocketProtocol type, int port)
    {
        if (type == SocketProtocol.UDP)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }
        else
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen();
        }
        type.GetUsablePort(port, port);
    }
}
