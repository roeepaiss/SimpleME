// Decompiled with JetBrains decompiler
// Type: NReco.VideoInfo.FFProbe
// Assembly: NReco.VideoInfo, Version=1.1.1.0, Culture=neutral, PublicKeyToken=8f59ee36d2987295
// MVID: EA8FDDF5-134B-4861-AA60-EA39A49F2980
// Assembly location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.dll
// XML documentation location: D:\Projects\Encoders\script-generator-avs-master\packages\NReco.VideoInfo.1.1.1\lib\net35\NReco.VideoInfo.xml

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml.XPath;
using System.Text.Json;

namespace NReco.VideoInfo
{
    /// <summary>
    /// Provides information about media streams, video or audio files (wrapper for FFProbe command line tool)
    /// </summary>
    public class FFProbe
    {
        private static object globalObj = new object();

        /// <summary>Gets or sets path where FFProbe.exe is extracted</summary>
        /// <remarks>
        /// By default this property initialized with folder with application assemblies.
        /// For ASP.NET applications it is recommended to use "~/App_Code/".
        /// </remarks>
        public string ToolPath { get; set; }

        /// <summary>
        /// Get or set FFProbe tool executive file name ('ffprobe.exe' by default)
        /// </summary>
        public string FFProbeExeName { get; set; }

        /// <summary>
        /// Get or set custom WkHtmlToImage command line arguments
        /// </summary>
        public string CustomArgs { get; set; }

        /// <summary>
        /// Gets or sets FFProbe process priority (Normal by default)
        /// </summary>
        public ProcessPriorityClass ProcessPriority { get; set; }

        /// <summary>
        /// Gets or sets maximum execution time for running FFProbe process (null is by default = no timeout)
        /// </summary>
        public TimeSpan? ExecutionTimeout { get; set; }

        /// <summary>Include information about file format.</summary>
        public bool IncludeFormat { get; set; }

        /// <summary>Include information about media streams.</summary>
        public bool IncludeStreams { get; set; }

        /// <summary>Create new instance of HtmlToPdfConverter</summary>
        public FFProbe()
        {
            string str = AppDomain.CurrentDomain.BaseDirectory;
            if (HttpContext.Current != null)
                str = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data\\ffmpeg");
            this.ToolPath = str;
            this.FFProbeExeName = "ffprobe.exe";
            this.ProcessPriority = ProcessPriorityClass.Normal;
            this.IncludeFormat = true;
            this.IncludeStreams = true;
        }

        private void EnsureFFProbe()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
            string str = "NReco.VideoInfo.FFProbe.";
            foreach (string name in manifestResourceNames)
            {
                if (name.StartsWith(str))
                {
                    string path = Path.Combine(this.ToolPath, Path.GetFileNameWithoutExtension(name.Substring(str.Length)));
                    lock (FFProbe.globalObj)
                    {
                        if (File.Exists(path))
                        {
                            //if (File.GetLastWriteTime(path) > File.GetLastWriteTime(executingAssembly.Location))
                            continue;
                        }
                        using (GZipStream gzipStream = new GZipStream(executingAssembly.GetManifestResourceStream(name), CompressionMode.Decompress, false))
                        {
                            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                byte[] buffer = new byte[131072];
                                int count;
                                while ((count = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                                    fileStream.Write(buffer, 0, count);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns information about local media file or online stream (URL).
        /// </summary>
        /// <param name="inputFile">local file path or URL</param>
        /// <returns>Structured information about media</returns>
        public MediaInfo GetMediaInfo(string inputFile) => new MediaInfo(this.GetInfoInternal(inputFile));

        private XPathDocument GetInfoInternal(string input)
        {
            this.EnsureFFProbe();
            try
            {
                string str = Path.Combine(this.ToolPath, this.FFProbeExeName);
                if (!File.Exists(str))
                    throw new FileNotFoundException("Cannot locate FFProbe: " + str);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(" -print_format xml -sexagesimal ");
                if (this.IncludeFormat)
                    stringBuilder.Append(" -show_format ");
                if (this.IncludeStreams)
                    stringBuilder.Append(" -show_streams ");
                if (!string.IsNullOrEmpty(this.CustomArgs))
                    stringBuilder.Append(this.CustomArgs);
                stringBuilder.AppendFormat(" \"{0}\" ", (object)input);
                Process proc = Process.Start(new ProcessStartInfo(str, stringBuilder.ToString())
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(this.ToolPath),
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
                if (this.ProcessPriority != ProcessPriorityClass.Normal)
                    proc.PriorityClass = this.ProcessPriority;
                string lastErrorLine = string.Empty;
                proc.ErrorDataReceived += (DataReceivedEventHandler)((o, args) =>
                {
                    if (args.Data == null)
                        return;
                    lastErrorLine = lastErrorLine + args.Data + "\n";
                });
                proc.BeginErrorReadLine();
                string end = proc.StandardOutput.ReadToEnd();
                this.WaitProcessForExit(proc);
                this.CheckExitCode(proc.ExitCode, lastErrorLine);
                proc.Close();
                return new XPathDocument((TextReader)new StringReader(end));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        public JsonDocument GetInfoJsonInternal(string input)
        {
            this.EnsureFFProbe();
            try
            {
                string str = Path.Combine(this.ToolPath, this.FFProbeExeName);
                if (!File.Exists(str))
                    throw new FileNotFoundException("Cannot locate FFProbe: " + str);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(" -print_format json -sexagesimal ");
                if (this.IncludeFormat)
                    stringBuilder.Append(" -show_format ");
                if (this.IncludeStreams)
                    stringBuilder.Append(" -show_streams ");
                if (!string.IsNullOrEmpty(this.CustomArgs))
                    stringBuilder.Append(this.CustomArgs);
                stringBuilder.AppendFormat(" \"{0}\" ", (object)input);
                Process proc = Process.Start(new ProcessStartInfo(str, stringBuilder.ToString())
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(this.ToolPath),
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
                if (this.ProcessPriority != ProcessPriorityClass.Normal)
                    proc.PriorityClass = this.ProcessPriority;
                string lastErrorLine = string.Empty;
                proc.ErrorDataReceived += (DataReceivedEventHandler)((o, args) =>
                {
                    if (args.Data == null)
                        return;
                    lastErrorLine = lastErrorLine + args.Data + "\n";
                });
                proc.BeginErrorReadLine();
                string end = proc.StandardOutput.ReadToEnd();
                this.WaitProcessForExit(proc);
                this.CheckExitCode(proc.ExitCode, lastErrorLine);
                proc.Close();
                return JsonDocument.Parse(end);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void WaitProcessForExit(Process proc)
        {
            if (this.ExecutionTimeout.HasValue)
            {
                if (!proc.WaitForExit((int)this.ExecutionTimeout.Value.TotalMilliseconds))
                {
                    this.EnsureProcessStopped(proc);
                    throw new FFProbeException(-2, string.Format("FFProbe process exceeded execution timeout ({0}) and was aborted", (object)this.ExecutionTimeout));
                }
            }
            else
                proc.WaitForExit();
        }

        private void EnsureProcessStopped(Process proc)
        {
            if (!proc.HasExited)
            {
                try
                {
                    proc.Kill();
                    proc.Close();
                    proc = (Process)null;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                proc.Close();
                proc = (Process)null;
            }
        }

        private void CheckExitCode(int exitCode, string lastErrLine)
        {
            if (exitCode != 0)
                throw new FFProbeException(exitCode, lastErrLine);
        }
    }
}
