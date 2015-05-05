namespace ListFolders {
  partial class Test {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.tbRes = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // tbRes
      // 
      this.tbRes.Location = new System.Drawing.Point(12, 12);
      this.tbRes.Name = "tbRes";
      this.tbRes.Size = new System.Drawing.Size(532, 425);
      this.tbRes.TabIndex = 0;
      this.tbRes.Text = "";
      // 
      // Test
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(556, 449);
      this.Controls.Add(this.tbRes);
      this.Name = "Test";
      this.Text = "Test";
      this.Load += new System.EventHandler(this.Test_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox tbRes;
  }
}