using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GammaJul.LgLcd;
using System.Timers;
using System.Windows;

namespace Zinal.SkypeLCD
{
    internal class LCDHandler
    {
        LcdApplet _Applet;
        LcdDevice _Device;

        Timer UpdatedTimer = new Timer(30);

        Dictionary<String, MyLcdGdiPage> Pages = new Dictionary<String, MyLcdGdiPage>();

        internal MyLcdGdiPage CurrentPage = null;

        private Zinal.SkypeLibrary.Message _LastMessage = null;

        private System.Drawing.Font textFont = null;

        public Window ConfigurationWindow = null;

        public Zinal.SkypeLibrary.Message LastMessage
        {
            get
            {
                return _LastMessage;
            }
            set
            {
                _LastMessage = value;

                if (_LastMessage != null)
                {
                    Pages["NewMessage"].Get<LcdGdiText>("Time").Text = _LastMessage.timestamp.ToShortTimeString();
                    Pages["NewMessage"].Get<LcdGdiText>("Author").Text = _LastMessage.from_dispname;
                    Pages["NewMessage"].Get<LcdGdiText>("Message").Text = _LastMessage.ParsedMessage;

                    Pages["NewMessage"].SetAsCurrentDevicePage();
                    _Device.SetAsForegroundApplet = true;
                }                
            }
        }

        public LCDHandler()
        {
            UpdatedTimer.AutoReset = true;
            UpdatedTimer.Elapsed += new ElapsedEventHandler(UpdatedTimer_Elapsed);
            UpdatedTimer.Start();

            _Applet = new LcdApplet("Skype LCD", LcdAppletCapabilities.Both, true);

            
            _Applet.DeviceArrival += new EventHandler<LcdDeviceTypeEventArgs>(_Applet_DeviceArrival);
            _Applet.Configure += new EventHandler(_Applet_Configure);
            _Applet.Connect();
        }

        private delegate void ShowDelegate();

        void _Applet_Configure(object sender, EventArgs e)
        {
            ShowConfigurationWindow();
        }

        private void ShowConfigurationWindow()
        {
            if (ConfigurationWindow != null)
            {
                if (!ConfigurationWindow.Dispatcher.CheckAccess())
                    ConfigurationWindow.Dispatcher.Invoke(new ShowDelegate(ShowConfigurationWindow));
                else
                    ConfigurationWindow.Show();
            }
        }

