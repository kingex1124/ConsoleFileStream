using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ConsoleFileStream
{
    public class StreamSample
    {
        public StreamSample()
        {

        }

        #region Stream

        public void StreamEX()
        {
            byte[] buffer = null;

            string testString = " Stream!Hello world ";
            char[] readCharArray = null;
            byte[] readBuffer = null;
            string readString = string.Empty;

            //關於MemoryStream我會在後續章節詳細闡述
            using (MemoryStream stream = new MemoryStream())
            {
                Console.WriteLine("初始字符串為：{0} ", testString);

                //如果該流可寫
                if (stream.CanWrite)
                {
                    //首先我們嘗試將testString寫入流中
                    //關於Encoding我會在另一篇文章中詳細說明，暫且通過它實現string->byte[]的轉換                    
                    buffer = Encoding.Default.GetBytes(testString);
                    //我們從該數組的第一個位置開始寫，長度為3，寫完之後stream中便有了數據
                    //對於新手來說很難理解的就是數據是什麼時候寫入到流中，在冗長的項目代碼面前，我碰見過很
                    //多新手都會有這種經歷，我希望能夠用如此簡單的代碼讓新手或者老手們在溫故下基礎                    
                    stream.Write(buffer, 0, 3);
                    Console.WriteLine("現在Stream.Postion在第{0}位置", stream.Position + 1);

                    //從剛才結束的位置（當前位置）往後移3位，到第7位
                    long newPositionInStream = stream.CanSeek ? stream.Seek(3, SeekOrigin.Current) : 0;

                    Console.WriteLine("重新定位後Stream.Postion在第{0}位置", newPositionInStream + 1);
                    if (newPositionInStream < buffer.Length)
                    {
                        //將從新位置（第7位）一直寫到buffer的末尾，注意下stream已經寫入了3個數據“Str”
                        stream.Write(buffer, (int)newPositionInStream, buffer.Length - (int)newPositionInStream);
                    }

                    //寫完後將stream的Position屬性設置成0，開始讀流中的數據
                    stream.Position = 0;

                    //設置一個空的盒子來接收流中的數據，長度根據stream的長度來決定
                    readBuffer = new byte[stream.Length];

                    //設置stream總的讀取數量，
                    //注意！這時候流已經把數據讀到了readBuffer中
                    int count = stream.CanRead ? stream.Read(readBuffer, 0, readBuffer.Length) : 0;

                    //由於剛開始時我們使用加密Encoding的方式,所以我們必須解密將readBuffer轉化成Char數組，這樣才能重新拼接成string 
                    //首先通過流讀出的readBuffer的數據求出從相應Char的數量
                    int charCount = Encoding.Default.GetCharCount(readBuffer, 0, count);
                    //通過該Char的數量設定一個新的readCharArray數組                    
                    readCharArray = new char[charCount];

                    // Encoding類的強悍之處就是不僅包含加密的方法，甚至將解密者都能創建出來（GetDecoder()），
                    //解密者便會將readCharArray填充（通過GetChars方法，把readBuffer逐個轉化將byte轉化成char，並且按一致順序填充到readCharArray中）
                    Encoding.Default.GetDecoder().GetChars(readBuffer, 0, count, readCharArray, 0);
                    for (int i = 0; i < readCharArray.Length; i++)
                        readString += readCharArray[i];

                    Console.WriteLine("讀取的字符串為：{0} ", readString);

                }
                stream.Close();
            }
            Console.ReadLine();
        }

        #endregion

        #region TextReader and StreamReader


        #region TextReader

        public void TextReaderEX()
        {
            string text = " abc\nabc ";

            using (TextReader reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    Console.WriteLine(" Peek = {0} ", (char)reader.Peek());
                    Console.WriteLine(" Read = {0} ", (char)reader.Read());
                }
                reader.Close();
            }

            using (TextReader reader = new StringReader(text))
            {
                char[] charBuffer = new char[3];
                int data = reader.ReadBlock(charBuffer, 0, 3);
                for (int i = 0; i < charBuffer.Length; i++)
                {
                    Console.WriteLine("通過readBlock讀出的數據：{0} ", charBuffer[i]);
                }
                reader.Close();
            }

            string stringlineData = string.Empty;

            using (TextReader reader = new StringReader(text))
            {
                stringlineData = reader.ReadLine();
                Console.WriteLine("第一行的數據為:{0} ", stringlineData);
                reader.Close();
            }

            using (TextReader reader = new StringReader(text))
            {
                string allData = reader.ReadToEnd();
                Console.WriteLine("全部的數據為:{0} ", allData);
                reader.Close();
            }

            Console.ReadLine();

        }

        #endregion

        #region StreamReader

        public void StreamReaderEX()
        {
            //文件地址
            string txtFilePath = " D:\\TextReader.txt ";

            //定義char數組
            char[] charBuffer2 = new char[3];

            //利用FileStream類將文件文本數據變成流然後放入StreamReader構造函數中
            using (FileStream stream = File.OpenRead(txtFilePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // StreamReader.Read()方法                    
                    DisplayResultStringByUsingRead(reader);
                }
            }
            using (FileStream stream = File.OpenRead(txtFilePath))
            {
                //使用Encoding.ASCII來嘗試下
                using (StreamReader reader = new StreamReader(stream, Encoding.ASCII, false))
                {
                    // StreamReader.ReadBlock()方法                    
                    DisplayResultStringByUsingReadBlock(reader);
                }
            }
            //嘗試用文件定位直接得到StreamReader，順便使用Encoding.Default 
            using (StreamReader reader = new StreamReader(txtFilePath, Encoding.Default, false, 123))
            {
                // StreamReader.ReadLine()方法              
                DisplayResultStringByUsingReadLine(reader);
            }
            //也可以通過File.OpenText方法直接獲取到StreamReader對象
            using (StreamReader reader = File.OpenText(txtFilePath))
            {
                // StreamReader.ReadLine()方法                
                DisplayResultStringByUsingReadLine(reader);
            }
            Console.ReadLine();
        }

        /// <summary> 
        ///使用StreamReader.Read()方法
        /// </summary> 
        /// <param name="reader"></param > 
        private void DisplayResultStringByUsingRead(StreamReader reader)
        {
            int readChar = 0;
            string result = string.Empty;

            while ((readChar = reader.Read()) != -1)
                result += (char)readChar;

            Console.WriteLine("使用StreamReader.Read()方法得到Text文件中的數據為: {0} ", result);
        }

        /// <summary> 
        ///使用StreamReader.ReadBlock()方法
        /// </summary> 
        /// < param name="reader"></param> 
        public void DisplayResultStringByUsingReadBlock(StreamReader reader)
        {
            char[] charBuffer = new char[10];
            string result = string.Empty;

            reader.ReadBlock(charBuffer, 0, 10);

            for (int i = 0; i < charBuffer.Length; i++)
                result += charBuffer[i];

            Console.WriteLine("使用StreamReader.ReadBlock()方法得到Text文件中前10個數據為: {0} ", result);
        }

        /// <summary> 
        /// 使用StreamReader.ReadLine()方法
        /// </summary> 
        /// <param name="reader"></param> 
        public void DisplayResultStringByUsingReadLine(StreamReader reader)
        {
            int i = 1;
            string resultString = string.Empty;
            while ((resultString = reader.ReadLine()) != null)
            {
                Console.WriteLine("使用StreamReader.Read()方法得到Text文件中第{1}行的數據為: {0} ", resultString, i);
                i++;
            }
        }

        #endregion

        #endregion

        #region TextWriter 和StreamWriter

        #region StreamWriter

        public void StreamWriterEX()
        {
            NumberFormatInfo numberFomatProvider = new NumberFormatInfo();
            //將小數“.”換成"?"
            numberFomatProvider.PercentDecimalSeparator = " ? ";
            StreamWriterTest test = new StreamWriterTest(Encoding.Default, txtFilePath, numberFomatProvider);
            // StreamWriter
            test.WriteSomthingToFile();
            // TextWriter
            test.WriteSomthingToFileByUsingTextWriter();
            Console.ReadLine();
        }

        const string txtFilePath = " D:\\TextWriter.txt ";

        ///  <summary> 
        ///   TextWriter和StreamWriter的舉例
        /// </summary> 
        public class StreamWriterTest
        {
            /// <summary> 
            ///編碼
            /// </summary> 
            private Encoding _encoding;
            /// <summary> 
            /// IFomatProvider 
            /// </summary> 
            private IFormatProvider _provider;
            /// <summary> 
            ///文件路徑
            /// </summary> 
            private string _textFilePath;

            public StreamWriterTest(Encoding encoding, string textFilePath) : this(encoding, textFilePath, null)
            {

            }

            public StreamWriterTest(Encoding encoding, string textFilePath, IFormatProvider provider)
            {
                this._encoding = encoding;
                this._textFilePath = textFilePath;
                this._provider = provider;
            }

            ///  <summary> 
            ///  我們可以通過FileStream或者文件路徑直接對該文件進行寫操作
            ///  </summary> 
            public void WriteSomthingToFile()
            {
                //獲取FileStream 
                using (FileStream stream = File.OpenWrite(_textFilePath))
                {
                    //獲取StreamWriter 
                    using (StreamWriter writer = new StreamWriter(stream, this._encoding))
                    {
                        this.WriteSomthingToFile(writer);
                    }

                    //也可以通過文件路徑和設置bool append，編碼和緩衝區來構建一個StreamWriter對象
                    using (StreamWriter writer = new StreamWriter(_textFilePath, true, this._encoding, 20))
                    {
                        this.WriteSomthingToFile(writer);
                    }
                }
            }

            ///  <summary> 
            ///  具體寫入文件的邏輯
            /// </summary> 
            /// <param name="writer"> StreamWriter對象</param> 
            public void WriteSomthingToFile(StreamWriter writer)
            {
                //需要寫入的數據
                string[] writeMethodOverloadType = {
                " 1.Write(bool); " ,
                " 2.Write(char); " ,
                " 3.Write(Char[]) " ,
                " 4.Write(Decimal) ",
                " 5.Write(Double) " ,
                " 6.Write(Int32) " ,
                " 7.Write(Int64) " ,
                " 8.Write(Object) " ,
                " 9.Write(Char[]) " ,
                " 10.Write (Single) " ,
                " 11.Write(Char[]) " ,
                " 12.Write(String) " ,
                " 13Write(UInt32) " ,
                " 14.Write(string format,obj) " ,
                " 15.Write(Char []) "
            };

                //定義writer的AutoFlush屬性，如果定義了該屬性，就不必使用writer.Flush方法
                writer.AutoFlush = true;
                writer.WriteLine("這個StreamWriter使用了{0}編碼", writer.Encoding.HeaderName);
                //這裡重新定位流的位置會導致一系列的問題
                // writer.BaseStream.Seek( 1, SeekOrigin.Current);
                writer.WriteLine("這裡簡單演示下StreamWriter.Writer方法的各種重載版本");

                writeMethodOverloadType.ToList().ForEach
                (
                    (name) => { writer.WriteLine(name); }
                );

                writer.WriteLine(" StreamWriter.WriteLine()方法就是在加上行結束符，其餘和上述方法是用一致");
                //writer.Flush();
                writer.Close();
            }

            public void WriteSomthingToFileByUsingTextWriter()
            {
                using (TextWriter writer = new StringWriter(_provider))
                {
                    writer.WriteLine("這裡簡單介紹下TextWriter怎麼使用用戶設置的IFomatProvider，假設用戶設置了NumberFormatInfoz.PercentDecimalSeparator屬性");
                    writer.WriteLine("看下區別吧{0:p10} ", 0.12);
                    Console.WriteLine(writer.ToString());
                    writer.Flush();
                    writer.Close();
                }

            }
        }

        #endregion

        #endregion

        #region FileStream

        #region 整包傳輸

        public void FileStreamEX()
        {
            FileStreamTest test = new FileStreamTest();
            //創建文件配置類
            CreateFileConfig createFileConfig = new CreateFileConfig { CreateUrl = @" d:\MyFile.txt ", IsAsync = true };
            //複製文件配置類
            CopyFileConfig copyFileConfig = new CopyFileConfig
            {
                OrginalFileUrl = @" d:\8.jpg ",
                DestinationFileUrl = @" d:\9.jpg ",
                IsAsync = true
            };
            test.Create(createFileConfig);
            test.Copy(copyFileConfig);
        }

        //首先我們嘗試DIY一個IFileConfig
        /// <summary> 
        /// 文件配置接口
        /// </summary> 
        public interface IFileConfig 
        { 
            string FileName { get; set; } 
            bool IsAsync { get; set; } 
        }

        // 創建文件配置類CreateFileConfig，用於添加文件一些配置設置，實現添加文件的操作
        /// <summary> 
        /// 創建文件配置類
        /// </summary> 
        public class CreateFileConfig : IFileConfig
        {
            //文件名
            public string FileName { get; set; }
            //是否異步操作
            public bool IsAsync { get; set; }
            //創建文件所在url 
            public string CreateUrl { get; set; }
        }

        //讓我們定義一個文件流測試類：FileStreamTest 來實現文件的操作
        ///  <summary> 
        /// FileStreamTest類
        /// </summary> 
        public class FileStreamTest
        {
            private object _lockObject;

            //在該類中實現一個簡單的Create方法用來同步或異步的實現添加文件，FileStream會根據配置類去選擇相應的構造函數，實現異步或同步的添加方式
            ///  <summary> 
            ///添加文件方法
            /// </summary> 
            /// <param name="config">創建文件配置類</param> 
            public void Create(IFileConfig config)
            {
                lock (_lockObject)
                {
                    //得到創建文件配置類對象
                    var createFileConfig = config as CreateFileConfig;
                    //檢查創建文件配置類是否為空
                    if (this.CheckConfigIsError(config))
                        return;
                    //假設創建完文件後寫入一段話，實際項目中無需這麼做，這裡只是一個演示
                    char[] insertContent = " HellowWorld ".ToCharArray();

                    //轉化成byte[] 
                    byte[] byteArrayContent = Encoding.Default.GetBytes(insertContent, 0, insertContent.Length);

                    //根據傳入的配置文件中來決定是否同步或異步實例化stream對象                
                    FileStream stream = createFileConfig.IsAsync ? 
                        new FileStream(createFileConfig.CreateUrl, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, true) : 
                        new FileStream(createFileConfig.CreateUrl, FileMode.Create);

                    using (stream)
                    {
                        //如果不註釋下面代碼會拋出異常，google上提示是WriteTimeout只支持網絡流//
                        // stream.WriteTimeout = READ_OR_WRITE_TIMEOUT;

                        //如果該流是同步流並且可寫
                        if (!stream.IsAsync && stream.CanWrite)
                            stream.Write(byteArrayContent, 0, byteArrayContent.Length);
                        else if (stream.CanWrite) //異步流並且可寫                        
                            stream.BeginWrite(byteArrayContent, 0, byteArrayContent.Length, this.End_CreateFileCallBack, stream); 
                        
                        stream.Close();
                    }
                }
            }

            private bool CheckConfigIsError(IFileConfig config)
            {
                throw new NotImplementedException();
            }

            // 如果採用異步的方式則最後會進入End_CreateFileCallBack回調方法，result.AsyncState對象就是上圖stream.BeginWrite()方法的最後一個參數
            // 還有一點必須注意的是每一次使用BeginWrite()方法事都要帶上EndWrite()方法，Read方法也一樣
            ///  <summary> 
            ///  異步寫文件callBack方法
            /// </summary> 
            /// <param name="result"> IAsyncResult </param> 
            private void End_CreateFileCallBack(IAsyncResult result)
            {
                //從IAsyncResult對象中得到原來的FileStream 
                var stream = result.AsyncState as FileStream;
                //結束異步寫            
                Console.WriteLine("異步創建文件地址：{0} ", stream.Name);
                stream.EndWrite(result);
                Console.ReadLine();
            }

            // 然後在FileStreamTest 類中新增一個Copy方法實現文件的複制功能
            ///  <summary> 
            ///複製方法
            /// </summary> 
            /// <param name="config">拷貝文件複製</param> 
            public void Copy(IFileConfig config)
            {
                lock (_lockObject)
                {
                    //得到CopyFileConfig對象
                    var copyFileConfig = config as CopyFileConfig;

                    //檢查CopyFileConfig類對像是否為空或者OrginalFileUrl是否為空
                    if (CheckConfigIsError(copyFileConfig) || !File.Exists(copyFileConfig.OrginalFileUrl))
                        return;

                    //創建同步或異步流                
                    FileStream stream = copyFileConfig.IsAsync ?
                        new FileStream(copyFileConfig.OrginalFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true) : 
                        new FileStream(copyFileConfig.OrginalFileUrl, FileMode.Open);

                    //定義一個byte數組接受從原文件讀出的byte數據
                    byte[] orignalFileBytes = new byte[stream.Length];

                    using (stream)
                    {
                        // stream.ReadTimeout = READ_OR_WRITE_TIMEOUT; 
                        //如果異步流
                        if (stream.IsAsync)
                        {
                            //將該流和讀出的byte[]數據放入配置類，在callBack中可以使用                        
                            copyFileConfig.OriginalFileStream = stream;
                            copyFileConfig.OriginalFileBytes = orignalFileBytes;

                            if (stream.CanRead) //異步開始讀取，讀完後進入End_ReadFileCallBack方法，該方法接受copyFileConfig參數
                                stream.BeginRead(orignalFileBytes, 0, orignalFileBytes.Length, End_ReadFileCallBack, copyFileConfig);
                        }
                        else //否則同步讀取
                        {
                            if (stream.CanRead)
                            {
                                //一般讀取原文件
                                stream.Read(orignalFileBytes, 0, orignalFileBytes.Length);
                            }
                            //定義一個寫流，在新位置中創建一個文件
                            FileStream copyStream = new FileStream(copyFileConfig.DestinationFileUrl, FileMode.CreateNew);
                            using (copyStream)
                            {
                                //   copyStream.WriteTimeout = READ_OR_WRITE_TIMEOUT; 
                                //將源文件的內容寫進新文件
                                copyStream.Write(orignalFileBytes, 0, orignalFileBytes.Length);
                                copyStream.Close();
                            }
                        }
                        stream.Close();
                        Console.ReadLine();
                    }
                }
            }

            // 最後，如果採用異步的方式，則會進入End_ReadFileCallBack回調函數進行異步讀取和異步寫操作
            ///  <summary> 
            ///異步讀寫文件方法
            /// </summary> 
            /// <param name="result"></param> 
            private void End_ReadFileCallBack(IAsyncResult result)
            {
                //得到先前的配置文件
                var config = result.AsyncState as CopyFileConfig;
                //結束異步讀            
                config.OriginalFileStream.EndRead(result);

                //異步讀後立即寫入新文件地址
                if (File.Exists(config.DestinationFileUrl))
                    File.Delete(config.DestinationFileUrl);

                FileStream copyStream = new FileStream(config.DestinationFileUrl, FileMode.CreateNew);
                using (copyStream)
                {
                    Console.WriteLine("異步複製原文件地址：{0} ", config.OriginalFileStream.Name);
                    Console.WriteLine("複製後的新文件地址：{0} ", config.DestinationFileUrl);
                    //調用異步寫方法CallBack方法為End_CreateFileCallBack，參數是copyStream
                    copyStream.BeginWrite(config.OriginalFileBytes, 0, config.OriginalFileBytes.Length, this.End_CreateFileCallBack, copyStream);
                    copyStream.Close();

                }
            }


        }

        // 文件複製的方式思路比較相似，首先定義復製文件配置類，由於在異步回調中用到該配置類的屬性，所以新增了文件流對象和相應的字節數組
        /// <summary> 
        /// 文件複製
        /// </summary> 
        public class CopyFileConfig : IFileConfig
        {
            //文件名
            public string FileName { get; set; }
            //是否異步操作
            public bool IsAsync { get; set; }
            //原文件地址
            public string OrginalFileUrl { get; set; }
            //拷貝目的地址
            public string DestinationFileUrl { get; set; }
            //文件流，異步讀取後在回調方法內使用
            public FileStream OriginalFileStream { get; set; }
            //原文件字節數組，異步讀取後在回調方法內使用
            public byte[] OriginalFileBytes { get; set; }
        }

        #endregion

        #region DIY一個簡單的分段傳輸的例子

        public void FileStreamEX1()
        {
            UpFileSingleTest test = new UpFileSingleTest();
            FileInfo info = new FileInfo(@" G:\\Skyrim\20080204173728108.torrent ");
            //取得文件總長度
            var fileLegth = info.Length;
            //假設將文件切成5段
            var divide = 5;
            //取到每個文件段的長度
            var perFileLengh = (int)fileLegth / divide;
            //表示最後剩下的文件段長度比perFileLengh小
            var restCount = (int)fileLegth % divide;

            //循環上傳數據
            for (int i = 0; i < divide + 1; i++)
            {
                //每次定義不同的數據段,假設數據長度是500，那麼每段的開始位置都是i*perFileLength 
                var startPosition = i * perFileLengh;
                //取得每次數據段的數據量                                                       // 剩下的量
                var totalCount = fileLegth - perFileLengh * i > perFileLengh ? perFileLengh : (int)(fileLegth - perFileLengh * i);
                //上傳該段數據                
                test.UpLoadFileFromLocal(@" G:\\Skyrim\\20080204173728108.torrent ", @" G:\\Skyrim\\20080204173728109.torrent ", startPosition, i == divide ? divide : totalCount);

            }
        }

        ///  <summary> 
        ///分段上傳例子
        /// </summary> 
        public class UpFileSingleTest
        {
            //我們定義Buffer為1000 
            public const int BUFFER_COUNT = 1000;

            /// <summary> 
            ///將文件上傳至服務器（本地），由於採取分段傳輸所以，
            ///每段必須有一個起始位置和相對應該數據段的數據
            /// </summary> 
            /// <param name="filePath">服務器上文件地址</param> 
            /// <param name="startPositon">分段起始位置</param> 
            /// <param name="btArray">每段的數據</param>
            private void WriteToServer(string filePath, int startPositon, byte[] btArray)
            {
                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                using (fileStream)
                {
                    //將流的位置設置在該段起始位置
                    fileStream.Position = startPositon;
                    //將該段數據通過FileStream寫入文件中，每次寫一段的數據，就好比是個水池，分段蓄水一樣，直到蓄滿為止
                    fileStream.Write(btArray, 0, btArray.Length);
                }
            }


            ///  <summary> 
            ///處理單獨一段本地數據上傳至服務器的邏輯，根據客戶端傳入的startPostion 
            ///和totalCount來處理相應段的數據上傳至服務器（本地）
            ///  </summary> 
            /// <param name="localFilePath">本地需要上傳的文件地址</param> 
            /// <param name=" uploadFilePath">服務器（本地）目標地址</param> 
            /// <param name="startPostion">該段起始位置</param> 
            /// <param name="totalCount">該段最大數據量< /param> 
            public void UpLoadFileFromLocal(string localFilePath, string uploadFilePath, int startPostion, int totalCount)
            {
                // if(!File.Exists(localFilePath)){return;} //每次臨時讀取數據數

                int tempReadCount = 0;
                int tempBuffer = 0;

                //定義一個緩衝區數組
                byte[] bufferByteArray = new byte[BUFFER_COUNT];

                //定義一個FileStream對象            
                FileStream fileStream = new FileStream(localFilePath, FileMode.Open);

                //將流的位置設置在每段數據的初始位置            
                fileStream.Position = startPostion;

                using (fileStream)
                {
                    //循環將該段數據讀出在寫入服務器中
                    while (tempReadCount < totalCount)
                    {
                        tempBuffer = BUFFER_COUNT;

                        //每段起始位置+每次循環讀取數據的長度
                        var writeStartPosition = startPostion + tempReadCount;

                        //當緩衝區的數據加上臨時讀取數大於該段數據量時，
                        //則設置緩衝區的數據為totalCount-tempReadCount這一段的數據
                        if (tempBuffer + tempReadCount > totalCount)
                        {
                            //緩衝區的數據為totalCount-tempReadCount                         
                            tempBuffer = totalCount - tempReadCount;
                            //讀取該段數據放入bufferByteArray數組中                        
                            fileStream.Read(bufferByteArray, 0, tempBuffer);
                            if (tempBuffer > 0)
                            {
                                byte[] newTempBtArray = new byte[tempBuffer];
                                Array.Copy(bufferByteArray, 0, newTempBtArray, 0, tempBuffer);
                                //將緩衝區的數據上傳至服務器
                                this.WriteToServer(uploadFilePath, writeStartPosition, newTempBtArray);
                            }
                        }
                        //如果緩衝區的數據量小於該段數據量，並且tempBuffer=設定BUFFER_COUNT時，通過
                        // while循環每次讀取一樣的buffer值的數據寫入服務器中，直到將該段數據全部處理完畢
                        else if (tempBuffer == BUFFER_COUNT)
                        {
                            fileStream.Read(bufferByteArray, 0, tempBuffer);
                            this.WriteToServer(uploadFilePath, writeStartPosition, bufferByteArray);
                        }
                        //通過每次的緩衝區數據，累計增加臨時讀取數
                        tempReadCount += tempBuffer;
                    }
                }
            }

        }

        #endregion

        #endregion

        #region MemoryStream

        #region OutOfMemory

        public void OutOfMemoryEX()
        {
            //測試byte數組假設該數組容量是256M 
            byte[] testBytes = new byte[256 * 1024 * 1024];
            MemoryStream ms = new MemoryStream();
            using (ms)
            {
                for (int i = 0; i < 1000; i++)
                {
                    try
                    {
                        ms.Write(testBytes, 0, testBytes.Length);
                    }
                    catch
                    {
                        Console.WriteLine("該內存流已經使用了{0}M容量的內存,該內存流最大容量為{1}M,溢出時容量為{2}M ",
                            GC.GetTotalMemory(false) / (1024 * 1024), // MemoryStream已經消耗內存量
                            ms.Capacity / (1024 * 1024), // MemoryStream最大的可用容量
                            ms.Length / (1024 * 1024)); // MemoryStream當前流的長度（容量）
                        break;
                    }
                }
            }

            Console.ReadLine();
        }

        #endregion

        #region XmlWriter中使用MemoryStream

        ///  <summary> 
        ///演示在xmlWriter中使用MemoryStream
        ///  </summary> 
        public void UseMemoryStreamInXMLWriter()
        {
            MemoryStream ms = new MemoryStream();
            using (ms)
            {
                //定義一個XMLWriter 
                using (XmlWriter writer = XmlWriter.Create(ms))
                {
                    //寫入xml頭
                    writer.WriteStartDocument(true);
                    //寫入一個元素
                    writer.WriteStartElement("Content");
                    //為這個元素新增一個test屬性
                    writer.WriteStartAttribute("test");
                    //設置test屬性的值
                    writer.WriteValue("逆時針的風");
                    //釋放緩衝，這裡可以不用釋放，但是在實際項目中可能要考慮部分釋放對性能帶來的提升
                    writer.Flush();
                    Console.WriteLine("此時內存使用量為:{2}KB,該MemoryStream的已經使用的容量為{0}byte,默認容量為{1}byte ",
                        Math.Round((double)ms.Length, 4), ms.Capacity, GC.GetTotalMemory(false) / 1024);
                    Console.WriteLine("重新定位前MemoryStream所在的位置是{0} ", ms.Position);
                    //將流中所在的當前位置往後移動7位，相當於空格
                    ms.Seek(7, SeekOrigin.Current);
                    Console.WriteLine("重新定位後MemoryStream所在的位置是{0} ", ms.Position);
                    //如果將流所在的位置設置為如下所示的位置則xml文件會被打亂
                    // ms.Position = 0; 
                    writer.WriteStartElement("Content2");
                    writer.WriteStartAttribute("testInner");
                    writer.WriteValue("逆時針的風Inner ");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    //再次釋放
                    writer.Flush();
                    Console.WriteLine("此時內存使用量為:{2}KB,該MemoryStream的已經使用的容量為{0}byte,默認容量為{1}byte ",
                        Math.Round((double)ms.Length, 4), ms.Capacity, GC.GetTotalMemory(false) / 1024);
                    //建立一個FileStream文件創建目的地是d:\test.xml 
                    FileStream fs = new FileStream(@" d:\test.xml ", FileMode.OpenOrCreate);
                    using (fs)
                    {
                        //將內存流注入FileStream 
                        ms.WriteTo(fs);
                        if (ms.CanWrite)
                            //釋放緩衝區
                            fs.Flush();
                    }
                }
            }
        }

        #endregion

        #region 自定義一個處理圖片的HttpHandler

        public class ImageHandler : IHttpHandler
        {
            #region IHttpHandler Members

            public bool IsReusable
            {
                get { return true; }
            }

            ///  <summary> 
            ///實現IHTTPHandler後必須實現的方法
            ///  </summary> 
            ///  <param name="context"> HttpContext上下文</param> 
            public void ProcessRequest(HttpContext context)
            {
                context.Response.Clear();
                //得到圖片名
                var imageName = context.Request["ImageName"] == null ? "逆時針的風"
                    : context.Request["ImageName"].ToString();
                //得到圖片ID,這裡只是演示，實際項目中不是這麼做的
                var id = context.Request["Id"] == null ? " 01 "
                    : context.Request["Id"].ToString();
                //得到圖片地址
                var stringFilePath = context.Server.MapPath(string.Format(" ~/Image/{0}{1}.jpg ", imageName, id));
                //聲明一個FileStream用來將圖片暫時放入流中
                FileStream stream = new FileStream(stringFilePath, FileMode.Open);
                using (stream)
                {
                    //透過GetImageFromStream方法將圖片放入byte數組中
                    byte[] imageBytes = this.GetImageFromStream(stream, context);
                    //上下文確定寫到客戶短時的文件類型
                    context.Response.ContentType = " image/jpeg ";
                    //上下文將imageBytes中的數據寫到前段
                    context.Response.BinaryWrite(imageBytes);
                    stream.Close();
                }
            }

            ///  <summary> 
            ///將流中的圖片信息放入byte數組後返回該數組
            ///  </summary> 
            ///  <param name="stream">文件流</param> 
            ///  <param name="context">上下文</param> 
            ///  <returns></returns> 
            private byte[] GetImageFromStream(FileStream stream, HttpContext context)
            {
                //通過stream得到Image 
                Image image = Image.FromStream(stream);
                //加上水印
                image = SetWaterImage(image, context);
                //得到一個ms對象
                MemoryStream ms = new MemoryStream();
                using (ms)
                {
                    //將圖片保存至內存流
                    image.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = new byte[ms.Length];
                    ms.Position = 0;
                    //通過內存流讀取到imageBytes 
                    ms.Read(imageBytes, 0, imageBytes.Length);
                    ms.Close();
                    //返回imageBytes 
                    return imageBytes;
                }
            }
            ///  <summary> 
            ///為圖片加上水印，這個方法不用在意，只是演示，所以沒加透明度
            ///下次再加上吧
            ///  </summary> 
            ///  <param name= "image">需要加水印的圖片</param> 
            ///  <param name="context">上下文</param> 
            ///  <returns></returns> 
            private Image SetWaterImage(Image image, HttpContext context)
            {
                Graphics graphics = Graphics.FromImage(image);
                Image waterImage = Image.FromFile(context.Server.MapPath(" ~/Image/逆時針的風01.jpg "));
                //在大圖右下角畫上水印圖就行
                graphics.DrawImage(waterImage,
                     new Point
                     {
                         X = image.Size.Width - waterImage.Size.Width,
                         Y = image.Size.Height - waterImage.Size.Height
                     });
                return image;
            }

            #endregion

            #region Web.config 要加入的東西

            //< httpHandlers > 
            //   < add type = "ImageHandler.ImageHandler,ImageHandler"   verb ="*" path ="ImageHandler.apsx" /> 
            //</ httpHandlers >

            #endregion

            #region 前台使用

            //< asp:Content ID = "BodyContent" runat ="server" ContentPlaceHolderID ="MainContent" > 
            // < h2 >
            //     About
            // </ h2> 
            // < p >
            //     Put content here.
            //     < asp:Image runat = "server" ImageUrl ="ImageHandler.apsx?ImageName=逆時針的風&Id=02"  /> 
            // </ p > 
            //</ asp:Content >

            #endregion

        }

        #endregion

        #endregion

        #region BufferedStream

        #endregion
    }
}
