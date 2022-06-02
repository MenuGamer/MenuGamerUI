using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPickerV2
{
    public partial class MainWindow : Form
    {
        public enum Pages
        {
            Main, 
            Settings,
        }

        private int FormX, FormY, nextValidRecordTick;

        public MainWindow()
        {
            InitializeComponent();

            this.customScrollbar1.Minimum = 0;
            this.customScrollbar1.Maximum = this.PreviewItemsPanel.DisplayRectangle.Height;
            this.PreviewItemsPanel.ControlAdded += CustomScrollbar_refresh;
            this.PreviewItemsPanel.ControlRemoved += CustomScrollbar_refresh;
            this.PreviewItemsPanel.MouseWheel += PreviewItemsPanel_Scroll; 
            this.customScrollbar1.LargeChange = customScrollbar1.Maximum / customScrollbar1.Height + this.PreviewItemsPanel.Height;
            this.customScrollbar1.SmallChange = 15;
            this.customScrollbar1.Value = Math.Abs(this.PreviewItemsPanel.AutoScrollPosition.Y);
            this.PreviewItemsPanel.Width -= 10;

            this.Settings_Scrollbar.Minimum = 0;
            this.Settings_Scrollbar.Maximum = this.SettingsPanel.DisplayRectangle.Height;
            this.Settings_Scrollbar.LargeChange = Settings_Scrollbar.Maximum / Settings_Scrollbar.Height + this.SettingsPanel.Height;
            this.Settings_Scrollbar.SmallChange = 15;
            this.Settings_Scrollbar.Value = Math.Abs(this.SettingsPanel.AutoScrollPosition.Y);
        }

        #region WindowEvents

        #region MoveWindow
        private void MoveWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                FormX = Cursor.Position.X - this.Location.X;
                FormY = Cursor.Position.Y - this.Location.Y;
                MoveWindow.Start();
            }
        }

        private void MoveWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MoveWindow.Stop();
        }
        private void MoveWindow_Tick(object sender, EventArgs e) => this.Location = new Point(Cursor.Position.X - FormX, Cursor.Position.Y - FormY);
        #endregion
        #region WindowButtons
        private void ApplicationCloseButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Application.Exit();
        }

        private void ApplicationCloseButton_MouseEnter(object sender, EventArgs e) => ApplicationCloseButton.BackgroundImage = Properties.Resources.CloseWindow_Hovered;
        private void ApplicationCloseButton_MouseLeave(object sender, EventArgs e) => ApplicationCloseButton.BackgroundImage = Properties.Resources.CloseWindow_Night;

        private void ApplicationMinimizeButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.WindowState = FormWindowState.Minimized;
        }

        private void ApplicationMinimizeButton_MouseEnter(object sender, EventArgs e) => ApplicationMinimizeButton.BackgroundImage = Properties.Resources.Minimize_Hovered;
        private void ApplicationMinimizeButton_MouseLeave(object sender, EventArgs e) => ApplicationMinimizeButton.BackgroundImage = Properties.Resources.Minimize_Night;
        #endregion
        #region SettingsButton
        private void SettingsButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ChangePage(Pages.Settings);
        }
        private void SettingsButton_MouseEnter(object sender, EventArgs e) => SettingsButton.BackgroundImage = Properties.Resources.Settings_20px_hovered;
        private void SettingsButton_MouseLeave(object sender, EventArgs e) => SettingsButton.BackgroundImage = Properties.Resources.Setting_20px_Night;
        private void SettingsButton_MouseDown(object sender, MouseEventArgs e) 
        {
            if (e.Button == MouseButtons.Left)
                SettingsButton.BackgroundImage = Properties.Resources.Settings_20px_Down;
        }
        private void SettingsButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SettingsButton.BackgroundImage = Properties.Resources.Settings_20px_hovered;
        }
        #endregion

        #endregion

        private void CustomScrollbar_refresh(object sender, ControlEventArgs e)
        {
            if (this.PreviewItemsPanel.DisplayRectangle.Height > this.PreviewItemsPanel.Height)
            {
                if (!this.customScrollbar1.Visible)
                {
                    this.customScrollbar1.Show();
                    this.PreviewItemsPanel.Width += 10;
                }
            }
            else if (this.customScrollbar1.Visible)
            {
                this.customScrollbar1.Hide();
                this.PreviewItemsPanel.Width -= 10;
            }
            this.customScrollbar1.Maximum = this.PreviewItemsPanel.DisplayRectangle.Height;
            this.customScrollbar1.LargeChange = customScrollbar1.Maximum / customScrollbar1.Height + this.PreviewItemsPanel.Height;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            TabHandler.SendToBack();
            Settings_ScrollbarCoverPanel.BringToFront();
            Settings_ApplicationInfo_VersionValueLbl.Text = Application.ProductVersion;
            ColorPreview.Start();
        }

        private void SettingsExitButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ChangePage(Pages.Main);
        }

        private void SettingsExitButton_MouseEnter(object sender, EventArgs e) => SettingsExitButton.BackgroundImage = Properties.Resources.Delete_WhiteHovered25px;

        private void SettingsExitButton_MouseLeave(object sender, EventArgs e) => SettingsExitButton.BackgroundImage = Properties.Resources.Delete_White25px;

        private void SettingsExitButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SettingsExitButton.BackgroundImage = Properties.Resources.Delete_WhiteDown25px;
        }

        private void SettingsExitButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SettingsExitButton.BackgroundImage = Properties.Resources.Delete_WhiteHovered25px;
        }

        private void ColorPreview_Tick(object sender, EventArgs e)
        {
            bool record = false;
            if (nextValidRecordTick < Environment.TickCount && (WinAPI.GetAsyncKeyState(Settings.RecordKey) & 0x8000) > 0)
            {
                record = true;
                nextValidRecordTick = Environment.TickCount + 200;
            }

            if (Settings.RealTimeColorDisplay || record)
            {
                Point pixelPos = Cursor.Position;
                Color pixelColor = WinAPI.GetColorAt(pixelPos);
                PreviewBoxX.Value = pixelPos.X.ToString();
                PreviewBoxY.Value = pixelPos.Y.ToString();
                PreviewBoxColorDisplayBox.FillColor = pixelColor;
                PreviewBoxA.Value = pixelColor.A.ToString();
                PreviewBoxR.Value = pixelColor.R.ToString();
                PreviewBoxG.Value = pixelColor.G.ToString();
                PreviewBoxB.Value = pixelColor.B.ToString();
                PreviewBoxHex.Value = pixelColor.ToHex();
                PreviewBoxARGBCode.Value = $"({pixelColor.A}, {pixelColor.R}, {pixelColor.G}, {pixelColor.B})";
                PreviewBoxARGBEditor.Value = $"{pixelColor.A}; {pixelColor.R}; {pixelColor.G}; {pixelColor.B}";

                if (record)
                    CreateNewPreviewItem(pixelColor);
            }
        }

        private void PreviewItemsPanel_Scroll(object sender, MouseEventArgs e) => customScrollbar1.Value = Math.Abs(this.PreviewItemsPanel.AutoScrollPosition.Y);

        private void customScrollbar1_Scroll(object sender, EventArgs e)
        {
            PreviewItemsPanel.AutoScrollPosition = new Point(0, customScrollbar1.Value);
            customScrollbar1.Invalidate();
            Application.DoEvents();
        }

        private void Settings_Scrollbar_Scroll(object sender, EventArgs e)
        {
            SettingsPanel.AutoScrollPosition = new Point(0, Settings_Scrollbar.Value);
            Settings_Scrollbar.Invalidate();
            Application.DoEvents();
        }

        private void SettingsRefresh()
        {
            foreach (Control cntrl in SettingsPanel.Controls)
            {
                if (cntrl.GetType() == typeof(UI.RoundedToggleButton))
                {
                    UI.RoundedToggleButton toggleButton = (UI.RoundedToggleButton)cntrl;
                    toggleButton.Checked = GetSettingsByName<bool>((string)toggleButton.Tag);
                }
            }
        }

        private T GetSettingsByName<T>(string name)
        {
            var property = typeof(Settings).GetFields().ToList().Find(x => x.Name == name);
            if (property.Equals(default(System.Reflection.FieldInfo)))
                throw new Exception("Field not found!");
            return (T)property.GetValue(property);
        }

        private void SetSettingsByName<T>(string name, T value)
        {
            var property = typeof(Settings).GetFields().ToList().Find(x => x.Name == name);
            if (property.Equals(default(System.Reflection.FieldInfo)))
                throw new Exception("Field not found!");
            property.SetValue(property, Convert.ChangeType(value, value.GetType()));
        }

        private void ToggleButton_OnCheckedChanged(object sender, EventArgs e)
        {
            UI.RoundedToggleButton toggleButton = (UI.RoundedToggleButton)sender;
            SetSettingsByName<bool>((string)toggleButton.Tag, toggleButton.Checked);
        }

        private void CreateNewPreviewItem(Color item)
        {
            UI.PreviewItem PreviewCntrl = new UI.PreviewItem(item) {
                Dock = DockStyle.Top,
            };
            PreviewCntrl.MouseWheel += PreviewItemsPanel_Scroll; 
            PreviewItemsPanel.Controls.Add(PreviewCntrl);
        }

        private void ChangePage(Pages page)
        {
            if (page == Pages.Settings)
                SettingsRefresh();
            TabHandler.SelectedIndex = (int)page;
        }
    }
}
