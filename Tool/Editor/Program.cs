using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;

using Otter.Export;
using Otter.Project;
using Otter.Plugins;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using Otter.Editor.Forms;

namespace Otter.Editor
{
    static class Program
    {
        /// <summary>
        /// Command Line Arguments Parser for the Otter.Editor tool
        /// </summary>
        private class CommandLineArgs
        {
            public string mProject = "";
            public string mOutputDir = "";
            public string mPlatform = "";

            public bool mExport = false;
            public bool mDumpAnims = false;

            /// <summary>
            /// Constructor - parses the command line args
            /// </summary>
            /// <param name="args"></param>
            public CommandLineArgs(string[] args)
            {
                // Get each of the tokens individually
                List<string> tokens = new List<string>();
                foreach(string arg in args)
                {
                    string[] str = arg.Split(new char[] { '-', '=', '\"' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in str)
                        tokens.Add(s);
                }

                for (int i = 0; i < tokens.Count; i++ )
                {
                    if (tokens[i] == "export")
                    {
                        mExport = true;
                    }
                    else if ((i + 1) < tokens.Count)
                    {
                        if (tokens[i] == "open" || tokens[i] == "project")
                        {
                            mProject = tokens[++i];
                        }
                        else if (tokens[i] == "outdir")
                        {
                            mOutputDir = tokens[++i];
                        }
                        else if (tokens[i] == "platform")
                        {
                            mPlatform = tokens[++i];
                        }
                    }
                }
            }
        };

