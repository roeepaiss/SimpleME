// Decompiled with JetBrains decompiler
// Type: NReco.VideoInfo.MediaInfo
// Assembly: NReco.VideoInfo, Version=1.1.1.0, Culture=neutral, PublicKeyToken=8f59ee36d2987295
// MVID: EA8FDDF5-134B-4861-AA60-EA39A49F2980
// Assembly location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.dll
// XML documentation location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.xml

using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace NReco.VideoInfo
{
  /// <summary>Represents information about media file or stream.</summary>
  public class MediaInfo
  {
    private KeyValuePair<string, string>[] _FormatTags;
    private MediaInfo.StreamInfo[] _Streams;

    /// <summary>Media container format identifier.</summary>
    public string FormatName => this.GetAttrValue("/ffprobe/format/@format_name");

    /// <summary>Human-readable container format name.</summary>
    public string FormatLongName => this.GetAttrValue("/ffprobe/format/@format_long_name");

    /// <summary>List of media container tags.</summary>
    public KeyValuePair<string, string>[] FormatTags => this._FormatTags ?? (this._FormatTags = this.GetTags("/ffprobe/format/tag"));

    /// <summary>List of media streams.</summary>
    public MediaInfo.StreamInfo[] Streams => this._Streams ?? (this._Streams = this.GetStreams());

    /// <summary>Total duration of the media.</summary>
    public TimeSpan Duration
    {
      get
      {
        string attrValue = this.GetAttrValue("/ffprobe/format/@duration");
        TimeSpan result;
        return !string.IsNullOrEmpty(attrValue) && TimeSpan.TryParse(attrValue, out result) ? result : TimeSpan.Zero;
      }
    }

    /// <summary>FFProble XML result.</summary>
    public XPathDocument Result { get; private set; }

    public MediaInfo(XPathDocument ffProbeResult) => this.Result = ffProbeResult;

    /// <summary>Returns attribute value from FFProbe XML result.</summary>
    /// <param name="xpath">XPath selector</param>
    /// <returns>attribute value or null</returns>
    public string GetAttrValue(string xpath) => this.Result.CreateNavigator().SelectSingleNode(xpath)?.Value;

    private KeyValuePair<string, string>[] GetTags(string xpath)
    {
      XPathNavigator navigator = this.Result.CreateNavigator();
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string xpath1 = xpath;
      XPathNodeIterator xpathNodeIterator = navigator.Select(xpath1);
      while (xpathNodeIterator.MoveNext())
        keyValuePairList.Add(new KeyValuePair<string, string>(xpathNodeIterator.Current.GetAttribute("key", string.Empty), xpathNodeIterator.Current.GetAttribute("value", string.Empty)));
      return keyValuePairList.ToArray();
    }

    private MediaInfo.StreamInfo[] GetStreams()
    {
      List<MediaInfo.StreamInfo> streamInfoList = new List<MediaInfo.StreamInfo>();
      XPathNodeIterator xpathNodeIterator = this.Result.CreateNavigator().Select("/ffprobe/streams/stream/@index");
      while (xpathNodeIterator.MoveNext())
        streamInfoList.Add(new MediaInfo.StreamInfo(this, xpathNodeIterator.Current.Value));
      return streamInfoList.ToArray();
    }

    /// <summary>Represents information about stream.</summary>
    public class StreamInfo
    {
      private MediaInfo Info;
      private KeyValuePair<string, string>[] _Tags;

      /// <summary>Stream index</summary>
      public string Index { get; private set; }

      /// <summary>Codec name identifier</summary>
      public string CodecName => this.Info.GetAttrValue(this.XPathPrefix + "/@codec_name");

      /// <summary>Human-readable codec name.</summary>
      public string CodecLongName => this.Info.GetAttrValue(this.XPathPrefix + "/@codec_long_name");

      /// <summary>Codec type (video, audio).</summary>
      public string CodecType => this.Info.GetAttrValue(this.XPathPrefix + "/@codec_type");

      /// <summary>Video stream pixel format (if applicable).</summary>
      /// <remarks>Null is returned if pixel format is not available.</remarks>
      public string PixelFormat => this.Info.GetAttrValue(this.XPathPrefix + "/@pix_fmt");

      /// <summary>Video frame width (if applicable).</summary>
      public int Width => this.ParseInt(this.Info.GetAttrValue(this.XPathPrefix + "/@width"));

      /// <summary>Video frame height (if applicable)</summary>
      public int Height => this.ParseInt(this.Info.GetAttrValue(this.XPathPrefix + "/@height"));

      /// <summary>Video frame rate per second (if applicable).</summary>
      public float FrameRate
      {
        get
        {
          string attrValue = this.Info.GetAttrValue(this.XPathPrefix + "/@r_frame_rate");
          if (!string.IsNullOrEmpty(attrValue))
          {
            string[] strArray = attrValue.Split('/');
            if (strArray.Length == 2)
            {
              int num1 = this.ParseInt(strArray[0]);
              int num2 = this.ParseInt(strArray[1]);
              if (num1 > 0 && num2 > 0)
                return (float) num1 / (float) num2;
            }
          }
          return -1f;
        }
      }

      private int ParseInt(string s)
      {
        int result;
        return !string.IsNullOrEmpty(s) && int.TryParse(s, out result) ? result : -1;
      }

      public KeyValuePair<string, string>[] Tags => this._Tags ?? (this._Tags = this.Info.GetTags(this.XPathPrefix + "/tag"));

      internal StreamInfo(MediaInfo info, string index)
      {
        this.Info = info;
        this.Index = index;
      }

      private string XPathPrefix => "/ffprobe/streams/stream[@index=\"" + this.Index + "\"]";
    }
  }
}
