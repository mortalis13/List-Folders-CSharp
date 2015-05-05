namespace Test {
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
      this.tbRes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbRes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
      this.tbRes.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tbRes.ForeColor = System.Drawing.Color.White;
      this.tbRes.Location = new System.Drawing.Point(12, 12);
      this.tbRes.Margin = new System.Windows.Forms.Padding(10);
      this.tbRes.Name = "tbRes";
      this.tbRes.Size = new System.Drawing.Size(204, 175);
      this.tbRes.TabIndex = 1;
      this.tbRes.Text = "";
      this.tbRes.WordWrap = false;
      this.tbRes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbRes_KeyDown);
      // 
      // Test
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(227, 207);
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

