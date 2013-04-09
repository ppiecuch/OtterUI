using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Otter.Export
{
    /// <summary>
    /// Simple class to manage FourCC blocks
    /// </summary>
    public class FourCCStack
    {
        /// <summary>
        /// Represents a simple fourcc header
        /// </summary>
        private struct FourCC
        {
            public char[] mFourCC;
            public int mStreamPos;

            public FourCC(char[] fourCC, int streamPos)
            {
                mFourCC = fourCC;
                mStreamPos = streamPos;
            }
        };

        private PlatformBinaryWriter mBinaryWriter = null;
        private Stack<FourCC> mFourCCStack = new Stack<FourCC>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bw"></param>
        public FourCCStack(PlatformBinaryWriter bw)
        {
            mBinaryWriter = bw;
        }

        /// <summary>
        /// Pushes a fourcc object onto the stack, and writes out the code
        /// and the stream position.
        /// </summary>
        /// <param name="fourCC"></param>
        /// <param name="bw"></param>
        public void Push(string code)
        {
            FourCC fourCC = new FourCC(code.ToCharArray(0, 4), (int)mBinaryWriter.BaseStream.Position);
            mFourCCStack.Push(fourCC);

            // Since Windows little endian, the correct int representation is the reverse of the fourcc.
            int intCode = System.BitConverter.ToInt32(new byte[] { (byte)fourCC.mFourCC[3], (byte)fourCC.mFourCC[2], (byte)fourCC.mFourCC[1], (byte)fourCC.mFourCC[0] }, 0);

            // Write out the intcode.  The binary writer will write the code out to the appropriate endianness.
            mBinaryWriter.Write(intCode);

            mBinaryWriter.Write(0); // size placeholder
        }

        /// <summary>
        /// Pops a FourCC object from the stack, fills out the size info,
        /// and returns the binary writer to its current position.
        /// </summary>
        /// <param name="bw"></param>
        public void Pop()
        {
            FourCC fourCC = mFourCCStack.Pop();

            int curPos = (int)mBinaryWriter.BaseStream.Position;
            int dataSize = curPos - fourCC.mStreamPos;

            mBinaryWriter.Seek(fourCC.mStreamPos + 4, SeekOrigin.Begin);
            mBinaryWriter.Write(dataSize);
            mBinaryWriter.Seek(curPos, SeekOrigin.Begin);
        }
    }
}
