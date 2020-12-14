using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFileStream
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamSample ss = new StreamSample();

            ss.TextReaderEX();
        }

        #region 字串 <=> Byte[]

        /// <summary>
        /// 字串轉byte[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] StringToBytes(string str)
        {
            UnicodeEncoding converter = new UnicodeEncoding();
            return converter.GetBytes(str);
        }

        /// <summary>
        /// bytes轉String
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string BytesToString(byte[] bytes)
        {
            UnicodeEncoding converter = new UnicodeEncoding();
            return converter.GetString(bytes);
        }

        #endregion

        #region byte[] <=> Base64 String

        /// <summary>
        /// byte轉Base64
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string BytesToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64轉Byte[]
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public byte[] Base64ToBytes(string base64)
        {
            return Convert.FromBase64String(base64);
        }

        #endregion

        #region Stream <=> Byte[]

        /// <summary>
        /// Stream 轉 bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 設定當前流的位置為流的開始 

            stream.Seek(0, SeekOrigin.Begin);

            stream.Close();

            return bytes;
        }

        /// <summary>
        /// Bytes 轉 Steam
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);

            return stream;
        }

        #endregion

        #region Stream <=> File

        /// <summary>
        /// 把Stream 寫入 File(建立檔案)
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName">完整檔案路徑，包含檔名</param>
        public bool StreamToFile(Stream stream, string fileName)
        {
            try
            {
                // 把 Stream 轉換成 byte[] 

                byte[] bytes = new byte[stream.Length];

                // 把 byte[] 寫入檔案 
                stream.Read(bytes, 0, bytes.Length);

                // 設定當前流的位置為流的開始 
                stream.Seek(0, SeekOrigin.Begin);

                stream.Close();

                FileStream fs = new FileStream(fileName, FileMode.Create);

                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(bytes);

                bw.Close();

                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// File 轉 Stream
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Stream FileToStream(string fileName)
        {
            // 開啟檔案 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            // 讀取檔案的 byte[] 
            byte[] bytes = new byte[fileStream.Length];

            fileStream.Read(bytes, 0, bytes.Length);

            fileStream.Close();

            // 把 byte[] 轉換成 Stream 

            Stream stream = new MemoryStream(bytes);

            return stream;
        }

        #endregion

        #region Byte[] <=> File

        /// <summary>
        /// 透過byte 寫入 File
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool BytesToFile(byte[] bytes, string fileName)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// File 轉 Bytes
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] FileToBytes(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);

            long numBytes = new FileInfo(fileName).Length;

            buff = br.ReadBytes((int)numBytes);

            fs.Close();

            br.Close();

            return buff;
        }

        /// <summary>
        /// 透過byte 寫入 File
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool BytesToFile1(byte[] bytes, string fileName)
        {
            try
            {
                File.WriteAllBytes(fileName, bytes);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// File 轉 Bytes
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public byte[] FileToBytes1(string fileName)
        {
            return File.ReadAllBytes(fileName);
        }

        #endregion

        #region FileRead

        /// <summary>
        /// 逐筆讀取文字檔案的內容
        /// </summary>
        /// <param name="fileName"></param>
        public void FileToRead(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    Console.WriteLine(line);
            }
        }

        /// <summary>
        /// 使用非同步逐筆讀取文字檔案的內容
        /// </summary>
        /// <param name="fileName"></param>
        public async void FileToRead2(string fileName)
        {
            Char[] buffer;

            using (var sr = new StreamReader(fileName))
            {
                buffer = new Char[(int)sr.BaseStream.Length];
                await sr.ReadAsync(buffer, 0, (int)sr.BaseStream.Length);
            }

            Console.WriteLine(new String(buffer));
        }

        /// <summary>
        /// 逐筆讀取文字檔案的內容
        /// </summary>
        /// <param name="fileName"></param>
        public void FileToRead3(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                byte[] b = new byte[1024]; //這邊代表的是每行讀取的字元
                UTF8Encoding temp = new UTF8Encoding(true);
                while (fs.Read(b, 0, b.Length) > 0)
                    Console.WriteLine(temp.GetString(b));
            }
        }

        #endregion

        #region FileWriter

        /// <summary>
        /// 把資料寫入成文字檔
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public void DataWriterToFile(List<string> data, string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var dataItem in data)
                    sw.WriteLine(dataItem);
            }
        }

        /// <summary>
        /// 非同步把資料寫入成文字檔
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public async void DataWriterToFile1(List<string> data, string fileName)
        {
            // using (FileStream fs = File.Create(fileName))
            // using (FileStream fs = File.Open(fileName, FileMode.Open))
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                foreach (var dataItem in data)
                    await AddText(fs, dataItem);
            }
        }

        private async Task AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            //可指定讀取位置，從頭讀取設定為0
            fs.Seek(0, SeekOrigin.End);
            await fs.WriteAsync(info, 0, info.Length);
        }

        #endregion

       
    }
}