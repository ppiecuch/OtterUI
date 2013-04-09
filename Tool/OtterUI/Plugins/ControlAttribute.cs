using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Otter.Plugins
{
    /// <summary>
    /// Control Descriptor interface.
    /// </summary>
    public interface ControlDescriptor
    {
        /// <summary>
        /// Retrieves the control's name
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Retrieves the image representing the control
        /// </summary>
        Image Image
        {
            get;
        }
    };

    /// <summary>
    /// Describes a control that can be created in the OtterUI editor.
    /// Without this attribute, a control will not show up in the OtterUI Controls View
    /// </summary>
    public class ControlAttribute : System.Attribute
    {
#region Data
        private Type mDescriptorType;
#endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public ControlAttribute(Type descriptorType)
        {
            mDescriptorType = descriptorType;
        }

        /// <summary>
        /// Creates an instance of the descriptor object
        /// </summary>
        /// <returns></returns>
        public ControlDescriptor GetDescriptor()
        {
            return (ControlDescriptor)Activator.CreateInstance(mDescriptorType);
        }
    }
}
