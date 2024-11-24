namespace Plugin.CryptoUI
{
	partial class PanelCryptoUI
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
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox gbDescr;
			this.txtDescription = new System.Windows.Forms.TextBox();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.splitToc = new System.Windows.Forms.SplitContainer();
			this.lbModules = new System.Windows.Forms.ListBox();
			this.bnInvoke = new System.Windows.Forms.Button();
			this.pgMain = new System.Windows.Forms.PropertyGrid();
			this.error = new System.Windows.Forms.ErrorProvider(this.components);
			gbDescr = new System.Windows.Forms.GroupBox();
			gbDescr.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.splitToc.Panel1.SuspendLayout();
			this.splitToc.Panel2.SuspendLayout();
			this.splitToc.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.error)).BeginInit();
			this.SuspendLayout();
			// 
			// gbDescr
			// 
			gbDescr.Controls.Add(this.txtDescription);
			gbDescr.Dock = System.Windows.Forms.DockStyle.Fill;
			gbDescr.Location = new System.Drawing.Point(0, 0);
			gbDescr.Name = "gbDescr";
			gbDescr.Size = new System.Drawing.Size(75, 72);
			gbDescr.TabIndex = 0;
			gbDescr.TabStop = false;
			gbDescr.Text = "Description";
			// 
			// txtDescription
			// 
			this.txtDescription.AcceptsReturn = true;
			this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDescription.Location = new System.Drawing.Point(3, 16);
			this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
			this.txtDescription.Multiline = true;
			this.txtDescription.Name = "txtDescription";
			this.txtDescription.ReadOnly = true;
			this.txtDescription.Size = new System.Drawing.Size(69, 53);
			this.txtDescription.TabIndex = 0;
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 0);
			this.splitMain.Name = "splitMain";
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.splitToc);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.bnInvoke);
			this.splitMain.Panel2.Controls.Add(this.pgMain);
			this.splitMain.Size = new System.Drawing.Size(225, 206);
			this.splitMain.SplitterDistance = 75;
			this.splitMain.TabIndex = 0;
			// 
			// splitToc
			// 
			this.splitToc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitToc.Location = new System.Drawing.Point(0, 0);
			this.splitToc.Name = "splitToc";
			this.splitToc.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitToc.Panel1
			// 
			this.splitToc.Panel1.Controls.Add(this.lbModules);
			// 
			// splitToc.Panel2
			// 
			this.splitToc.Panel2.Controls.Add(gbDescr);
			this.splitToc.Size = new System.Drawing.Size(75, 206);
			this.splitToc.SplitterDistance = 130;
			this.splitToc.TabIndex = 0;
			// 
			// lbModules
			// 
			this.lbModules.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbModules.FormattingEnabled = true;
			this.lbModules.Location = new System.Drawing.Point(0, 0);
			this.lbModules.Name = "lbModules";
			this.lbModules.Size = new System.Drawing.Size(75, 130);
			this.lbModules.TabIndex = 0;
			this.lbModules.SelectedIndexChanged += new System.EventHandler(this.lbModules_SelectedIndexChanged);
			// 
			// bnInvoke
			// 
			this.bnInvoke.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.error.SetIconAlignment(this.bnInvoke, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
			this.bnInvoke.Location = new System.Drawing.Point(68, 180);
			this.bnInvoke.Name = "bnInvoke";
			this.bnInvoke.Size = new System.Drawing.Size(75, 23);
			this.bnInvoke.TabIndex = 1;
			this.bnInvoke.Text = "&Invoke";
			this.bnInvoke.UseVisualStyleBackColor = true;
			this.bnInvoke.Click += new System.EventHandler(this.bnInvoke_Click);
			// 
			// pgMain
			// 
			this.pgMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pgMain.LineColor = System.Drawing.SystemColors.ControlDark;
			this.pgMain.Location = new System.Drawing.Point(2, 3);
			this.pgMain.Name = "pgMain";
			this.pgMain.Size = new System.Drawing.Size(141, 171);
			this.pgMain.TabIndex = 0;
			// 
			// error
			// 
			this.error.ContainerControl = this;
			// 
			// PanelCryptoUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Name = "PanelCryptoUI";
			this.Size = new System.Drawing.Size(225, 206);
			gbDescr.ResumeLayout(false);
			gbDescr.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.splitToc.Panel1.ResumeLayout(false);
			this.splitToc.Panel2.ResumeLayout(false);
			this.splitToc.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.error)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitToc;
		private System.Windows.Forms.TextBox txtDescription;
		private System.Windows.Forms.ListBox lbModules;
		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.Button bnInvoke;
		private System.Windows.Forms.ErrorProvider error;
		private System.Windows.Forms.PropertyGrid pgMain;

	}
}
