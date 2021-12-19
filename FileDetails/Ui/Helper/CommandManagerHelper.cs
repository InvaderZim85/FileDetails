using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDetails.Ui.Helper
{
    /// <summary>
    /// This class contains methods for the CommandManager that help avoid memory leaks by using weak references.
    /// </summary>
    public static class CommandManagerHelper
    {
        /// <summary>
        /// Calls the weak reference handlers.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        public static void CallWeakReferenceHandlers(List<WeakReference> handlers)
        {
            if (handlers == null) return;

            var callees = new EventHandler[handlers.Count];
            var count = 0;

            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];

                if (reference.Target is not EventHandler handler)
                {
                    handlers.RemoveAt(i);
                }
                else
                {
                    callees[count] = handler;
                    count++;
                }
            }

            for (var i = 0; i < count; i++)
            {
                var handler = callees[i];
                handler(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds the weak reference handler.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        /// <param name="handler">The handler.</param>
        public static void AddWeakReferenceHandler(ref List<WeakReference> handlers, EventHandler handler)
        {
            handlers ??= new List<WeakReference>();

            handlers.Add(new WeakReference(handler));
        }

        /// <summary>
        /// Removes the weak reference handler.
        /// </summary>
        /// <param name="handlers">The handlers.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveWeakReferenceHandler(List<WeakReference> handlers, EventHandler handler)
        {
            if (handlers == null) return;

            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                var reference = handlers[i];

                if (reference.Target is not EventHandler existingHandler || existingHandler == handler)
                    handlers.RemoveAt(i);
            }
        }
    }
}
