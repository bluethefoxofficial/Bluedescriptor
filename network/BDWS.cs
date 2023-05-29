using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MelonLoader;

namespace Bluedescriptor_Rewritten
{
    public class BDWS
    {
        private ClientWebSocket _clientWebSocket;
        private CancellationTokenSource _cancellationTokenSource;
        public event Action<string> OnMessageReceived;
        public event Action<List<string>> OnOnlineUsersReceived;
        public async Task ConnectAsync(string serverUri, string username)
        {
            _clientWebSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await _clientWebSocket.ConnectAsync(new Uri(serverUri), _cancellationTokenSource.Token);
                MelonLogger.Msg("WebSocket connected successfully.");
                // Send the username to the server
                await SendMessageAsync(new { type = "username", username });
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
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
            _clientWebSocket.Dispose();
            MelonLogger.Msg("WebSocket disconnected.");
        }

        public async Task SendMessageAsync(object message)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            MelonLogger.Msg("Message sent.");
        }

        private async Task StartListeningAsync()
        {
            try
            {
                var buffer = new byte[1024];
                var receivedData = new List<byte>();

                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    receivedData.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));

                    if (result.EndOfMessage)
                    {
                        string message = Encoding.UTF8.GetString(receivedData.ToArray());
                        receivedData.Clear();

                        ProcessMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"WebSocket client error: {ex.Message}");
            }
        }

        private void ProcessMessage(string message)
        {
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(message);
            string messageType = data?.type.ToString();

            switch (messageType)
            {
                case "info":
                    string infoMessage = data?.message.ToString();
                    MelonLogger.Msg($"Info: {infoMessage}");
                    break;

                case "onlineUsers":
                    List<string> onlineUsers = data?.usernames.ToObject<List<string>>();
                    OnOnlineUsersReceived?.Invoke(onlineUsers);
                    break;

                case "heartbeat":
                    // Respond to heartbeat messages from the server
                    SendHeartbeatAsync();
                    break;

                default:
                    OnMessageReceived?.Invoke(message);
                    break;
            }
        }

        private async void SendHeartbeatAsync()
        {
            await SendMessageAsync(new { type = "heartbeat" });
            MelonLogger.Msg("Heartbeat sent.");
        }
    }
}