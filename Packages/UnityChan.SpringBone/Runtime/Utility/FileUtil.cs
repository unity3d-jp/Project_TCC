using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UTJ
{
    public class FileUtil
    {
    	// 全てのテキストを読み込む。文字コードを認識しようとする
    	public static string ReadAllText(string inFilePath, Encoding inDefaultEncoding)
    	{
            var outputText = "";
            var encoding = TryToDetectEncoding(inFilePath, inDefaultEncoding);
            try
            {
                outputText = File.ReadAllText(inFilePath, encoding);
            }
            catch (System.Exception exception)
            {
                Debug.LogError("ReadAllText failed\n" + inFilePath + "\n\n" + exception.ToString());
                outputText = "";
            }
            return outputText;
    	}
    
    	// 全てのテキストを読み込む。文字コードを認識しようとする
    	public static string ReadAllText(string inFilePath)
    	{
    		return ReadAllText(inFilePath, TryToDetectEncoding(inFilePath, Encoding.Default));
    	}
    
    	// 全てのテキスト行を読み込む。文字コードを認識しようとする
    	public static string[] ReadAllLines(string inFilePath, Encoding inDefaultEncoding)
    	{
            var outputLines = new string[0];
            var encoding = TryToDetectEncoding(inFilePath, inDefaultEncoding);
            try
            {
                outputLines = File.ReadAllLines(inFilePath, encoding);
            }
            catch (System.Exception exception)
            {
                Debug.LogError("ReadAllLines failed\n" + inFilePath + "\n\n" + exception.ToString());
                outputLines = new string[0];
            }
            return outputLines;
    	}
    	
    	// 全てのテキスト行を読み込む。文字コードを認識しようとする
    	public static string[] ReadAllLines(string inFilePath)
    	{
    		return ReadAllLines(inFilePath, TryToDetectEncoding(inFilePath, Encoding.Default));
    	}
    
        public static bool WriteAllText(string filePath, string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            var succeeded = false;
            try
            {
                System.IO.File.WriteAllText(filePath, text, encoding);
                succeeded = true;
            }
            catch (System.Exception exception)
            {
                Debug.LogError("保存失敗: " + filePath + "\n" + exception.ToString());
                succeeded = false;
            }
            return succeeded;
        }

    	// エンコードを認識しようとする
    	public static Encoding TryToDetectEncoding
    	(
    		string inFilePath,
    		Encoding inDefaultEncoding
    	)
    	{
    		// BOM
    		byte[] kUTF8Bom = { 0xEF, 0xBB, 0xBF };
    		byte[] kUTF16LEBom = { 0xFF, 0xFE };
    		byte[] kUTF16BEBom = { 0xFE, 0xFF };
    		// XMLヘッダー
    		byte[] kXMLUTF8Header = { 0x3C, 0x3F, 0x78, 0x6D }; // <?xm
    		byte[] kXMLUTF16LEHeader = { 0x3C, 0x00 }; // <?
    		byte[] kXMLUTF16BEHeader = { 0x00, 0x3C }; // <?
    
    		Dictionary<byte[], Encoding> kEncodingMap = new Dictionary<byte[], Encoding>();
    		kEncodingMap[kUTF8Bom] = Encoding.UTF8;
    		kEncodingMap[kUTF16LEBom] = Encoding.Unicode;
    		kEncodingMap[kUTF16BEBom] = Encoding.BigEndianUnicode;
    		kEncodingMap[kXMLUTF8Header] = Encoding.UTF8;
    		kEncodingMap[kXMLUTF16LEHeader] = Encoding.Unicode;
    		kEncodingMap[kXMLUTF16BEHeader] = Encoding.BigEndianUnicode;
    		const int kMaxHeaderLen = 4;
    
    		byte[] fileStart = ReadFirstBytesOfFile(inFilePath, kMaxHeaderLen);
    		foreach (var pair in kEncodingMap)
    		{
    			if (CheckIfBufferStartsWithHeader(fileStart, pair.Key))
    			{
    				return pair.Value;
    			}
    		}
    
    		return inDefaultEncoding;
    	}
    
    	// エンコードを認識しようとする
    	public static Encoding TryToDetectEncoding(string inFilePath)
    	{
    		return TryToDetectEncoding(inFilePath, Encoding.Default);
    	}

        public static void ExploreToDirectory(string directory)
        {
            // todo: support other OSes
#if UNITY_EDITOR || UNITY_WINDOWS
            directory = directory.Replace('/', '\\');
            if (directory.Length > 0 && Directory.Exists(directory))
            {
                var command = "explorer.exe";
                try
                {
                    System.Diagnostics.Process.Start(command, directory);
                }
                catch (System.Exception exception)
                {
                    Debug.LogError("フォルダーを表示できませんでした: " 
                        + directory + "\n\n" + exception.ToString());
                }
            }
#endif
        }
        
        // private

    	// ファイルの最初の数バイトを読み込む
    	private static byte[] ReadFirstBytesOfFile(string inFilePath, int inNumBytesToRead)
    	{
    		byte[] outBytes = new byte[inNumBytesToRead];
    		for (int iByte = 0; iByte < outBytes.Length; ++iByte)
    		{
    			outBytes[iByte] = 0;
    		}
    		
    		FileStream stream = null;
    		try
    		{
    			stream = new FileStream(inFilePath, FileMode.Open, FileAccess.Read);
    			stream.Read(outBytes, 0, inNumBytesToRead);
    		}
    		catch //(System.Exception ex)
    		{
    			//Debug.LogError(ex.ToString());
    		}
    
    		if (null != stream)
    		{
    			stream.Dispose();
    		}
    
    		return outBytes;
    	}
    
    	// バッファーが指定したヘッダーで始まるかどうか
    	private static bool CheckIfBufferStartsWithHeader(byte[] inBuffer, byte[] inHeader)
    	{
    		if (inBuffer.Length < inHeader.Length)
    		{
    			return false;
    		}
    
    		for (int iHeader = 0; iHeader < inHeader.Length; ++iHeader)
    		{
    			if (inBuffer[iHeader] != inHeader[iHeader])
    			{
    				return false;
    			}
    		}
    		return true;
    	}
    }
}
