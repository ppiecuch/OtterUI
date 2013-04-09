using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Otter.Export
{
    /// <summary>
    ///  IExportable interface
    /// </summary>
    public interface IExportable
    {
        /// <summary>
        /// Exports this object to a binary file
        /// </summary>
        /// <param name="bw"></param>
        void Export(PlatformBinaryWriter bw);
    }
}
