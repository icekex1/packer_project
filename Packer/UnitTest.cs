using System;
using System.Diagnostics;
using System.IO;

namespace Packer {

    /// <summary>
    /// Globale Klasse für das Testen von Methoden/Aufgaben 
    /// </summary>
    public static class UnitTest {
        public static bool TestWriteHeader(String name, String expectedName) {
            String outputFile = "testwriteheader.bin";
            FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            Encoder.WriteHeader(bw, name);

            fs.Flush();
            bw.Close();
            fs.Close();

            fs = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            String header = "";

            for(int i = 0; i < fs.Length - 1; i++)
                header += br.ReadChar();

            fs.Flush();
            br.Close();
            fs.Close();

            return Generals.MagicNumber + Generals.Marker + expectedName + Generals.EndOfHeader == header;
        }

        public static bool TestGetCharOfHeader(char expected) {
            String outputFile = "TestGetCharOfHeader.bin";
            FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            Encoder.WriteHeader(bw, outputFile);
            fs.Flush();
            bw.Close();
            fs.Close();

            fs = new FileStream(outputFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            return expected == Decoder.GetMarker(fs, br);
        }

        public static bool TestCheckMagic(string fileName) {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            bool checkMagic = Decoder.CheckMagic(br);

            fs.Flush();
            br.Close();
            fs.Close();

            return checkMagic;
        }

        public static bool CheckFiles(string originalFileName) {
            bool check = true;

            Stopwatch watchEncode = new Stopwatch();
            Stopwatch watchDecode = new Stopwatch();

            watchEncode.Start();
            Encoder.Encode("files\\" + originalFileName, "encode\\" + originalFileName + Generals.FileExt);
            watchEncode.Stop();

            watchDecode.Start();
            Decoder.Decode("encode\\" + originalFileName + Generals.FileExt, "result\\RESULT" + originalFileName);
            watchDecode.Stop();

            FileStream fsOrigin = new FileStream("files\\" + originalFileName, FileMode.Open, FileAccess.Read);
            FileStream fsResult = new FileStream("result\\RESULT" + originalFileName, FileMode.Open, FileAccess.Read);
            BinaryReader brOrigin = new BinaryReader(fsOrigin);
            BinaryReader brResult = new BinaryReader(fsResult);

            if(fsOrigin.Length < fsResult.Length || fsOrigin.Length > fsResult.Length) // Wenn die Originaldatei kleiner als das Ergebnis ist, dann ist sowieso was nicht richtig | Selbe andersrum
                check = false;

            while(fsOrigin.Position < fsOrigin.Length) {
                if(brOrigin.ReadByte() != brResult.ReadByte())
                    check = false;
            }

            fsOrigin.Flush();
            fsResult.Flush();

            brOrigin.Close();
            brResult.Close();
            fsOrigin.Close();
            fsResult.Close();

            FileStream originalFS = new FileStream("files\\" + originalFileName, FileMode.Open, FileAccess.Read);
            FileStream tomFS = new FileStream("encode\\" + originalFileName + Generals.FileExt, FileMode.Open, FileAccess.Read);
            FileStream resultFS = new FileStream("result\\RESULT" + originalFileName, FileMode.Open, FileAccess.Read);

            string content = $"{originalFileName};{originalFS.Length - tomFS.Length};{originalFS.Length - resultFS.Length};{check};{originalFS.Length};{watchEncode.Elapsed.TotalSeconds};{tomFS.Length};{watchDecode.Elapsed.TotalSeconds}";

            originalFS.Flush();
            tomFS.Flush();
            resultFS.Flush();
            originalFS.Close();
            tomFS.Close();
            resultFS.Close();

            StreamWriter sw = new StreamWriter("result.txt", true);
            sw.WriteLine(content);
            sw.Close();

            return check;
        }

        public static bool TestLeastChar(char search) {
            string txt = "TestLeastChar.txt";
            FileStream fs = new FileStream(txt, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            for(byte i = 0; i < byte.MaxValue; i++) { 
                if(i != search)
                    bw.Write(i);
            }

            fs.Flush();
            bw.Close();
            fs.Close();

            fs = new FileStream(txt, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Encoder.GetMarker(br, fs);

            return Generals.Marker == search;
        }
    }
}
