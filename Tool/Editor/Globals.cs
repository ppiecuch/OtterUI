using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using Otter.Project;
using Otter.Editor.ContentViews;

namespace Otter.Editor
{
    /// <summary>
    /// Global data
    /// </summary>
    static class Globals
    {
        #region Data
        private static TimelineView mTimelineView = null;
        private static ProjectView mProjectView = null;
        private static ControlsView mControlsView = null;
        private static PropertiesView mPropertiesView = null;
        private static SceneHierarchyView mHierarchyView = null;
        private static HistoryView mHistoryView = null;

        private static List<Type> mControlTypes = new List<Type>();
        private static List<Type> mPlugins = new List<Type>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the Timeline ParentView
        /// </summary>
        public static TimelineView TimelineView
        {
            get { return Globals.mTimelineView; }
            set { Globals.mTimelineView = value; }
        }

        /// <summary>
        /// Gets / Sets the Project ParentView
        /// </summary>
        public static ProjectView ProjectView
        {
            get { return Globals.mProjectView; }
            set { Globals.mProjectView = value; }
        }

        /// <summary>
        /// Gets / Sets the Controls ParentView
        /// </summary>
        public static ControlsView ControlsView
        {
            get { return Globals.mControlsView; }
            set { Globals.mControlsView = value; }
        }

        /// <summary>
        /// Gets / Sets the Properties ParentView
        /// </summary>
        public static PropertiesView PropertiesView
        {
            get { return Globals.mPropertiesView; }
            set { Globals.mPropertiesView = value; }
        }

        /// <summary>
        /// Gets / Sets the Hierarchy ParentView
        /// </summary>
        public static SceneHierarchyView SceneHierarchyView
        {
            get { return Globals.mHierarchyView; }
            set { Globals.mHierarchyView = value; }
        }

        /// <summary>
        /// Gets / Sets the History ParentView
        /// </summary>
        public static HistoryView HistoryView
        {
            get { return Globals.mHistoryView; }
            set { Globals.mHistoryView = value; }
        }

        /// <summary>
        /// Gets the list of Custom Control Types
        /// </summary>
        public static List<Type> CustomControlTypes
        {
            get { return mControlTypes; }
        }

        /// <summary>
        /// Gets the list of Plugin Types
        /// </summary>
        public static List<Type> Plugins
        {
            get { return mPlugins; }
        }
        #endregion

        // Nothing..
        static Globals()
        {

        }
    }
}
