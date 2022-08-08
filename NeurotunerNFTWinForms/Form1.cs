using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CSCore;
using CSCore.SoundIn;//Вход звука
using CSCore.SoundOut;//Выход звука
using CSCore.CoreAudioAPI;
using CSCore.Streams;

using CSCore.Codecs;
using CSCore.Codecs.WAV;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.IO;

//using Microsoft.DirectX.DirectSound;
//using Buffer = Microsoft.DirectX.DirectSound.Buffer;
using System.Runtime.InteropServices;

using CSCore.DSP;
using System.Globalization;
using CSCore.Streams.Effects;
using TEST_API;

namespace NeurotunerNFTWinForms
{
    public partial class NeurotunerNFT : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("BiblZvuk.dll", CallingConvention = CallingConvention.Cdecl)]
        //unsafe
        public static extern int vizualzvuk(string filename, string secfile, int[] Rdat, int ParV);

        private FileInfo fileInfo = new FileInfo("window.tmp");
        private FileInfo fileInfo1 = new FileInfo("Data_Load.tmp");
        private FileInfo FileLanguage = new FileInfo("Data_Language.tmp");
        private FileInfo fileinfo = new FileInfo("DataTemp.tmp");
        private SimpleMixer mMixer;
        private int SampleRate;//44100;
        //private Equalizer equalizer;
        private WasapiOut mSoundOut;
        private WasapiCapture mSoundIn;
        private SampleDSP mDsp;
        private SampleDSPRecord mDspRec;
        private SampleDSPTurbo mDspTurbo;
        string[] file1 = File.ReadAllLines("window.tmp");

        string folder = "Record";
        private IWaveSource mSource;
        private MMDeviceCollection mOutputDevices;
        private MMDeviceCollection mInputDevices;
        public double Magn;
        string myfile;
        string cutmyfile;
        public int index = 1;
        string langindex;

        private ISampleSource mMp3;
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string path2;
        private string file, filename, RecordName;
        private string record;
        private string[] allfile;
        private int click, audioclick = 0;

        private static int limit = 20;
        private int ImgBtnStartClick = 0, ImgBtnRecordClick = 0, ImgBtnListenClick = 0, ImgBtnTurboClick = 0, ModeIndex, BtnSetClick = 0, NFTShadow = 0;

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        BackgroundWorker worker;

        public NeurotunerNFT()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (worker.CancellationPending == true)
                    {
                        //e.Cancel = true;
                        (sender as BackgroundWorker).ReportProgress(100);
                        break;
                        //return;
                    }
                    (sender as BackgroundWorker).ReportProgress(i);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_DoWork: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                /*PBNFT.Value = e.ProgressPercentage;
                if (PBNFT.Value == 25)
                {
                    string uri = @"Neurotuners\progressbar\Group 13.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 50)
                {
                    string uri = @"Neurotuners\progressbar\Group 12.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 75)
                {
                    string uri = @"Neurotuners\progressbar\Group 11.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                }
                else if (PBNFT.Value == 100)
                {
                    PBNFT.Visibility = Visibility.Hidden;
                    lbPBNFT.Visibility = Visibility.Hidden;
                    string uri = @"Neurotuners\element\progressbar-backgrnd.png";
                    ImgPBNFTback.ImageSource = new ImageSourceConverter().ConvertFromString(uri) as ImageSource;
                    //imgPBNFTBack.Visibility = Visibility.Hidden;
                }*/
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in worker_ProgressChanged: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void select_an_entry()
        {
            try
            {
                if (langindex == "0")
                {
                    string msg = "Выберите запись.";
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "Select a record.";
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в select_an_entry: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in select_an_entry: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void NFT_download()
        {
            if (langindex == "0")
            {
                string msg = "Подождите пока загрузится NFT картинка.";
                MessageBox.Show(msg);
            }
            else
            {
                string msg = "Wait for the NFT image to load.";
                MessageBox.Show(msg);
            }
        }

        private void Mixer()
        {
            try
            {
                mMixer = new SimpleMixer(1, SampleRate) //стерео, 44,1 КГц
                {
                    FillWithZeros = true,
                    DivideResult = true, //Для этого установлено значение true, чтобы избежать звуков тиков из-за превышения -1 и 1.
                };
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Mixer: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void NeurotunerNFT_Load(object sender, EventArgs e)
        {
            try
            {

                if (file1.Length == 0)
                {
                    //File.Create("DataTemp.dat");
                    //WelcomeWindow window = new WelcomeWindow();
                    //window.Show();
                    //File.AppendAllText(fileInfo.FullName, "1");
                }
                //Находит устройства для захвата звука и заполнияет комбобокс
                MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();
                mInputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active);
                MMDevice activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                SampleRate = activeDevice.DeviceFormat.SampleRate;

                foreach (MMDevice device in mInputDevices)
                {
                    //cmbInput.Items.Add(device.FriendlyName);
                    //if (device.DeviceID == activeDevice.DeviceID) cmbInput.SelectedIndex = cmbInput.Items.Count - 1;
                }

                //Находит устройства для вывода звука и заполняет комбобокс
                activeDevice = deviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                mOutputDevices = deviceEnum.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active);
                foreach (MMDevice device in mOutputDevices)
                {
                    //cmbOutput.Items.Add(device.FriendlyName);
                    //if (device.DeviceID == activeDevice.DeviceID) cmbOutput.SelectedIndex = cmbOutput.Items.Count - 1;
                }

                //cmbRecord.Items.Add("Select a record");
                //cmbRecord.SelectedIndex = cmbRecord.Items.Count - 1;
                Filling();
                string[] filename = File.ReadAllLines(fileInfo1.FullName);
                if (filename.Length == 1)
                {
                    //Languages();
                }
                if (!File.Exists("log.tmp"))
                {
                    File.Create("log.tmp").Close();
                }
                else
                {
                    if (File.ReadAllLines("log.tmp").Length > 1000)
                    {
                        File.WriteAllText("log.tmp", " ");
                    }
                }
                if (!Directory.Exists("Image"))
                {
                    Directory.CreateDirectory("Image");
                }
                if (!Directory.Exists("Record"))
                {
                    Directory.CreateDirectory("Record");
                }
                if (!Directory.Exists(path + "NeurotunerNFT"))
                {
                    Directory.CreateDirectory(path + @"\NeurotunerNFT");
                    path2 = path + @"\NeurotunerNFT\Data";

                }
                //SampleRate = mSoundIn.WaveFormat.SampleRate;
                TembroClass tembro = new TembroClass();
                tembro.Tembro(48000);

                if (check.strt(path2) > limit)
                {
                    //this.IsEnabled = false;
                    //ActivationForm activation = new ActivationForm();
                    //activation.Show();
                }
                //SendMessageW(hWnd, WM_APPCOMMAND, hWnd, (IntPtr)APPCOMMAND_VOLUME_UP);
                //pbVolumeLeft.Value = (double)hWnd;
                ModeIndex = -1;
                //Modes();
                //System.Media.SystemSound sound;
                //sound.GetPropertyValue;
                //ShowCurrentVolume();

                //path = System.IO.Path.GetFullPath("Zvuk.dll");
                /*uint volume;
                waveOutGetVolume(IntPtr.Zero, out volume);
                int vol = (int)((volume >> 16) & 0xFFFF);
                pbVolume.Value = vol;*/
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Loaded: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Filling()
        {
            try
            {
                allfile = Directory.GetFiles(folder);
                foreach (string filename in allfile)
                {
                    //record = filename.Replace(@"Record\", "");
                    record = filename.Remove(0, 7);
                    //cmbRecord.Items.Add(record);
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Filling: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Filling: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private void Stop()
        {
            try
            {
                if (mMixer != null)
                {
                    mMixer.Dispose();
                    mMp3.ToWaveSource(32).Loop().ToSampleSource().Dispose();
                    mMixer = null;
                }
                if (mSoundOut != null)
                {
                    mSoundOut.Stop();
                    mSoundOut.Dispose();
                    mSoundOut = null;
                }
                if (mSoundIn != null)
                {
                    mSoundIn.Stop();
                    mSoundIn.Dispose();
                    mSoundIn = null;
                }
                if (mSource != null)
                {
                    mSource.Dispose();
                    mSource = null;
                }
                if (mMp3 != null)
                {
                    if (mDspRec != null)
                    {
                        mDspRec.Dispose();
                    }
                    mMp3.Dispose();
                    mMp3 = null;
                }
            }
            catch (Exception ex)
            {
                /*if (langindex == "0")
                {
                    string msg = "Ошибка в Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Stop: \r\n" + ex.Message;
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }*/
            }
        }

        private async void StartFullDuplex()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                //mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                //Init DSP для смещения высоты тона
                mDsp = new SampleDSP(source.ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/.ToMono());

                //SetPitchShiftValue();
                mSoundIn.Start();

                //Инициальный микшер
                //Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDsp.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.
                //await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplex: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private async void StartFullDuplexTurbo()//запуск пича и громкости
        {
            try
            {
                //Запускает устройство захвата звука с задержкой 1 мс.
                mSoundIn = new WasapiCapture(/*false, AudioClientShareMode.Exclusive, 1*/);
                //mSoundIn.Device = mInputDevices[cmbInput.SelectedIndex];
                mSoundIn.Initialize();

                var source = new SoundInSource(mSoundIn) { FillWithZeros = true };

                //Init DSP для смещения высоты тона
                mDspTurbo = new SampleDSPTurbo(source.ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/.ToMono());
                //SetPitchShiftValue();

                //SetPitchShiftValue();
                mSoundIn.Start();

                //Инициальный микшер
                Mixer();

                //Добавляем наш источник звука в микшер
                mMixer.AddSource(mDspTurbo.ChangeSampleRate(mMixer.WaveFormat.SampleRate));

                //Запускает устройство воспроизведения звука с задержкой 1 мс.

                await Task.Run(() => SoundOut());

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в StartFullDuplexTurbo: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in StartFullDuplexTurbo: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
            //return false;
        }

        private void SoundOut()
        {
            try
            {
                mSoundOut = new WasapiOut(/*false, AudioClientShareMode.Exclusive, 1*/);

                //mSoundOut.Device = mOutputDevices[cmbOutput.SelectedIndex];
                mSoundOut.Initialize(mMixer.ToWaveSource(32));

                //mSoundOut.Initialize(mSource);
                mSoundOut.Play();
                mSoundOut.Volume = 10;

            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in SoundOut: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }

        private async void Sound(string file)
        {
            try
            {
                //Stop();
                if (click != 0)
                {
                    Mixer();
                    mMp3 = CodecFactory.Instance.GetCodec(filename).ToMono().ToSampleSource()/*.AppendSource(Equalizer.Create10BandEqualizer, out mEqualizer)*/;
                    mDspRec = new SampleDSPRecord(mMp3.ToWaveSource(32).ToSampleSource());
                    mMixer.AddSource(mDspRec.ChangeSampleRate(mMixer.WaveFormat.SampleRate).ToWaveSource(32).Loop().ToSampleSource());
                    await Task.Run(() => SoundOut());

                }
                else
                {
                    Stop();
                }
            }
            catch (Exception ex)
            {
                if (langindex == "0")
                {
                    string msg = "Ошибка в Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
                else
                {
                    string msg = "Error in Sound: \r\n" + ex.Message;
                    LogClass.LogWrite(msg);
                    MessageBox.Show(msg);
                    Debug.WriteLine(msg);
                }
            }
        }
    }
}
