// Decompiled with JetBrains decompiler
// Type: AntiToxin
// Assembly: Bluedescriptor Rewritten, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3A6715E0-3E73-47B0-980B-864701A87E7A
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\ChilloutVR\Mods\Bluedescriptor Rewritten.dll

using Bluedescriptor_Rewritten.UISYSTEM;
using MelonLoader;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class AntiToxin
{
  public float fpsThreshold = 90f;
  public float timeThreshold = 5f;
  private float timeBelowThreshold;

  public void MonitorFPS()
  {
    if (!MelonPreferences.GetEntryValue<bool>("Bluedescriptor", "rainbowhud"))
      return;
    if ((double) (1f / Time.deltaTime) < (double) this.fpsThreshold)
    {
      this.timeBelowThreshold += Time.deltaTime;
      if ((double) this.timeBelowThreshold >= (double) this.timeThreshold)
      {
        this.HandleFPSDrop();
        this.timeBelowThreshold = 0.0f;
      }
    }
    else
      this.timeBelowThreshold = 0.0f;
  }

  public void HandleFPSDrop()
  {
    MelonLogger.Warning("FPS drop detected! Taking action...");
    new UIfunctions().panic();
  }

  public void MonitorWebPackets()
  {
    int port = 12345;
    byte[] numArray = new byte[1024];
    using (UdpClient udpClient = new UdpClient(port))
    {
      IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
      if (!this.IsSuspiciousPacket(udpClient.Receive(ref remoteEP)))
        return;
      this.HandleSuspiciousPacket();
    }
  }

  public bool IsSuspiciousPacket(byte[] packet) => false;

  public void HandleSuspiciousPacket() => MelonLogger.Warning("Suspicious packet detected! Taking action...");
}
