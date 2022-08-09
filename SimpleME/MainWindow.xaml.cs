using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using Microsoft.Win32;
using SimpleME.Classes;
using SimpleME.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shell;
using System.Windows.Threading;

namespace SimpleME
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Object fileLock = new Object();
        string V;
        Thread thread;
        volatile bool IsEncoding;
        DispatcherTimer tt = new DispatcherTimer();
        string logFile;
        volatile CancellationTokenSource tokenSource;
        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            IsEncoding = false;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string y = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
            string ff = y + @"\FFMPEG\bin\";
            //string[] files = Directory.GetFiles(y, "*.exe", SearchOption.AllDirectories);
            //foreach (var item in files)
            //{
            //    if (Path.GetFileName(item).Equals("ffmpeg.exe"))
            //        ff = Path.GetDirectoryName(item);
            //}
            //try
            //{
                //FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = y +@"\FFMPEG\bin\" });
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Environment.CurrentDirectory + "\\FFMPEG\\bin\\" });

            //}
            //catch (FFMpegCore.Exceptions.FFMpegException)
            //{
            //    GlobalFFOptions.Configure(new FFOptions { BinaryFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\FFMPEG\bin\" });
            //}
            Console.WriteLine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\FFMPEG\bin\");
            //foreach (string s in files)
            //{
            //    string ss = Path.GetFileName(s);
            //    if (ss.Equals("ffms2.dll") || ss.Equals("ImageSeq.dll") || ss.Equals("VSFilterMod.dll"))
            //        ls.Add(s);
            //}
            //Builder.SetPlugins(ls.ToArray());
        }

        private void BtnFindVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url;
                OpenFileDialog f = new OpenFileDialog
                {
                    Filter = " Video Files (*.mkv/*.mp4/*.m4v)|*.mkv;*.mp4;*.m4v|All files (*.*)|*.*",//" Matroska Multimedia Container (*.mkv)|*.mkv|All files (*.*)|*.*",//
                    InitialDirectory = Paths.LastPathUse,
                    Multiselect = false
                };

                if (f.ShowDialog() == true)
                {
                    url = f.FileName;
                    Paths.LastPathUse = V = Path.GetDirectoryName(url);
                    txtVideoUrl.Text = Path.GetFileName(url);
                    Builder.SetMainVideo(url);
                    ((TextBlock)((ScrollViewer)lblVideoDetails.Content).Content).Text = MediaInfoHelper.GetInfoString(url);

                    //string s = lblVideoDetails.Content;
                    //string[] r = s.Split('\n');
                    double n = Paths.GetVideoDuration(url).TotalSeconds;
                    //string[] qq = r[8].Split(' ');
                    //Builder.VideoFrameRate = double.Parse(qq[2].Split('f')[0]);
                    Builder.VideoFrameRate = MediaInfoHelper.GetFrameRate(url);
                    Builder.VideoFrames = Math.Round(n * Builder.VideoFrameRate);
                    //if (Path.GetExtension(url) == ".mkv")
                    //{
                    //    r[4] = "Video Duration: " + Paths.GetVideoDuration(f.FileName).ToString(@"hh\:mm\:ss");
                    //    StringBuilder a = new StringBuilder();
                    //    for (int i = 0; i < r.Length - 1; i++)
                    //    {
                    //        a.Append(r[i] + "\n");
                    //    }
                    //    a.Append("Size: " + (Paths.ConvertToSize(new FileInfo(url).Length)));
                    //    lblVideoDetails.Content = a.ToString();
                    //}
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message, es.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnFindSubs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog
            {
                Multiselect = true,
                Filter = " Aegisub Advanced Subtitle (*.ass)|*.ass|All files (*.*)|*.*",//Aegisub Advanced Subtitle
                InitialDirectory = Paths.LastPathUse
            };

            if (f.ShowDialog() == true)
            {
                Paths.LastPathUse = Path.GetDirectoryName(f.FileNames[0]);
                foreach (string filename in f.FileNames)
                {
                    lbSubs.Items.Add(System.IO.Path.GetFileName(filename));
                    Builder.AddSubtitle(filename);
                }
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnClearSubs_Click(object sender, RoutedEventArgs e)
        {
            lbSubs.Items.Clear();
            Builder.RemoveSubtitles();
        }

        private void btnRemoveSubs_Click(object sender, RoutedEventArgs e)
        {
            int n = lbSubs.SelectedIndex;
            if (n == -1)
                return;
            lbSubs.Items.RemoveAt(n);
            Builder.RemoveSubtitle(n);
        }

        private void BtnBuild_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtVideoUrl.Text))
            {
                MessageBox.Show("You Must Set a Video!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog f = new SaveFileDialog
            {
                Filter = "AVS Files (*.avs)| *.avs",
                DefaultExt = "avs",
                InitialDirectory = Paths.LastPathUse

            };
            if (f.ShowDialog() == true)
            {
                System.IO.Stream fileStream = f.OpenFile();
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream);
                sw.WriteLine(Builder.Build(false));
                sw.Flush();
                sw.Close();
            }
        }

        private void Window_Closed(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Encode(string outFile)
        {
            logFile = Path.Combine(Environment.CurrentDirectory, "log.txt");
            File.Create(logFile).Close();
            IsEncoding = true;
            try
            {
                var argumentProsessor = FFMpegArguments.FromFileInput(Builder.GetMainVideo(), true, options =>
                {
                    //options.WithHardwareAcceleration();
                    //options.WithCustomArgument("-hwaccel");

                }).OutputToFile(outFile, true, options =>
                       {
                           options//.WithSpeedPreset(Speed.Slower)
                           .WithVideoCodec(VideoCodec.LibX264)
                           .WithArgument(new LogArgument(logFile))
                           .WithCustomArgument("-crf 21 -sn")
                           .UsingMultithreading(false)
                           .WithAudioCodec(AudioCodec.Aac);
                           if (Builder.Subtitles != null && Builder.Subtitles.Count > 0)
                           {
                               options.WithVideoFilters(filterOptions =>
                               {
                                   foreach (var subs in Builder.Subtitles)
                                       filterOptions.HardBurnSubtitle(SubtitleHardBurnOptions.Create(subs));
                               });
                           }
                           //options.WithFastStart();
                       });
                argumentProsessor.CancellableThrough(tokenSource.Token);
                argumentProsessor.ProcessSynchronously(true);
                //.ProcessAsynchronously(false).Wait(tokenSource.Token);
                MessageBox.Show("Completed successfully!", "Message", MessageBoxButton.OK);
            }
            catch (FFMpegCore.Exceptions.FFMpegException er)
            {
                MessageBox.Show(er.Message.Substring(er.Message.Length / 2), er.InnerException?.Message, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            catch (ThreadAbortException)
            {
                MessageBox.Show("Aborted", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            finally
            {
                IsEncoding = false;
            }
        }
        private void Tt_Tick(object sender, EventArgs e)
        {
            if (File.Exists(logFile))
            {
                StringBuilder q = new StringBuilder();
                using FileStream stream = File.Open(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        q.Append(line + "\n");
                    }
                }
                string ls = q.ToString();
                if (string.IsNullOrEmpty(ls))
                    return;
                string[] ss = new string[12],
                    y = ls.Split(new char[] { '\n' });
                Array.Reverse(y);
                for (int ii = 0; ii < ss.Length; ii++)
                {
                    ss[ii] = y[12 - ii];

                }
                double d = (100 * double.Parse(ss[0].Split('=')[1].ToString())) / Builder.VideoFrames;
                d = Math.Round(d * 100) / 100d;
                if (d >= 100)
                    d = 99;
                if (ss[11].Split('=')[1] == "end")
                {
                    tt.Stop();
                    IsEncoding = false;
                    this.btnToggleEncode.Content = "Encode";
                    d = 100;
                    tokenSource = null;
                    Process.Start("explorer.exe", "/select," + V + @"\");

                    string yi = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)));
                    string pa = @"\FFMPEG\doc\";
                    string n = "Saves.txt";
                    string c = yi + pa + n;
                }
                lblVideoInfo.Content = d + "%" + " (Encoding)";
                prbPro.Value = d;
                //Thread.Sleep(200);
            }
        }


        private void btnToggleEncode_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEncoding)
            {
                if (Builder.GetMainVideo() == null)
                {
                    MessageBox.Show("You must choose a video file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SaveFileDialog s = new SaveFileDialog
                {
                    Filter = "Video Files(*.mp4/*.mkv)|*.mp4;*.mkv",
                    DefaultExt = '.' + cbFormat.Text.ToLower(),
                    InitialDirectory = Paths.LastPathUse
                };
                if (s.ShowDialog() == false)
                {
                    MessageBox.Show("You must choose a path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    tt.Interval = TimeSpan.FromMilliseconds(300);
                    tt.Tick += Tt_Tick;
                    tt.Start();
                    if (tokenSource == null)
                        tokenSource = new CancellationTokenSource();
                    thread = new Thread((newFileExtention) => Encode((string)newFileExtention));
                    thread.Priority = ThreadPriority.Normal;
                    thread.Start(s.FileName);
                    this.btnToggleEncode.Content = "Abort";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                tt.Stop();
                tokenSource.Cancel();
                thread.Abort();
                IsEncoding = false;
                this.btnToggleEncode.Content = "Encode";
                prbPro.Value = 100;
                lblVideoInfo.Content = 100 + "%";
                tokenSource.Dispose();
                tokenSource = null;

            }
        }

        private void prbPro_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine(e.NewValue / 100);
            TaskbarItemInfo.ProgressValue = e.NewValue / 100;
        }

        public bool IsValid(string str)
        {
            double i;
            return double.TryParse(str, out i) && i >= 0 && i <= 51;
        }

        private void Window_Update(object sender, RoutedEventArgs e)
        {
            var updater = new UpdateManagerWindow();
            updater.Owner = this;
            updater.Topmost = true;
            updater.Show();
        }
    }
}
