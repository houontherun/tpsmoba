using System;
using System.IO;
using System.Text;

namespace PsdLayoutTool
{
    public class Chunk
    {
        public int Length { get; private set; }
        public int Type { get; private set; }
        public byte[] Data { get; private set; }

        public Chunk(int length, int type, byte[] data)
        {
            Length = length;
            Type = type;
            Data = data;
        }
    }
    public enum ColorModel : byte
    {
        Gray = 0x00,
        Palette = 0x01,
        Color = 0x02,
        Alpha = 0x04
    }
    public class PngHeader
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte BitDepth { get; private set; }
        public ColorModel ColorModel { get; private set; }

        public int BytesPerPixel { get; private set; }
        public int Stride { get; private set; }

        public PngHeader(int width, int height, byte bitdepth, ColorModel colorModel)
        {
            Width = width;
            Height = height;
            BitDepth = bitdepth;
            ColorModel = colorModel;

            bool alpha = (colorModel & ColorModel.Alpha) == ColorModel.Alpha;
            bool palette = (colorModel & ColorModel.Palette) == ColorModel.Palette;
            bool grayscale = (colorModel & ColorModel.Color) != ColorModel.Color;
            bool packed = bitdepth < 8;

            if (grayscale && palette)
                throw new ArgumentOutOfRangeException("palette and greyscale are exclusive");

            int channels = (grayscale || palette) ?
                    (alpha ? 2 : 1) :   // Grayscale or Palette
                    (alpha ? 4 : 3);    // RGB 

            int bpp = channels * BitDepth;
            BytesPerPixel = (bpp + 7) / 8;
            Stride = (bpp * width + 7) / 8;
            int samplesPerRow = channels * width;
            int samplesPerRowPacked = (packed) ? Stride : samplesPerRow;
        }        
    }
    public class PngHelper
    {
        public static string IHDR = "IHDR";
        public static string IDAT = "IDAT";
        public static string IEND = "IEND";
        public static string SBIT = "sBIT";
        public static string SRGB = "sRGB";
        public static string TIME = "tIME";
        public static string PLTE = "PLTE";

        public static long Signature = 0x0a1a0a0d474e5089;

        public static int BlockSize = 8192;
        public static int CompressionLevel = 9;

        private static Encoding encoding = Encoding.GetEncoding("ISO-8859-1");

        public static PngHeader ConvertIHDR(Chunk chunk)
        {
            byte[] data = chunk.Data;

            int ofs = 0;
            int columns = DWordToInt(BitConverter.ToInt32(data, ofs)); ofs += 4;
            int rows = DWordToInt(BitConverter.ToInt32(data, ofs)); ofs += 4;

            // bit depth: number of bits per channel
            byte bitdepth = data[ofs++];
            byte colormodel = data[ofs++];

            byte compmeth = data[ofs++];
            byte filmeth = data[ofs++];

            byte interlaced = data[ofs++];
            if (interlaced > 0)
                throw new NotSupportedException("interlaced png");

            return new PngHeader(columns, rows, bitdepth, (ColorModel)colormodel);
        }
        
        public static int DWordToInt(int dword)
        {
            byte[] bytes = BitConverter.GetBytes(dword);
            return (bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3];
        }
        
        public static int TxtToInt(string txt)
        {
            return BitConverter.ToInt32(encoding.GetBytes(txt), 0);
        }        
    }
    public class PngBinaryReader : BinaryReader
    {
        public PngBinaryReader(Stream input) : base(input)
        {
        }

        public void ReadSignature()
        {
            long signatur = ReadInt64();
            if (signatur != PngHelper.Signature)
                throw new InvalidDataException("Wrong PNG Signature");
        }

        public int ReadDWord()
        {
            return PngHelper.DWordToInt(ReadInt32());
        }        

        public Chunk ReadChunk()
        {
            int dataLength = ReadDWord();
            int type = ReadInt32();
            byte[] data = ReadBytes(dataLength);
            ReadDWord();
            //if (ReadDWord() != (int)Crc32.Calc(type, data))
            //    throw new InvalidDataException("Checksum error!");
            return new Chunk(dataLength, type, data);
        }
    }

    public class Png
    {
        public PngHeader header { get; private set; }

        public Chunk dataChunk { get; private set; }

        public Png(string fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.Open))
            {
                using (PngBinaryReader reader = new PngBinaryReader(stream))
                {
                    reader.ReadSignature();

                    // Header lesen
                    Chunk chunk = reader.ReadChunk();
                    if ((chunk.Length != 13) || (chunk.Type != PngHelper.TxtToInt(PngHelper.IHDR)))
                    {
                        throw new InvalidDataException("Wrong IHDR Chunk!");
                    }
                    header = PngHelper.ConvertIHDR(chunk);

                    using (MemoryStream outMemoryStream = new MemoryStream())
                    {
                        // Daten Stream bis zum Ende
                        while (chunk.Type != PngHelper.TxtToInt(PngHelper.IEND))
                        {
                            if (chunk.Type == PngHelper.TxtToInt(PngHelper.IDAT))
                            {
                                outMemoryStream.Write(chunk.Data, 0, chunk.Data.Length);
                                dataChunk = chunk;
                            }

                            chunk = reader.ReadChunk();
                        }
                    }
                }
            }
        }

    }
    public class PngUtil
    {
        public static bool IsSamePng(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                return false;
            }
            Png p1 = new Png(file1);
            Png p2 = new Png(file2);

            bool same = isSameHeader(p1.header, p2.header);
            if (!same)
            {
                return false;
            }
            return isSameData(p1.dataChunk.Data, p2.dataChunk.Data);
        }
        public static bool IsSamePng(Png p1, Png p2)
        {
            return isSameHeader(p1.header, p2.header) && isSameData(p1.dataChunk.Data, p2.dataChunk.Data);
        }
        static bool isSameHeader(PngHeader h1, PngHeader h2)
        {
            return (
                h1.Width == h2.Width && 
                h1.Height == h2.Height &&
                h1.BitDepth == h2.BitDepth &&
                h1.ColorModel == h2.ColorModel
                );
        }

        static bool isSameData(byte[] d1, byte[] d2)
        {
            if (d1 == null || d2 == null)
            {
                return false;
            }
            if (d1.LongLength != d2.LongLength)
            {
                return false;
            }
            int space = 15; // 3*5
            for (int i = 0; i < d1.LongLength; i += space)
            {
                if (d1[i] != d2[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
