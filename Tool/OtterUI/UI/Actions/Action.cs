using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

using Otter.Export;

namespace Otter.UI.Actions
{
    /// <summary>
    /// OtterUI Action Base Class
    /// </summary>
    public abstract class Action : ICloneable
    {
        #region Properties
        /// <summary>
        /// Gets / Sets the scene that this action belongs to
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public GUIScene Scene { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view"></param>
        public Action()
        {
        }

        /// <summary>
        /// Exports to a binary stream
        /// </summary>
        /// <param name="bw"></param>
        public virtual void Export(PlatformBinaryWriter bw) 
        {
        }

        /// <summary>
        /// Clones this action
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();
    }
}
