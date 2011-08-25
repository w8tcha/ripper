

namespace RiPRipper
{
    using System.Windows.Forms;

    /// <summary>
    /// Displays MessageBox messages as a top most window
    /// </summary>
    public static class TopMostMessageBox
    {
        /// <summary>
        /// Displays a <see cref="MessageBox"/> but as a TopMost window.
        /// </summary>
        /// <param name="message">The text to appear in the message box.</param>
        /// <returns>The button pressed.</returns>
        /// <remarks>This will display with no title and only the OK button.</remarks>
        static public DialogResult Show(string message)
        {
            return Show(message, string.Empty, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Displays a <see cref="MessageBox"/> but as a TopMost window.
        /// </summary>
        /// <param name="message">The text to appear in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <returns>The button pressed.</returns>
        /// <remarks>This will display with only the OK button.</remarks>
        static public DialogResult Show(string message, string title)
        {
            return Show(message, title, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Displays a <see cref="MessageBox"/> but as a TopMost window.
        /// </summary>
        /// <param name="message">The text to appear in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <returns>The button pressed.</returns>
        static public DialogResult Show(string message, string title, MessageBoxButtons buttons)
        {
            // Create a host form that is a TopMost window which will be the parent of the MessageBox.
            Form topmostForm = new Form
                                   {
                                       Size = new System.Drawing.Size(1, 1),
                                       StartPosition = FormStartPosition.Manual
                                   };
            // We do not want anyone to see this window so position it off the visible screen and make it as small as possible
            System.Drawing.Rectangle rect = SystemInformation.VirtualScreen;
            topmostForm.Location = new System.Drawing.Point(rect.Bottom + 10, rect.Right + 10);
            topmostForm.Show();
            // Make this form the active form and make it TopMost
            topmostForm.Focus();
            topmostForm.BringToFront();
            topmostForm.TopMost = true;
            // Finally show the MessageBox with the form just created as its owner
            DialogResult result = MessageBox.Show(topmostForm, message, title, buttons);
            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
        /// <summary>
        /// Displays a <see cref="MessageBox"/> but as a TopMost window.
        /// </summary>
        /// <param name="message">The text to appear in the message box.</param>
        /// <param name="title">The title of the message box.</param>
        /// <param name="buttons">The buttons to display in the message box.</param>
        /// <param name="icon">The Icon to display in the message box.</param>
        /// <returns>The button pressed.</returns>
        static public DialogResult Show(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            // Create a host form that is a TopMost window which will be the parent of the MessageBox.
            Form topmostForm = new Form
                                   {
                                       Size = new System.Drawing.Size(1, 1),
                                       StartPosition = FormStartPosition.Manual
                                   };
            // We do not want anyone to see this window so position it off the visible screen and make it as small as possible
            System.Drawing.Rectangle rect = SystemInformation.VirtualScreen;
            topmostForm.Location = new System.Drawing.Point(rect.Bottom + 10, rect.Right + 10);
            topmostForm.Show();
            // Make this form the active form and make it TopMost
            topmostForm.Focus();
            topmostForm.BringToFront();
            topmostForm.TopMost = true;
            // Finally show the MessageBox with the form just created as its owner
            DialogResult result = MessageBox.Show(topmostForm, message, title, buttons, icon);
            topmostForm.Dispose(); // clean it up all the way

            return result;
        }
    }
}
