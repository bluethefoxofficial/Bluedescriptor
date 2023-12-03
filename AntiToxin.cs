using Bluedescriptor_Rewritten.UISYSTEM;
using MelonLoader;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class AntiToxin
{
    public void MonitorFPS()
    {
        var entryValue = MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "n");

        if (entryValue)
        {
            var num = 1f / Time.deltaTime;
            var flag = num < fpsThreshold;

            if (flag)
            {
                timeBelowThreshold += Time.deltaTime;
                var flag2 = timeBelowThreshold >= timeThreshold;

                if (flag2)
                {
                    HandleFPSDrop();
                    timeBelowThreshold = 0f;
                }
            }
            else
            {
                timeBelowThreshold = 0f;
            }
        }
    }

    public void HandleFPSDrop()
    {
        MelonLogger.Warning("FPS drop detected! Taking action...");
        new UIfunctions().panic();
    }

    public void MonitorWebPackets()
    {
        var port = 12345;
        var array = new byte[1024];

        using (UdpClient udpClient = new UdpClient(port))
        {
            var ipendPoint = new IPEndPoint(IPAddress.Any, port);
            var packet = udpClient.Receive(ref ipendPoint);
            var flag = IsSuspiciousPacket(packet);

            if (flag)
            {
                HandleSuspiciousPacket();
            }
        }
    }

    public bool IsSuspiciousPacket(byte[] packet) => false;

    public void HandleSuspiciousPacket()
    {
        MelonLogger.Warning("Suspicious packet detected! Taking action...");
    }

    public float fpsThreshold = 90f;

    public float timeThreshold = 5f;

    float timeBelowThreshold;
}