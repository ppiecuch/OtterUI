using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace Otter.Imaging
{
    class PCXHeader
    {
        public byte Manufacturer { get; set; }
        public byte Version { get; set; }
        public byte Encoding { get; set; }
        public byte BitsPerPixel { get; set; }
        public short LeftMargin { get; set; }
        public short UpperMargin { get; set; }
        public short RightMargin { get; set; }
        public short LowerMargin { get; set; }
        public short Horiz { get; set; }
        public short Vertic { get; set; }
        public byte[] Palette { get; set; }
        public byte Unused1 { get; set; }
        public byte ColourPlanes { get; set; }
        public short BytesPerScanline { get; set; }
        public short PaletteInfo { get; set; }
        public short HorizontalScreenSize { get; set; }
        public short VerticalScreenSize { get; set; }
    }

    public class PCXLoader
    {
        private static PCXHeader ReadHeader(Stream input)
        {
            BinaryReader reader = new BinaryReader(input);

            PCXHeader header = new PCXHeader();

            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            header.Manufacturer = reader.ReadByte();
            header.Version = reader.ReadByte();
            header.Encoding = reader.ReadByte();
            header.BitsPerPixel = reader.ReadByte();
            header.LeftMargin = reader.ReadInt16();
            header.UpperMargin = reader.ReadInt16();
            header.RightMargin = reader.ReadInt16();
            header.LowerMargin = reader.ReadInt16();
            header.Horiz = reader.ReadInt16();
            header.Vertic = reader.ReadInt16();
            header.Palette = reader.ReadBytes(48);
            header.Unused1 = reader.ReadByte();
            header.ColourPlanes = reader.ReadByte();
            header.BytesPerScanline = reader.ReadInt16();
            header.PaletteInfo = reader.ReadInt16();
            header.HorizontalScreenSize = reader.ReadInt16();
            header.VerticalScreenSize = reader.ReadInt16();

            return header;
        }

        private static bool IsPaletteThere(Stream file)
        {
            bool bRet = false;

            BinaryReader r = new BinaryReader(file);

            long pos = r.BaseStream.Position;
            r.BaseStream.Seek(-769, SeekOrigin.End);

            bRet = (r.ReadByte() == 0x0c);

            r.BaseStream.Seek(pos, SeekOrigin.Begin);
            return bRet;
        }

        private static byte[] GetPalette(Stream file)
        {
            BinaryReader r = new BinaryReader(file);
            long pos = r.BaseStream.Position;
            r.BaseStream.Seek(-768, SeekOrigin.End);

            byte[] pal = new byte[768];

            for (int i = 0; i < 768; i++)
            {
                pal[i] = r.ReadByte();
            }

            r.BaseStream.Seek(pos, SeekOrigin.Begin);
            return pal;
        }

        private static void GetScanline(BinaryReader br, ref byte[] scanline)
        {
            int i = 0;
            while (i < scanline.Length)
            {
                // read a byte!
                int data = br.ReadByte();
                if (data == -1) 
                    throw new ApplicationException("Invalid data");

                if ((data & 0xC0) != 0xC0)
                {  
                    // non RLE
                    scanline[i++] = (byte)data;
                }
                else
                {
                    // RLE
                    // read the repeated byte
                    int numbytes = data & 0x3F;
                    data = br.ReadByte();
                    if (data == -1) 
                        throw new ApplicationException("Invalid data");

                    while (i < scanline.Length && numbytes-- != 0)
                        scanline[i++] = (byte)data;
                }
            }
        }

        public static Image Load(string path)
        {
            if (!File.Exists(path))
                return null;

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                PCXHeader header = ReadHeader(fs);

                Int32 row = 0;

                byte[] palette = header.Palette;        // Regular palette
                if (IsPaletteThere(fs))
                    palette = GetPalette(fs);
                int width=header.RightMargin - header.LeftMargin + 1;
                int height=header.LowerMargin - header.UpperMargin + 1;

                BinaryReader br = new BinaryReader(fs);
                br.BaseStream.Seek(128, SeekOrigin.Begin);

                byte[] scanLine = new byte[header.ColourPlanes * header.BytesPerScanline];

                Bitmap bm = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData data = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bm.PixelFormat);
                
                byte[] parsed = new byte[height*data.Stride];

                int i;
                while (row < height)
                {
                    if (header.ColourPlanes == 1 && header.BitsPerPixel == 8)
                    {
                        GetScanline(br, ref scanLine);

                        for (i = 0; i < width; ++i)
                        {
                            int index = scanLine[i] * 3;
                            int q = row * data.Stride + i*3;
                            parsed[q + 2] = palette[index + 0];
                            parsed[q + 1] = palette[index + 1];
                            parsed[q + 0] = palette[index + 2];

                            
                        }
                    }
                    else if (header.ColourPlanes == 3 && header.BitsPerPixel == 8)
                    {
                        GetScanline(br, ref scanLine);
                        for (i = 0; i < width; ++i)
                        {
                            int q = row * data.Stride + i * 3;
                            parsed[q + 2] = scanLine[header.BytesPerScanline * 0 + i];
                            parsed[q + 1] = scanLine[header.BytesPerScanline * 1 + i];
                            parsed[q + 0] = scanLine[header.BytesPerScanline * 2 + i];

                        }
                    }

                    row += 1;
                }
                System.Runtime.InteropServices.Marshal.Copy(parsed, 0, data.Scan0, data.Stride*height);
                bm.UnlockBits(data);
                return bm;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return null;
        }
    }
}
