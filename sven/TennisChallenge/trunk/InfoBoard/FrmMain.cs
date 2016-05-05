using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TennisChallenge.InfoBoard.Properties;

namespace TennisChallenge.InfoBoard
{
  public partial class FrmMain : Form
  {
    private readonly Model _model;

    private readonly Timer _timer = new Timer();

    public FrmMain(Model model)
    {
      InitializeComponent();
      InitTimeout();

      mainBrowser.DocumentCompleted += mainBrowser_DocumentCompleted;

      _model = model;
      _model.RfidRead += ModelOnRfidRead;
    }

    void mainBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
      mainBrowser.Document.MouseUp += new HtmlElementEventHandler(Document_MouseUp);
    }

    public void InitTimeout()
    {
      _timer.Interval = Settings.Default.RefreshTimeout;
      _timer.Tick += new EventHandler(timer_Tick);
      _timer.Start();
    }

    void timer_Tick(object sender, EventArgs e)
    {
      // Manually invoke Garbage Collection to prevent memory leaks.
      GC.Collect();
      mainBrowser.Navigate(Properties.Settings.Default.StandbyUrl);
      _timer.Stop();
    }

    private void ModelOnRfidRead(string rfid)
    {
      if (mainBrowser.InvokeRequired)
      {
        mainBrowser.Invoke(new Action<string>(ModelOnRfidRead), new object[] { rfid });
      }
      else
      {
        mainBrowser.Document.InvokeScript("onRfidRead", new object[] { rfid });
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      LoadInfoboard();
      
      base.OnLoad(e);
    }

    private void LoadInfoboard()
    {
      Guid clubId = Properties.Settings.Default.ClubId;
      var url = Properties.Settings.Default.InfoUrl + clubId;
      mainBrowser.Navigate(url);
    }


    private void Document_MouseUp(object sender, HtmlElementEventArgs e)
    {
      var url = mainBrowser.Url.AbsoluteUri;
      if (e.MouseButtonsPressed == System.Windows.Forms.MouseButtons.Left)
      {
        if (url == Properties.Settings.Default.StandbyUrl)
        {
          LoadInfoboard();
          _timer.Start();
        }
        else
        {
          _timer.Stop();
          _timer.Start();
        }
      }
    }
  }
}
