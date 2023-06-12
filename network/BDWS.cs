using MelonLoader;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

public class BDWS
{
    private WebSocket _webSocket;
    private bool _isConnected;
    private CancellationTokenSource _cancellationTokenSource;

    public bool IsConnected => _isConnected;
    public event Action<string> OnMessageReceived;
    public event Action<List<string>> OnOnlineUsersReceived;
    public event Action<string> OnMessageRequestReceived;
    public event Action<string> OnMessageRequestAccepted;
    public event Action<string> OnMessageRequestDeclined;
    public event Action OnConnected;
    public event Action OnDisconnected;

    public async Task ConnectAsync(string serverUri, string username)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            _webSocket = new WebSocket(serverUri);
            _webSocket.OnMessage += (sender, e) => ProcessMessage(e.Data);

            _webSocket.ConnectAsync();

            while (!_webSocket.IsAlive)
            {
                await Task.Delay(100);
            }

            MelonLogger.Msg("WebSocket connected successfully.");

            _isConnected = true;
            OnConnected?.Invoke();

            // Send the username to the server
            await SendMessageAsync(new { type = "username", username = username });
            MelonLogger.Msg("Username sent to the server.");

            // Start listening for messages from the server
            _ = StartListeningAsync();
            MelonLogger.Msg("Started listening for messages.");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"WebSocket connection error: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        _cancellationTokenSource?.Cancel();
        _webSocket.Close();
        MelonLogger.Msg("WebSocket disconnected.");

        _isConnected = false;
        OnDisconnected?.Invoke();
        await Task.CompletedTask;
    }

    public async Task SendMessageAsync(object message)
    {
        if (!_isConnected)
        {
            MelonLogger.Msg("WebSocket is not connected. Cannot send message.");
            return;
        }

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
        _webSocket.Send(json);
        MelonLogger.Msg("Message sent.");
        await Task.CompletedTask;
    }

    public async Task SendGlobalMessageAsync(string message)
    {
        await SendMessageAsync(new
        {
            type = "globalMessage",
            message
        });
        MelonLogger.Msg("Global message sent.");
    }

    private async Task StartListeningAsync()
    {
        try
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested && _webSocket.IsAlive)
            {
                await Task.Delay(10);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"WebSocket receive error: {ex.Message}");
        }
        finally
        {
            _isConnected = false;
            OnDisconnected?.Invoke();
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message);
            string messageType = data?.type.ToString();

            MelonLogger.Msg(message);

            switch (messageType)
            {
                case "info":
                    string infoMessage = data?.message.ToString();
                    MelonLogger.Msg($"Info: {infoMessage}");
                    break;

                case "onlineUsers":
                    List<string> onlineUsers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(data?.usernames.ToString());
                    OnOnlineUsersReceived?.Invoke(onlineUsers);
                    break;

                case "globalMessageReceived":
                    string globalMessage = data?.message.ToString();
                    OnMessageReceived?.Invoke(globalMessage);
                    break;

                case "messageRequest":
                    string requestSender = data?.sender.ToString();
                    OnMessageRequestReceived?.Invoke(requestSender);
                    break;

                case "messageRequestAccepted":
                    string acceptedSender = data?.sender.ToString();
                    OnMessageRequestAccepted?.Invoke(acceptedSender);
                    break;

                case "messageRequestDeclined":
                    string declinedSender = data?.sender.ToString();
                    OnMessageRequestDeclined?.Invoke(declinedSender);
                    break;

                case "banNotification":
                    string banMessage = data?.message.ToString();
                    MelonLogger.Msg(banMessage);
                    DisconnectAsync().Wait(); // Disconnect the client once they receive the ban notification
                    break;

                case "heartbeat":
                    // Ignore heartbeat messages
                    break;

                default:
                    OnMessageReceived?.Invoke(message);
                    break;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Error processing message: {ex.Message}");
        }
    }
}