        void UpdatedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_Device != null && !_Device.IsDisposed)
                _Device.DoUpdateAndDraw();
        }

        void _Applet_DeviceArrival(object sender, LcdDeviceTypeEventArgs e)
        {
            if (_Device == null)
            {
                Console.WriteLine("Device Arrived!");
                _Device = _Applet.OpenDeviceByType(e.DeviceType);
                _Device.SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(_Device_SoftButtonsChanged);

                Pages.Add("NewMessage", new MyLcdGdiPage(_Device, this));
                Pages["NewMessage"].SoftButtonsChanged += new EventHandler<LcdSoftButtonsEventArgs>(NewMessage_SoftButtonsChanged);
                Pages["NewMessage"].Updated += new EventHandler<UpdateEventArgs>(LCDHandler_Updated);

                Pages["NewMessage"].Add("BG", new LcdGdiRectangle());
                Pages["NewMessage"].Get<LcdGdiRectangle>("BG").Size = new System.Drawing.SizeF(_Device.PixelWidth, _Device.PixelHeight);
                Pages["NewMessage"].Get<LcdGdiRectangle>("BG").Brush = System.Drawing.Brushes.Black;

                Pages["NewMessage"].Add("Time", new LcdGdiText());
                Pages["NewMessage"].Get<LcdGdiText>("Time").Text = "";
                Pages["NewMessage"].Get<LcdGdiText>("Time").VerticalAlignment = LcdGdiVerticalAlignment.Top;
                Pages["NewMessage"].Get<LcdGdiText>("Time").HorizontalAlignment = LcdGdiHorizontalAlignment.Left;
                textFont = new System.Drawing.Font(Pages["NewMessage"].Get<LcdGdiText>("Time").Font, System.Drawing.FontStyle.Bold);
                Pages["NewMessage"].Get<LcdGdiText>("Time").Font = textFont;
                Pages["NewMessage"].Get<LcdGdiText>("Time").Brush = System.Drawing.Brushes.White;

                Pages["NewMessage"].Add("Author", new LcdGdiText());
                Pages["NewMessage"].Get<LcdGdiText>("Author").Text = "";
                Pages["NewMessage"].Get<LcdGdiText>("Author").VerticalAlignment = LcdGdiVerticalAlignment.Top;
                Pages["NewMessage"].Get<LcdGdiText>("Author").HorizontalAlignment = LcdGdiHorizontalAlignment.Right;
                Pages["NewMessage"].Get<LcdGdiText>("Author").Font = textFont;
                Pages["NewMessage"].Get<LcdGdiText>("Author").Brush = System.Drawing.Brushes.White;

                System.Drawing.StringFormat MessageFormat = new System.Drawing.StringFormat();
                MessageFormat.Alignment = System.Drawing.StringAlignment.Center;
                MessageFormat.LineAlignment = System.Drawing.StringAlignment.Center;

                Pages["NewMessage"].Add("Message", new LcdGdiText());
                Pages["NewMessage"].Get<LcdGdiText>("Message").Text = "";
                Pages["NewMessage"].Get<LcdGdiText>("Message").Margin = new MarginF(0, 0, 0, 0);
                Pages["NewMessage"].Get<LcdGdiText>("Message").StringFormat = MessageFormat;
                Pages["NewMessage"].Get<LcdGdiText>("Message").VerticalAlignment = LcdGdiVerticalAlignment.Stretch;
                Pages["NewMessage"].Get<LcdGdiText>("Message").HorizontalAlignment = LcdGdiHorizontalAlignment.Stretch;
                Pages["NewMessage"].Get<LcdGdiText>("Message").Font = textFont;
                Pages["NewMessage"].Get<LcdGdiText>("Message").Brush = System.Drawing.Brushes.White;

                Pages["NewMessage"].SetAsCurrentDevicePage();
            }
        }


        void LCDHandler_Updated(object sender, UpdateEventArgs e)
        {

        }

        void NewMessage_SoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            if (e.SoftButtons == LcdSoftButtons.None)
                return;

            if (e.SoftButtons == LcdSoftButtons.Button3)
            {
                _Device.SetAsForegroundApplet = false;
            }

        }

        void _Device_SoftButtonsChanged(object sender, LcdSoftButtonsEventArgs e)
        {
            if (this.CurrentPage is MyLcdGdiPage)
                ((MyLcdGdiPage)this.CurrentPage).OnSoftButtonsChanged(e);
        }

    }

    internal class MyLcdGdiPage : IDisposable
    {

        public event EventHandler<UpdateEventArgs> Updated
        {
            add
            {
                _Page.Updated += value;
            }
            remove
            {
                _Page.Updated -= value;
            }
        }

        public event EventHandler<LcdSoftButtonsEventArgs> SoftButtonsChanged;

        private LcdGdiPage _Page;

        private LCDHandler _Handler;

        private Dictionary<String, LcdGdiObject> _Children = new Dictionary<String, LcdGdiObject>();

        public MyLcdGdiPage(LcdDevice Device, LCDHandler Handler)
        {
            this._Page = new LcdGdiPage(Device);
            this._Handler = Handler;
        }

        public void SetAsCurrentDevicePage()
        {
            this._Page.Device.CurrentPage = this._Page;
            this._Handler.CurrentPage = this;
        }

        public void Add(String Name, LcdGdiObject obj)
        {
            obj.Changed += new EventHandler(obj_Changed);
            this._Children.Add(Name, obj);
        }

        public T Get<T>(String Name) where T : LcdGdiObject
        {
            if (this._Children.ContainsKey(Name))
                return (T)this._Children[Name];

            return null;
        }

        public bool Remove(String Name)
        {
            if (this._Children.ContainsKey(Name))
            {
                this._Children[Name].Changed -= obj_Changed;
                return this._Children.Remove(Name);
            }

            return false;
        }

        internal void OnSoftButtonsChanged(LcdSoftButtonsEventArgs e)
        {
            if (this.SoftButtonsChanged != null)
                this.SoftButtonsChanged(this, e);
        }

        void obj_Changed(object sender, EventArgs e)
        {
            this._Page.Children.Clear();
            for (int i = 0; i < this._Children.Count; i++)
                this._Page.Children.Add(this._Children.ElementAt(i).Value);

            this._Page.Invalidate();
        }

        public void Dispose()
        {
            this._Children.Clear();
            this._Page.Dispose();
        }
    }
}
