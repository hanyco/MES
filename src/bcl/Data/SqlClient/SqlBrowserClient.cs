using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using Library.Validations;

namespace Library.Data.SqlClient;

public static class SqlBrowserClient
{
    private const int SQL_BROWSER_PORT = 1434;

    private static readonly byte[] _getInstancesMessage = new byte[1] { 2 };
    private static IEnumerable<SqlInstance>? _instances;

    public static IEnumerable<SqlInstance> Instances
    {
        get
        {
            if (_instances is null)
            {
                InitInstances();
            }

            return _instances.NotNull();
        }
    }

    private static void InitInstances()
    {
        using var client = new UdpBroadcastMessage(SQL_BROWSER_PORT, _getInstancesMessage, new TimeSpan(0, 0, 5));

        var responses = client.GetResponse();
        _instances = ParseBrowserResponses(responses);
    }

    private static SqlInstance MapFromMatch(Match match)
        => new(match.Groups["ServerName"].Captures[0].Value,
               match.Groups["InstanceName"].Captures[0].Value,
               match.Groups["IsClustered"].Captures[0].Value != "No",
               match.Groups["Version"].Captures[0].Value,
               match.Groups["tcp"].Captures.Count > 0 ? int.Parse(match.Groups["tcp"].Captures[0].Value) : 0,
               match.Groups["np"].Captures.Count > 0 ? match.Groups["np"].Captures[0].Value : string.Empty);

    private static IEnumerable<SqlInstance> ParseBrowserResponses(IEnumerable<string> responses)
    {
        var parserRegex = new Regex(@"[^;]*ServerName;(?<ServerName>[\w\d]+);InstanceName;(?<InstanceName>[\w\d]+);IsClustered;(?<IsClustered>[\w]+);Version;(?<Version>[\d]+\.[\d]+\.[\d]+\.[\d]+)(;tcp;(?<tcp>\d+))?(;np;(?<np>[^;]+))?;;");
        return responses.SelectMany(s => parserRegex.Matches(s)).Select(MapFromMatch);
    }
}

public sealed record SqlInstance(string ServerName, string InstanceName, bool IsClustered, string Version, int TcpPort, string NamedPipe);

internal sealed class UdpBroadcastMessage : IDisposable
{
    private const int SOCKET_TIMEOUT_EXCEPTION_CODE = 10060;

    private readonly CancellationTokenSource _cancellationToken;
    private readonly byte[] _message;
    private readonly int _port;
    private readonly UdpClient _udpClient;

    internal UdpBroadcastMessage(int port, byte[] message, TimeSpan timeout)
    {
        this._port = port;
        this._message = message;
        this._udpClient = new UdpClient { Client = { ReceiveTimeout = 1000 } };
        this._udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
        this._udpClient.EnableBroadcast = true;
        this._cancellationToken = new CancellationTokenSource();
        this._cancellationToken.CancelAfter(timeout);
    }

    public void Dispose()
    {
        this._udpClient?.Dispose();
        this._cancellationToken?.Dispose();
    }

    public List<string> GetResponse()
    {
        var responses = new List<string>();

        var receive = new Task(_ =>
        {
            var anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (!this._cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receiveBuffer = this._udpClient.Receive(ref anyEndPoint);
                    responses.Add(Encoding.UTF8.GetString(receiveBuffer));
                }
                catch (SocketException se) when (se.ErrorCode == SOCKET_TIMEOUT_EXCEPTION_CODE)
                {
                }
            }
        }, this._cancellationToken.Token);

        receive.Start();
        _ = this._udpClient.Send(this._message, this._message.Length, new IPEndPoint(IPAddress.Broadcast, this._port));
        receive.Wait();
        return responses;
    }
}