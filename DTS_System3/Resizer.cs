using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTS_System3
{
    public class Resizer
    {
        private struct ControlInfo
        {
            public string Name;
            public int Left;
            public int Top;
            public int OriginalHeight;
            public int OriginalWidth;
            public float OriginalFontSize;
        }

        public int OriginalHeight { get; private set; } = -1;
        public int OriginalWidth { get; private set; } = -1;
        private Dictionary<string, ControlInfo> ctrlDict = new Dictionary<string, ControlInfo>();

        public void FindAllControls(Control thisCtrl)
        {
            if (OriginalHeight == -1)
            {
                OriginalHeight = thisCtrl.Height;
            }
            if (OriginalWidth == -1)
            {
                OriginalWidth = thisCtrl.Width;
            }

            foreach (Control ctl in thisCtrl.Controls)
            {
                var c = new ControlInfo
                {
                    Name = ctl.Name,
                    Top = ctl.Top,
                    Left = ctl.Left,
                    OriginalFontSize = ctl.Font.Size,
                    OriginalHeight = ctl.Height,
                    OriginalWidth = ctl.Width
                };

                if (!ctrlDict.ContainsKey(c.Name))
                {
                    ctrlDict.Add(c.Name, c);
                }

                if (ctl.Controls.Count > 0)
                {
                    FindAllControls(ctl);
                }
            }
        }

        public void ResizeAllControls(Control thisCtrl, int formWidth, int formHeight)
        {
            if (OriginalHeight == -1 || OriginalWidth == -1)
            {
                return;
            }

            double currentHeightFactor = (double)formHeight / OriginalHeight;
            double currentWidthFactor = (double)formWidth / OriginalWidth;

            foreach (Control ctl in thisCtrl.Controls)
            {
                if (ctrlDict.TryGetValue(ctl.Name, out ControlInfo c))
                {
                    ctl.Top = (int)(c.Top * currentHeightFactor);
                    ctl.Left = (int)(c.Left * currentWidthFactor);
                    ctl.Width = (int)(c.OriginalWidth * currentWidthFactor);
                    ctl.Height = (int)(c.OriginalHeight * currentHeightFactor);

                    Font f = ctl.Font;
                    float fratio = (float)((currentHeightFactor + currentWidthFactor) / 2);
                    ctl.Font = new Font(f.FontFamily, c.OriginalFontSize * fratio, f.Style);
                }

                if (ctl.Controls.Count > 0)
                {
                    ResizeAllControls(ctl, formWidth, formHeight);
                }
            }
        }
    }


}
