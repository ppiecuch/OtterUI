//
// OtterUI v. 1.3.2.0
//
// Copyright (c) Aonyx Software, LLC
// All rights reserved.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
// CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.ComponentModel;

using Otter;
using Otter.UI;
using Otter.Export;
using Otter.Interface;
using Otter.Plugins;
using Otter.UI.Animation;

namespace SamplePlugin
{
    /// <summary>
    /// Circle Control Descriptor.  This descriptor is necessary to show
    /// the control in the OtterUI Editor's controls view.
    /// </summary>
    public class CircleControlDescriptor : ControlDescriptor
    {
        /// <summary>
        /// Retrieves the control's name
        /// </summary>
        public string Name
        {
            get { return "Circle"; }
        }

        /// <summary>
        /// Retrieves the control's image
        /// </summary>
        public Image Image
        {
            get { return SamplePlugin.Resources.Resources.CircleIcon; }
        }
    }

    /// <summary>
    /// The control layout defines the properties that are animatable within
    /// a control.  For the circle, we will animate Radius, Width and Color.
    /// </summary>
    public class CircleLayout : ControlLayout
    {
        /// <summary>
        /// Circle's radius
        /// </summary>
        public uint Radius { get; set; }

        /// <summary>
        /// Circle's line width
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Circle color
        /// This property does not serialize properly to XML, so we ignore
        /// it here and create a new serializable property below.
        /// </summary>
        [XmlIgnore]
        public Color Color { get; set; }

        /// <summary>
        /// Exports the color to XML.  This property is solely responsible
        /// for serializing and deserializing the color property to and from XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement("Color")]
        public string ColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(this.Color); }
            set { this.Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }

        /// <summary>
        /// Clones the layout
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            CircleLayout layout = new CircleLayout();
            layout.SetFrom(this);

            layout.Radius = this.Radius;
            layout.Width = this.Width;
            layout.Color = this.Color;