        /// <summary>
        /// Loads OtterEditor Plugins
        /// </summary>
        private static void LoadPlugins()
        {
            List<Type> controlTypes = new List<Type>();
            List<Type> layoutTypes = new List<Type>();

            controlTypes.Add(typeof(Otter.UI.GUIButton));
            controlTypes.Add(typeof(Otter.UI.GUISprite));
            controlTypes.Add(typeof(Otter.UI.GUILabel));
            controlTypes.Add(typeof(Otter.UI.GUITable));
            controlTypes.Add(typeof(Otter.UI.GUIToggle));
            controlTypes.Add(typeof(Otter.UI.GUISlider));
            controlTypes.Add(typeof(Otter.UI.GUIMask));
            controlTypes.Add(typeof(Otter.UI.GUIGroup));

            layoutTypes.Add(typeof(Otter.UI.ControlLayout));
            layoutTypes.Add(typeof(Otter.UI.ButtonLayout));
            layoutTypes.Add(typeof(Otter.UI.SpriteLayout));
            layoutTypes.Add(typeof(Otter.UI.LabelLayout));
            layoutTypes.Add(typeof(Otter.UI.TableLayout));
            layoutTypes.Add(typeof(Otter.UI.ToggleLayout));
            layoutTypes.Add(typeof(Otter.UI.SliderLayout));
            layoutTypes.Add(typeof(Otter.UI.MaskLayout));
            layoutTypes.Add(typeof(Otter.UI.GroupLayout));

            try
            {
                // Let's see if we can find plugins
                string[] files = System.IO.Directory.GetFiles(Application.StartupPath + "\\Plugins");
                foreach (string file in files)
                {
                    try
                    {
                        System.Console.WriteLine("File: " + file);
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(file);

                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsSubclassOf(typeof(IPlugin)))
                            {
                                PluginAttribute attribute = (PluginAttribute)System.Attribute.GetCustomAttribute(type, typeof(PluginAttribute));

                                if (attribute != null)
                                {
                                    Globals.Plugins.Add(type);
                                }
                            }
                            else if (!controlTypes.Contains(type) && type.IsSubclassOf(typeof(Otter.UI.GUIControl)))
                            {
                                // Found a custom GUIControl.  Ensure that the "ControlAttribute" is present
                                System.Attribute attribute = System.Attribute.GetCustomAttribute(type, typeof(ControlAttribute));
                                if (attribute != null)
                                {
                                    Globals.CustomControlTypes.Add(type);
                                    controlTypes.Add(type);
                                }
                            }
                            else if (!layoutTypes.Contains(type) && type.IsSubclassOf(typeof(Otter.UI.ControlLayout)))
                            {
                                layoutTypes.Add(type);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            // TODO - Attempt to serialize/deserialize each type before
            // it's added to the controls list.  If it fails, don't add it.

            XmlAttributes controlAttrs = new XmlAttributes();
            foreach(Type type in controlTypes)
                controlAttrs.XmlArrayItems.Add(new XmlArrayItemAttribute(type));
            GUIProject.XmlAttributeOverrides.Add(typeof(Otter.UI.GUIControl), "Controls", controlAttrs);

            XmlAttributes layoutAttrs = new XmlAttributes();
            foreach (Type type in layoutTypes)
                layoutAttrs.XmlElements.Add(new XmlElementAttribute(type));
            GUIProject.XmlAttributeOverrides.Add(typeof(Otter.UI.GUIControl), "Layout", layoutAttrs);
            GUIProject.XmlAttributeOverrides.Add(typeof(Otter.UI.Animation.KeyFrame), "Layout", layoutAttrs);
        }

        /// <summary>
        /// Processes the command line
        /// </summary>
        /// <param name="args"></param>
        private static void ProcessCommandLine(CommandLineArgs commandLineArgs)
        {
            // Perform some validation
            if (commandLineArgs.mExport)
            {
                ImporterExporter exporter = new ImporterExporter();

                // We need a valid project specified
                if (!System.IO.File.Exists(commandLineArgs.mProject))
                {
                    System.Console.WriteLine("Invalid or no project specified.");
                    return;
                }

                // Open the project
                GUIProject.CurrentProject = GUIProject.Load(commandLineArgs.mProject);

                // We need a valid platform specified
                Platform targetPlatform = null;
                foreach (Platform platform in GUIProject.CurrentProject.Platforms)
                {
                    if (platform.Name == commandLineArgs.mPlatform)
                    {
                        targetPlatform = platform;
                        break;
                    }
                }

                if (targetPlatform == null)
                {
                    System.Console.WriteLine("Invalid or no platform specified");
                    return;
                }

                // See if we have an output directory override
                string outputDir = targetPlatform.OutputDirectory;
                if (commandLineArgs.mOutputDir != "")
                {
                    outputDir = commandLineArgs.mOutputDir;
                    targetPlatform.OutputDirectory = outputDir;
                }

                // Determine the output directory.  If it's not rooted, we need to construct
                // the rooted path
                string projectDir = System.IO.Path.GetDirectoryName(commandLineArgs.mProject);
                if (!System.IO.Path.IsPathRooted(outputDir))
                    outputDir = projectDir + "\\" + outputDir;

                // Create the output directory and verify that it exists
                System.IO.Directory.CreateDirectory(outputDir);
                if (!System.IO.Directory.Exists(outputDir))
                {
                    System.Console.WriteLine("Invalid output directory for platform " + targetPlatform.Name + " : " + outputDir);
                    return;
                }

                ArrayList itemsToExport = ImporterExporter.GetItemsToExport(GUIProject.CurrentProject.Entries);
                List<Platform> platforms = new List<Platform>(new Platform[] { targetPlatform });

                // Finally, export.
                foreach (object item in itemsToExport)
                {
                    System.Console.Write("Exporting.. %s : " + item.ToString());
                    bool bSuccess = exporter.Export(item, platforms);
                    System.Console.WriteLine(bSuccess ? " success" : " FAILED");
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            LoadPlugins();

            CommandLineArgs cmdArgs = new CommandLineArgs(args);
            
            if (cmdArgs.mExport)
            {
                ProcessCommandLine(cmdArgs);
            }
            else
            {
#if !(DEBUG)
                try
                {
#endif
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new OtterEditorMainForm(cmdArgs.mProject));
#if !(DEBUG)
                }
                catch (Exception e)
                {
                    ReportBugForm reportBugForm = new ReportBugForm(e);
                    reportBugForm.ShowDialog();
                }
#endif
            }
        }
    }
}
