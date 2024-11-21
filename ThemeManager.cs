using System.Drawing;
using System.Windows.Forms;

namespace FolderToPDF
{
    public static class ThemeManager
    {
        public static void ApplyTheme(Control control, bool isDarkMode)
        {
            if (control is Form form)
            {
                form.BackColor = isDarkMode ? Color.FromArgb(50, 50, 50) : SystemColors.Control;
                form.ForeColor = isDarkMode ? Color.White : SystemColors.ControlText;
            }

            foreach (Control childControl in control.Controls)
            {
                ApplyThemeToControl(childControl, isDarkMode);
            }
        }

        private static void ApplyThemeToControl(Control control, bool isDarkMode)
        {
            if (control is TextBox textBox)
            {
                textBox.BackColor = isDarkMode ? Color.FromArgb(45, 45, 45) : SystemColors.Window;
                textBox.ForeColor = isDarkMode ? Color.White : SystemColors.WindowText;
            }
            else if (control is Button button)
            {
                button.BackColor = isDarkMode ? Color.FromArgb(50, 50, 50) : SystemColors.Control;
                button.ForeColor = isDarkMode ? Color.White : SystemColors.ControlText;
            }
            else if (control is Label label)
            {
                label.ForeColor = isDarkMode ? Color.White : SystemColors.ControlText;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.ForeColor = isDarkMode ? Color.White : SystemColors.ControlText;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = isDarkMode ? Color.FromArgb(30, 30, 30) : SystemColors.Control;
                foreach (Control childControl in panel.Controls)
                {
                    ApplyThemeToControl(childControl, isDarkMode);
                }
            }
        }
    }
}
