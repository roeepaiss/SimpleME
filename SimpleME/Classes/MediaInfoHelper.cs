using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
//using FFMpegCore;
using FFMpegCore.Builders;
using NReco.VideoInfo;

namespace SimpleME.Classes
{
    public static class MediaInfoHelper
    {
        private static FFProbe ffProbe;
        static MediaInfoHelper()
        {
            ffProbe = new FFProbe();
            //ffProbe.ToolPath = Environment.CurrentDirectory;
            //ffProbe.
        }

        public static string GetInfoString(string path)
        {
            MediaInfo mediaInfo = ffProbe.GetMediaInfo(path);
            //mediaInfo.Result.CreateNavigator().OuterXml;
            //XmlDataDocument xmlDocument = new XmlDataDocument();
            //xmlDocument.
            JsonDocument jdoc = ffProbe.GetInfoJsonInternal(path);
            
            return jdoc.RootElement.ToString(); //mediaInfo.Result.CreateNavigator().OuterXml;
        }
        private static string ExtractXml()
        {
            return "";
        }

        public static double GetFrameRate(string path)
        {
            double fps = 0;
            MediaInfo mediaInfo = ffProbe.GetMediaInfo(path);
            foreach(MediaInfo.StreamInfo streamInfo in mediaInfo.Streams)
            {
                if(streamInfo.CodecType == "video")
                    fps = streamInfo.FrameRate;
            }
            return fps;
        }
    }
}
