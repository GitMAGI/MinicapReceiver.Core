using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CA_FakeServer
{
    public static class FakeProcessing
    {
        internal static byte[] HeaderMaker()
        {
            byte[] header = new byte[24];

            header[0] = (byte)1;
            header[1] = (byte)24;

            byte[] pid = BitConverter.GetBytes(15324);
            byte[] rH = BitConverter.GetBytes(1920);
            byte[] rW = BitConverter.GetBytes(1080);
            byte[] vH = BitConverter.GetBytes(480);
            byte[] vW = BitConverter.GetBytes(270);

            header[2] = pid[0];
            header[3] = pid[1];
            header[4] = pid[2];
            header[5] = pid[3];

            header[6] = rH[0];
            header[7] = rH[1];
            header[8] = rH[2];
            header[9] = rH[3];

            header[10] = rW[0];
            header[11] = rW[1];
            header[12] = rW[2];
            header[13] = rW[3];

            header[14] = vH[0];
            header[15] = vH[1];
            header[16] = vH[2];
            header[17] = vH[3];

            header[18] = vW[0];
            header[19] = vW[1];
            header[20] = vW[2];
            header[21] = vW[3];

            header[22] = (byte)1;
            header[23] = (byte)2;

            return header;
        }

        internal static List<byte[]> ImageExtraction()
        {
            string startupPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.FullName;
            string inputPath = "Input";
            string inputFile = "video.mp4";
            string input_fullfilename = Path.Combine(startupPath, inputPath, inputFile);

            string programPath = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "ffmpeg", "bin");
            string programName = "ffmpeg.exe";

            uint w = 270;
            uint h = 480;

            List<byte[]> images = new List<byte[]>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(programPath, programName),
                    Arguments = string.Join(" ", new List<string>() { "-i", input_fullfilename, "-c:v", "mjpeg", "-f", "image2pipe", "-s", string.Format("{0}x{1}", h, w), "pipe:1" }),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                }
            };
            Console.WriteLine(process.StartInfo.FileName + " " + process.StartInfo.Arguments);
            process.Start();

            MemoryStream ms_output = new MemoryStream();
            process.StandardOutput.BaseStream.CopyTo(ms_output);

            process.WaitForExit();
            process.Close();

            //byte[] data_byte = string.IsNullOrWhiteSpace(output) ? new byte[] { } : Encoding.ASCII.GetBytes(output);
            byte[] data_byte = ms_output.ToArray();
            if (data_byte == null || data_byte.Length == 0)
                return images;

            //data_byte = new byte[] { 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD8, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0x00, 0x00, 0x23, 0x45, 0x67, 0xFF, 0xD9 };

            // Gather an array of JPGs
            // A JPG is delimited by 2 Sequences:
            // SOI (Start of Image) 0xFF 0xD8
            // EOI (End of Image)   0xFF 0xD9

            byte[] soi_pattern = { 0xFF, 0xD8 };
            int[] start_indexes = (from index in Enumerable.Range(0, data_byte.Length - soi_pattern.Length + 1)
                                   where data_byte.Skip(index).Take(soi_pattern.Length).SequenceEqual(soi_pattern)
                                   select (int)index).ToArray();

            byte[] eoi_pattern = { 0xFF, 0xD9 };
            int[] end_indexes = (from index in Enumerable.Range(0, data_byte.Length - eoi_pattern.Length + 1)
                                 where data_byte.Skip(index).Take(eoi_pattern.Length).SequenceEqual(eoi_pattern)
                                 select (int)index).ToArray();

            for (int i = 0; i < start_indexes.Count(); i++)
            {
                int start_index = start_indexes[i];
                int end_index = end_indexes[i];

                byte[] image = data_byte.Skip(start_index).Take(end_index - start_index).ToArray();
                images.Add(image);
            }

            return images;
        }

    }
}
