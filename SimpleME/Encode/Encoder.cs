//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using System.Management;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Threading;
//using FFMpegCore;
//using FFMpegCore.Arguments;
//using FFMpegCore.Enums;
//using Microsoft.Win32;
//using ScriptGeneratorAVS.Classes;

//namespace ScriptGeneratorAVS.Encode
//{
//    public class Encoder
//    {
//        private DispatcherTimer threadTimer = new DispatcherTimer();
//        private CancellationTokenSource tokenSource;
//        private Thread thread;
//        private EncoderSharedInfo info;
//        public bool IsEncoding => info.IsEncoding;
//        public bool IsConfigured => info.IsConfigured;
//        public static void Configure(Action<FFOptions> optionsAction)
//        {
//            GlobalFFOptions.Configure(optionsAction);
//            IsConfigured = true;
//        }
//        public static void Configure(FFOptions options)
//        {
//            GlobalFFOptions.Configure(options);
//        }
//        public void Start(string outFile)
//        {
//            if (IsConfigured)
//            {
//                thread = new Thread(filename => Encode((string)filename));
//                thread.Priority = ThreadPriority.Lowest;
//                thread.Start(outFile);
//            }
//        }
//        private void Encode(string outFile)
//        {
//            logFile = Path.Combine(Environment.CurrentDirectory, "log.txt");
//            File.Create(logFile).Close();
//            threadTimer.Interval = TimeSpan.FromMilliseconds(300);
//            threadTimer.Tick += new EventHandler(Tt_Tick);
//            threadTimer.Start();
//            IsEncoding = true;
//            try
//            {
//                var arguments = FFMpegArguments.FromFileInput(Builder.GetMainVideo())
//                   .OutputToFile(outFile, false, options =>
//                   {
//                       options.WithVideoCodec(VideoCodec.LibX264)
//                       .UsingMultithreading(true)
//                       .WithAudioCodec(AudioCodec.Aac)
//                       .OverwriteExisting();
//                       options.WithArgument(new LogArgument(logFile));
//                       // .WithSpeedPreset(Speed.VerySlow))
//                       if (Builder.Subtitles != null && Builder.Subtitles.Count > 0)
//                       {
//                           options.WithVideoFilters(filterOptions =>
//                           {
//                               foreach (var subs in Builder.Subtitles)
//                                   filterOptions.HardBurnSubtitle(SubtitleHardBurnOptions.Create(subs));
//                           });
//                       }
//                   });
//                arguments.ProcessAsynchronously(false).Wait(tokenSource.Token);
//                MessageBox.Show("Completed successfully!", "Message", MessageBoxButton.OK);
//            }
//            catch (FFMpegCore.Exceptions.FFMpegException er)
//            {
//                MessageBox.Show(er.Message + "\n" + er.Source + "\n" + er.Type, er.InnerException?.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
//            }
//            catch (ThreadAbortException)
//            {
//                MessageBox.Show("Aborted", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
//            }
//            finally
//            {
//                IsEncoding = false;
//            }
//        }
//        private void Tt_Tick(object sender, EventArgs e)
//        {
//            if (File.Exists(logFile))
//            {
//                StringBuilder q = new StringBuilder();
//                using FileStream stream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
//                using (StreamReader reader = new StreamReader(stream))
//                {
//                    while (!reader.EndOfStream)
//                    {
//                        string line = reader.ReadLine();
//                        q.Append(line + "\n");
//                    }
//                }
//                string ls = q.ToString();
//                if (string.IsNullOrEmpty(ls))
//                    return;
//                string[] ss = new string[12],
//                    y = ls.Split(new char[] { '\n' });
//                Array.Reverse(y);
//                for (int ii = 0; ii < ss.Length; ii++)
//                {
//                    ss[ii] = y[12 - ii];

//                }
//                double d = (100 * double.Parse(ss[0].Split('=')[1].ToString())) / Builder.VideoFrames;
//                d = Math.Round(d * 100) / 100d;
//                if (d >= 100)
//                    d = 99;
//                if (ss[11].Split('=')[1] == "end")
//                {
//                    threadTimer.Stop();
//                    StopEncoder();
//                    Process.Start("explorer.exe", "/select," + V + @"\");
//                    d = 100;
//                    File.Delete(logFile);
//                    string yi = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
//                    string pa = @"\FFMPEG\doc\";
//                    string n = "Saves.txt";
//                    string c = yi + pa + n;
//                }
//                lblVideoInfo.Content = d + "%" + " (Encoding)";
//                prbPro.Value = d;
//                //Thread.Sleep(200);
//            }
//        }


//        private void btnToggleEncode_Click(object sender, RoutedEventArgs e)
//        {
//            if (!IsEncoding)
//            {
//                if (Builder.GetMainVideo() == null)
//                {
//                    MessageBox.Show("You must choose a video file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }
//                SaveFileDialog s = new SaveFileDialog
//                {
//                    Filter = "Video Files(*.mp4/*.mkv)|*.mp4;*.mkv",
//                    DefaultExt = '.' + cbFormat.Text.ToLower(),
//                    InitialDirectory = Paths.LastPathUse
//                };
//                if (s.ShowDialog() == false)
//                {
//                    MessageBox.Show("You must choose a path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                    return;
//                }
//                try
//                {
//                    thread = new Thread((newFileExtention) => Encode((string)newFileExtention));
//                    thread.Priority = ThreadPriority.Lowest;
//                    thread.Start(s.FileName);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//            else
//            {
//                try
//                {
//                    StopEncoder();
//                    IsEncoding = false;
//                    this.btnToggleEncode.Content = "Encode";
//                }
//                catch
//                {
//                    MessageBox.Show("You need to start in order to stop.");
//                }
//            }
//        }

//        private void StopEncoder()
//        {
//            try
//            {
//                tokenSource.Cancel();
//                tokenSource.Dispose();
//                threadTimer.Stop();
//                thread.Abort();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        //public static event EventHandler Started;
//        //public static event StoppedEventHandler Stopped;
//        //public static event ProgressChangedEventHandler Progress;
//    }
//}
