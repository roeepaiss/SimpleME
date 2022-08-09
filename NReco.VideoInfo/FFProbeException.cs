// Decompiled with JetBrains decompiler
// Type: NReco.VideoInfo.FFProbeException
// Assembly: NReco.VideoInfo, Version=1.1.1.0, Culture=neutral, PublicKeyToken=8f59ee36d2987295
// MVID: EA8FDDF5-134B-4861-AA60-EA39A49F2980
// Assembly location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.dll
// XML documentation location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.xml

using System;

namespace NReco.VideoInfo
{
  /// <summary>
  /// The exception that is thrown when FFProbe process retruns non-zero error exit code
  /// </summary>
  public class FFProbeException : Exception
  {
    /// <summary>Get FFMpeg process error code</summary>
    public int ErrorCode { get; private set; }

    public FFProbeException(int errCode, string message)
      : base(string.Format("{0} (exit code: {1})", (object) message, (object) errCode))
    {
      this.ErrorCode = errCode;
    }
  }
}
