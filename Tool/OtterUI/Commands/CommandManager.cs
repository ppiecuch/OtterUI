using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Otter.Commands
{
    /// <summary>
    /// This object is responsible for maintaining
    /// all the commands in the system.  When a new command is added,
    /// the command is executed and stored in an undo/redo list.
    /// By "undo'ing", the last executed command is undone, and the current
    /// command pointer moved.  The undone command are not removed until a new
    /// one is inserted after the current command.
    /// </summary>
    public class CommandManager
    {
        #region Events and Delegates
        public delegate void CommandDelegate(Command command);
        public event CommandDelegate OnExecute = null;
        public event CommandDelegate OnUndo = null;
        public event CommandDelegate OnCommandAdded = null;
        public event CommandDelegate OnCommandRemoved = null;
        #endregion

        #region Data
        /// <summary>
        /// The complete list of commands managed
        /// by this object.
        /// </summary>
        private List<Command> mCommandList = new List<Command>();

        /// <summary>
        /// Points the current command in the list.  The command
        /// pointed to has <i>already</i> been executed.
        /// </summary>
        private int mCurrentCommandIndex = -1;
        #endregion

        #region Properties

        /// <summary>
        /// Returns the list of commands
        /// </summary>
        public List<Command> Commands
        {
            get { return mCommandList; }
        }

        /// <summary>
        /// Retrieves the current command index
        /// </summary>
        public int CurrentCommandIndex
        {
            get { return mCurrentCommandIndex; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public CommandManager() { }

        /// <summary>
        /// Adds and executes a new command.  Will only add a command
        /// to the list if it has been successfully executed.
        /// </summary>
        /// <param name="command"></param>
        public bool AddCommand(Command command, bool bExecute)
        {
            if (mCurrentCommandIndex < mCommandList.Count - 1)
            {
                int start = mCurrentCommandIndex + 1;
                int count = mCommandList.Count - (mCurrentCommandIndex + 1);

                if (OnCommandRemoved != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        OnCommandRemoved(mCommandList[start + i]);
                    }
                }

                mCommandList.RemoveRange(start, count);
            }

            mCommandList.Add(command);
            mCurrentCommandIndex = mCommandList.Count - 1;

            if (OnCommandAdded != null)
                OnCommandAdded(command);

            if (bExecute)
            {
                if (command.Execute())
                {
                    if (OnExecute != null)
                    {
                        OnExecute(command);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Takes the latest action on the list, and undoes it.
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (mCurrentCommandIndex >= 0 && mCurrentCommandIndex < mCommandList.Count)
            {
                Command command = (Command)mCommandList[mCurrentCommandIndex];
                if (command.Undo())
                {
                    if (OnUndo != null)
                    {
                        OnUndo(command);
                    }
                }
                mCurrentCommandIndex--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Takes the last undone action on the list and redoes it.
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if((mCurrentCommandIndex + 1) >= 0 && (mCurrentCommandIndex + 1) < mCommandList.Count)
            {
                mCurrentCommandIndex++;
                Command command = (Command)mCommandList[mCurrentCommandIndex];
                
                if (command.Execute())
                {
                    if (OnExecute != null)
                    {
                        OnExecute(command);
                    }
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the command manager and, consequently, the entire undo/redo history.
        /// </summary>
        public void Clear()
        {
            mCurrentCommandIndex = -1;
            mCommandList.Clear();
        }
    }
}
