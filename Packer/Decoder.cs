using System.IO;

namespace Packer {

    /// <summary>
    /// Enkodiert die .tom-Datei zurück in ihren Ursprung
    /// </summary>
    public static class Decoder {

        /// <summary>
        /// Enkodiert die .tom-Datei zurück in ihren Ursprung
        /// </summary>
        /// <param name="fileName">Die Packer-Datei</param>
        /// <returns>TRUE wenn der Decode zuende ging, FALSE wenn die Datei nicht die entsprechende MagicNumber besitzt</returns>
        public static bool Decode(string filename, string newFilename) {
            //Streams erstellen und Reader/Writer öffnen
            FileStream fsR = new FileStream(filename, FileMode.Open, FileAccess.Read);
            FileStream fsW = new FileStream(newFilename, FileMode.Create, FileAccess.Write);
            BinaryReader br = new BinaryReader(fsR);
            BinaryWriter bw = new BinaryWriter(fsW);

            if (fsR.Length == 0)
                return false;

            if (!CheckMagic(br))
                return false;

            GetOldName(fsR, br);

            long length = fsR.Length;

            while(fsR.Position < length) //durch alle einträge von  file durchgehen
            {
                byte c = br.ReadByte();
                if(c == Generals.Marker)
                {
                    byte count = br.ReadByte();// wert wieoft das folgende zeichen vorkommt
                    byte sign = br.ReadByte();//zeichen
                    for(int i = 0; i < count; i++) 
                        bw.Write(sign);
                }
                else {
                    bw.Write(c);
                }
             }

            //Streams flushen und alles schließen
            fsR.Flush();
            fsW.Flush();

            br.Close();
            bw.Close();
            fsR.Close();
            fsW.Close();

            return true;
        }

        /// <summary>
        /// vergleicht magic number mit Generals.MagicNumber um dateiformat zu überprüfen
        /// </summary>
        /// <param name="br"> binary reader zur datei</param>
        /// <returns>true wenn gleich, false wenn ungleich</returns>
        public static bool CheckMagic(BinaryReader br) {
            string mNumber = "";
            for(int i = 0; i < Generals.MagicNumber.Length; i++)
                mNumber += (char)br.ReadByte();
            if(mNumber != Generals.MagicNumber)
                return false;
            else
                return true;
        } 

        /// <summary>
        /// Holt sich den Marker für die Datei aus dem Header
        /// </summary>
        /// /// <param name="fs">Der aktuelle Stream auf die Datei</param>
        /// <param name="br">Der BinaryReader, der die Datei aktuell offen hat</param>
        /// <returns>Der Marker für die Datei</returns>
        
        public static char GetMarker(FileStream fs, BinaryReader br) {
            fs.Position = Generals.MagicNumber.Length;
            return br.ReadChar();
        }

        /// <summary>
        /// Gibt den Namen der Originaldatei zurück
        /// </summary>
        /// <param name="fs">Der FileStream, der die Datei aktuell offen hat</param>
        /// <param name="br">Der BinaryReader, der die Datei aktuell offen hat</param>
        /// <returns>Den Namen der Originaldatei</returns>
        public static string GetOldName(FileStream fs, BinaryReader br) {
            if (fs.Length == 0)
                return"";

            fs.Position = (Generals.MagicNumber + Generals.Marker).Length;
            string oldName = "";

            while(true) {
                byte b = br.ReadByte();
                if(b == Generals.EndOfHeader)
                    break;

                oldName += (char)b;
            }

            return oldName;
        }

        public static string GetOldFileExtension(string name) {
            string[] dotSplitted = name.Split('.');
            return dotSplitted[dotSplitted.Length - 1];
        }
    }
}