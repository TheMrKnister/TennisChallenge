using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;


namespace Inspectron.CodeCollection.ApplicationBasics
{
    /// <summary>
    /// Mit dieser Klasse kann sichergestellt werden, dass eine Anwendung nur einmal gestartet wird. 
    /// </summary>
    /// <remarks>
    /// <para>Im Konstruktor wird dazu eine binäre System-Semaphore registriert.</para>
    /// <para>Vorschlag zur Verwendung:</para>
    /// <code>
    /// static void Main()
    /// {
    ///    using (SingleProgramInstance spi = new SingleProgramInstance())
    ///    {
    ///       if (spi.IsSingleInstance)
    ///       {
    ///          Application.EnableVisualStyles();
    ///          Application.SetCompatibleTextRenderingDefault(false);
    ///          Application.Run(new FrmMain());
    ///       }
    ///       else
    ///       {
    ///          MessageBox.Show("Die Anwendung läuft bereits und kann kein zweites Mal gestartet werden.");
    ///       }
    ///    }
    /// }
    /// </code>
    /// </remarks>
    public class SingleProgramInstance : IDisposable
    {
        /// <summary>
        /// Initialisiert den System-Semaphor und verwendet als Name den Assembly-Name.
        /// </summary>
        public SingleProgramInstance()
        {
            //Initialize a named mutex and attempt to
            // get ownership immediately 
            _processSync = new Semaphore(
                0, 1, Assembly.GetExecutingAssembly().GetName().Name,
                out _owned);
        }

        /// <summary>
        /// Initialisiert den System-Semaphor. Verwendet dabei als Name "Assembly-Name + identifier".
        /// </summary>
        /// <param name="identifier">Bezeichnung welche zusammen mit dem Assebmly-Name den Name des
        /// Semaphors bildet.</param>
        /// <remarks>
        /// Kann verwendet werden, falls die einfache Bezeichnung des Semaphors nur mit dem Assembly-Namen
        /// zu Konflikten führen könnte.
        /// </remarks>
        public SingleProgramInstance(string identifier)
        {
            //Initialize a named mutex and attempt to
            // get ownership immediately.
            //Use an addtional identifier to lower
            // our chances of another process creating
            // a mutex with the same name.
            _processSync = new Semaphore(
                0, 1, Assembly.GetExecutingAssembly().GetName().Name + identifier,
                out _owned);
        }

        ~SingleProgramInstance()
        {
            //Release mutex (if necessary) 
            //This should have been accomplished using Dispose() 
            Release();
        }

        /// <summary>
        /// Testet ob die Klasse als erste den Semaphor initialisiert hat.
        /// </summary>
        public bool IsSingleInstance
        {
            //If we don't own the mutex than
            // we are not the first instance.
            get { return _owned; }
        }

        private void Release()
        {
            if (_owned)
            {
                //If we own the mutex than release it so that
                // other "same" processes can now start.
                _processSync.Release();
                _owned = false;
            }
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Gibt den Semaphor wieder frei.
        /// </summary>
        public void Dispose()
        {
            //release mutex (if necessary) and notify 
            // the garbage collector to ignore the destructor
            Release();
            GC.SuppressFinalize(this);
        }

        #endregion


        #region Private members

        private Semaphore _processSync;
        private bool _owned = false;

        #endregion // Private members

    }
}
