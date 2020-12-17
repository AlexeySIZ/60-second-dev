using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class GZip
{
    const int BUFFER_SIZE = 0x4000;

    public static void Compress(Stream inputStream, Stream outputStream)
    {
        byte[] buffer = new byte[BUFFER_SIZE];
        using var gzip = new GZipStream(outputStream, CompressionMode.Compress);
        int count;
        while ((count = inputStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
        {
            gzip.Write(buffer, 0, count);
        }
    }

    public static void Decompress(Stream inputStream, Stream outputStream)
    {
        byte[] buffer = new byte[BUFFER_SIZE];
        using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
        int count;
        while ((count = gzip.Read(buffer, 0, BUFFER_SIZE)) > 0)
        {
            outputStream.Write(buffer, 0, count);
        }
    }

    public static byte[] Compress(byte[] input)
    {
        using var inputStream = new MemoryStream(input);
        using var outputStream = new MemoryStream();
        Compress(inputStream, outputStream);
        return outputStream.ToArray();
    }

    public static byte[] Decompress(byte[] input)
    {
        using var inputStream = new MemoryStream(input);
        using var outputStream = new MemoryStream();
        Decompress(inputStream, outputStream);
        return outputStream.ToArray();
    }

    public static byte[] CompressString(string input)
        => Compress(Encoding.UTF8.GetBytes(input));
    public static string DecompressToString(byte[] input)
        => Encoding.UTF8.GetString(Decompress(input));

    public static void CompressFile(string inputFileName)
    {
        string outputFileName = $"{inputFileName}.gz";
        if (File.Exists(outputFileName))
        {
            throw new IOException($"File {outputFileName} already exists");
        }
        using var inputStream = File.OpenRead(inputFileName);
        using var outputStream = File.OpenWrite(outputFileName);
        Compress(inputStream, outputStream);
    }

    public static void DecompressFile(string inputFileName)
    {
        if (!inputFileName.EndsWith(".gz", ignoreCase: true,
            CultureInfo.InvariantCulture))
        {
            throw new ArgumentException("File must have .gz extension",
                nameof(inputFileName));
        }
        string outputFileName = inputFileName.Substring(0, inputFileName.Length - 3);
        if (File.Exists(outputFileName))
        {
            throw new IOException($"File {outputFileName} already exists");
        }
        using var inputStream = File.OpenRead(inputFileName);
        using var outputStream = File.OpenWrite(outputFileName);
        Decompress(inputStream, outputStream);
    }
}
