using System;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Input;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Packer {

    public partial class MainWindow : Window {

        FileInfo file; // Um die Infos der ausgwählten Datei zu erhalten
        bool canWork;

        public MainWindow() {
            InitializeComponent();
            canWork = true;

            #region Tests
            // ######### TESTS WERDEN IRGENDWO BEHALTEN UM SIE VORZUZEIGEN BEI SCHWIERIGKEITEN

            //String longName = "test123455678.txt";
            //String name = "1234567.txt";

            //MessageBox.Show(UnitTest.TestWriteHeader(longName, "test1234.txt").ToString() + " " + UnitTest.TestWriteHeader(name, "1234567.txt").ToString());

            //MessageBox.Show("Test Marker: " + UnitTest.TestGetCharOfHeader(Generals.Marker).ToString());

            //MessageBox.Show($"Bei einer richtigen Datei: {UnitTest.TestCheckMagic("result.jpg.tom").ToString()}\r\n" +
            //                $"Bei einer Datei ohne MagicNumber: {UnitTest.TestCheckMagic("FileWithoutMagic.tom").ToString()}\r\n" +
            //                $"Bei einer Datei mit halber MagicNumber: {UnitTest.TestCheckMagic("HalfMagicNumber.tom").ToString()}");

            MessageBox.Show($"Test Least Char: {UnitTest.TestLeastChar('X')} | {Generals.Marker}");
            Clipboard.SetText("" + (byte)Generals.Marker);

            //Thread t = new Thread(new ThreadStart(TestFiles));
            //t.Start();
            #endregion
        }

        private void Button_ChoosePath(object sender, MouseButtonEventArgs e) {
            if(!canWork) {
                Message.Show("Achtung", "Nicht gleichzeitig De- oder Encoden", 16);
                return;
            }
            
            OpenFileDialog open = new OpenFileDialog();

            if(open.ShowDialog() == true) {
                file = new FileInfo(open.FileName);
                FileName.Text = file.Name;

                if(file.Extension == Generals.FileExt) {
                    EncodeButton.Visibility = Visibility.Hidden;
                    DecodeButton.Visibility = Visibility.Visible;
                }
                else {
                    DecodeButton.Visibility = Visibility.Hidden;
                    EncodeButton.Visibility = Visibility.Visible;
                }
            }
        }
        
        private async void Button_Decode(object sender, MouseButtonEventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            sf.FileName = Decoder.GetOldName(fs, br); 
            string fileExt = Decoder.GetOldFileExtension(sf.SafeFileName);
            sf.Filter = fileExt.Replace(".", "") + " Datei |." + fileExt;

            fs.Flush();
            br.Close();
            fs.Close();

            if(sf.ShowDialog() == true) {
                canWork = false;
                DecodeButton.Visibility = Visibility.Hidden;
                FileName.Text = "";
                
                await Task.Run(() => Decoder.Decode(file.FullName, sf.FileName));
                Message.Show("Decoder", "Fertig");
                canWork = true;
            }
        }

        private async void Button_Encode(object sender, MouseButtonEventArgs e) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = Generals.DialogFilter;
            sf.InitialDirectory = file.DirectoryName; // Ändert das SaveFileDialog Verzeichnis auf das der Originaldatei
            sf.FileName = file.Name.Replace(file.Extension, "") + Generals.FileExt; // Standardname ist der Name der Originaldatei

            if(sf.ShowDialog() == true) {
                canWork = false;
                EncodeButton.Visibility = Visibility.Hidden;
                FileName.Text = "";
    
                await Task.Run(() => Encoder.Encode(file.FullName, sf.FileName));
                Message.Show("Encoder", "Fertig");
                canWork = true;
            }
        }

        void TestFiles() {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\files");

            for(int i = 0; i < files.Length; i++)
                files[i] = files[i].Replace(Directory.GetCurrentDirectory() + @"\files\", "");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach(string f in files) 
                UnitTest.CheckFiles(f);
            watch.Stop();

            Message.Show("UnitTest für Files", "Fertig | " + watch.Elapsed.TotalMinutes + " Minuten");
        }
    }
}
