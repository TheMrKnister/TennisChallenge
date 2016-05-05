namespace TennisChallenge.InfoBoard
{
  partial class FrmMain
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.mainBrowser = new System.Windows.Forms.WebBrowser();
      this.SuspendLayout();
      // 
      // mainBrowser
      // 
      this.mainBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mainBrowser.Location = new System.Drawing.Point(0, 0);
      this.mainBrowser.MinimumSize = new System.Drawing.Size(20, 20);
      this.mainBrowser.Name = "mainBrowser";
      this.mainBrowser.ScriptErrorsSuppressed = true;
      this.mainBrowser.ScrollBarsEnabled = false;
      this.mainBrowser.Size = new System.Drawing.Size(942, 635);
      this.mainBrowser.TabIndex = 0;
      // 
      // FrmMain
      // 
      this.mainBrowser.ScriptErrorsSuppressed = true;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(942, 635);
      this.Controls.Add(this.mainBrowser);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "FrmMain";
      this.Text = "Form1";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.ResumeLayout(false);
      

    }

    #endregion

    private System.Windows.Forms.WebBrowser mainBrowser;
  }
}