            return layout;
        }

        /// <summary>
        /// Exports the circle layout
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("CRLT");
            {
                // Export the base layout properties
                ExportBase(bw);

                // Export our specific properties
                bw.Write((UInt32)Radius);
                bw.Write((UInt32)Width);
                bw.Write((UInt32)Color.ToArgb());
            }
            fourCCStack.Pop();
        }
    }

    /// <summary>
    /// Custom Circle Control.  Since we don't want to give 
    /// direct access to the layout itself, we create wrappers
    /// for the animatable properties here.
    /// </summary>
    [ControlAttribute(typeof(CircleControlDescriptor))]
    public class Circle : GUIControl
    {
        /// <summary>
        /// Number of segments in the circle
        /// </summary>
        public uint Segments { get; set; }

        /// <summary>
        /// Circle's radius
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public uint Radius
        {
            get
            {
                return ((CircleLayout)Layout).Radius;
            }
            set
            {
                ((CircleLayout)Layout).Radius = value;
            }
        }

        /// <summary>
        /// Circle's width
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public uint Width
        {
            get
            {
                return ((CircleLayout)Layout).Width;
            }
            set
            {
                ((CircleLayout)Layout).Width = value;
            }
        }

        /// <summary>
        /// Circle's color
        /// </summary>
        [XmlIgnore]
        [Category("Layout")]
        public Color Color
        {
            get
            {
                return ((CircleLayout)Layout).Color;
            }
            set
            {
                ((CircleLayout)Layout).Color = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Circle()
        {
            Name = "Circle";
            Layout = new CircleLayout();
            Segments = 20;

            this.Radius = 10;
            this.Width = 5;
            this.Color = Color.White;
        }

        /// <summary>
        /// Applies the provided keyframe to the control,
        /// overriding all needed properties
        /// </summary>
        /// <param name="frame"></param>
        public override void ApplyKeyFrame(KeyFrame startFrame, KeyFrame endFrame, float factor)
        {
            base.ApplyKeyFrame(startFrame, endFrame, factor);

            CircleLayout startLayout = startFrame != null ? startFrame.Layout as CircleLayout : null;
            CircleLayout endLayout = endFrame != null ? endFrame.Layout as CircleLayout : null;

            if (startLayout == null && endLayout == null)
                return;

            int r, g, b, a;
            uint width, radius;

            if (endLayout == null)
            {
                r = startLayout.Color.R;
                g = startLayout.Color.G;
                b = startLayout.Color.B;
                a = startLayout.Color.A;

                width = startLayout.Width;
                radius = startLayout.Radius;
            }
            else
            {
                r = (int)((float)startLayout.Color.R * (1.0f - factor) + (float)endLayout.Color.R * factor);
                g = (int)((float)startLayout.Color.G * (1.0f - factor) + (float)endLayout.Color.G * factor);
                b = (int)((float)startLayout.Color.B * (1.0f - factor) + (float)endLayout.Color.B * factor);
                a = (int)((float)startLayout.Color.A * (1.0f - factor) + (float)endLayout.Color.A * factor);

                width = (uint)((float)startLayout.Width * (1.0f - factor) + (float)endLayout.Width * factor);
                radius = (uint)((float)startLayout.Radius * (1.0f - factor) + (float)endLayout.Radius * factor);
            }

            this.Radius = radius;
            this.Width = width;
            this.Color = Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Draws the Circle.
        /// </summary>
        /// <param name="graphics"></param>
        public override void Draw(Otter.Interface.Graphics graphics)
        {
            // Do not draw if there are less than 3 segments in the circle
            if(Segments <= 2)
                return;
            
            Triangle[] triangles = new Triangle[Segments * 2];

            double x = 1.0f;
            double y = 0.0f;

            float radius = ((CircleLayout)Layout).Radius;
            uint halfWidth = ((CircleLayout)Layout).Width / 2;
            Color color = ((CircleLayout)Layout).Color;

            PointF center = new PointF(Layout.Size.Width / 2.0f, Layout.Size.Height / 2.0f);

            // For each segment create two triangles
            for (int i = 0; i < Segments; i++)
            {
                double angle1 = (i / (double)Segments) * Math.PI * 2;
                double angle2 = ((i + 1) / (double)Segments) * Math.PI * 2;

                float rx1 = (float)(x * Math.Cos(angle1) - y * Math.Sin(angle1));
                float ry1 = (float)(x * Math.Sin(angle1) + y * Math.Cos(angle1));

                float rx2 = (float)(x * Math.Cos(angle2) - y * Math.Sin(angle2));
                float ry2 = (float)(x * Math.Sin(angle2) + y * Math.Cos(angle2));

                triangles[i * 2 + 0] = new Triangle();
                triangles[i * 2 + 0].SetVertex(0, center.X + rx1 * (radius + halfWidth), center.Y + ry1 * (radius + halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());
                triangles[i * 2 + 0].SetVertex(1, center.X + rx2 * (radius + halfWidth), center.Y + ry2 * (radius + halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());
                triangles[i * 2 + 0].SetVertex(2, center.X + rx1 * (radius - halfWidth), center.Y + ry1 * (radius - halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());

                triangles[i * 2 + 1] = new Triangle();
                triangles[i * 2 + 1].SetVertex(0, center.X + rx1 * (radius - halfWidth), center.Y + ry1 * (radius - halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());
                triangles[i * 2 + 1].SetVertex(1, center.X + rx2 * (radius + halfWidth), center.Y + ry2 * (radius + halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());
                triangles[i * 2 + 1].SetVertex(2, center.X + rx2 * (radius - halfWidth), center.Y + ry2 * (radius - halfWidth), 0.0f, 0.0f, 0.0f, color.ToArgb());
            }

            graphics.DrawTriangles(-1, triangles);
        }

        /// <summary>
        /// Exports the Circle
        /// </summary>
        /// <param name="bw"></param>
        public override void Export(Otter.Export.PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("CIRC");
            {
                base.Export(bw);

                bw.Write((UInt32)Segments);
                bw.Write((UInt32)Radius);
                bw.Write((UInt32)Width);
                bw.Write((UInt32)this.Color.ToArgb());
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Clones the Circle
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            Circle ret = new Circle();

            // GUIControl level copy
            ret.ID = ParentView.NextControlID++;
            ret.Name = "Copy of " + Name;
            ret.Layout = this.Layout.Clone() as CircleLayout;
            ret.Scene = Scene;
            ret.Parent = Parent;

            // Circle Level copy
            ret.Segments = this.Segments;

            return ret;
        }
    }
}
