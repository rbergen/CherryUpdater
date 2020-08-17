namespace CherryUpdater
{
    partial class ConfigurationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mOkButton = new System.Windows.Forms.Button();
            this.mIgnoreProductModelCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.mEnableLoggingCheckBox = new System.Windows.Forms.CheckBox();
            this.mLogLevelComboBox = new System.Windows.Forms.ComboBox();
            this.mShowHelpCheckBox = new System.Windows.Forms.CheckBox();
            this.mUseAdbProviderCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.mCancelButton);
            this.flowLayoutPanel1.Controls.Add(this.mOkButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 108);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(334, 29);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(256, 3);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mOkButton
            // 
            this.mOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOkButton.Location = new System.Drawing.Point(175, 3);
            this.mOkButton.Name = "mOkButton";
            this.mOkButton.Size = new System.Drawing.Size(75, 23);
            this.mOkButton.TabIndex = 0;
            this.mOkButton.Text = "ok";
            this.mOkButton.UseVisualStyleBackColor = true;
            // 
            // mIgnoreProductModelCheckBox
            // 
            this.mIgnoreProductModelCheckBox.AutoSize = true;
            this.mIgnoreProductModelCheckBox.Location = new System.Drawing.Point(12, 35);
            this.mIgnoreProductModelCheckBox.Name = "mIgnoreProductModelCheckBox";
            this.mIgnoreProductModelCheckBox.Size = new System.Drawing.Size(119, 17);
            this.mIgnoreProductModelCheckBox.TabIndex = 1;
            this.mIgnoreProductModelCheckBox.Text = "ignoreproductmodel";
            this.mIgnoreProductModelCheckBox.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.mEnableLoggingCheckBox);
            this.flowLayoutPanel2.Controls.Add(this.mLogLevelComboBox);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(12, 78);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(322, 27);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // mEnableLoggingCheckBox
            // 
            this.mEnableLoggingCheckBox.AutoSize = true;
            this.mEnableLoggingCheckBox.Location = new System.Drawing.Point(0, 6);
            this.mEnableLoggingCheckBox.Margin = new System.Windows.Forms.Padding(0, 6, 0, 3);
            this.mEnableLoggingCheckBox.Name = "mEnableLoggingCheckBox";
            this.mEnableLoggingCheckBox.Size = new System.Drawing.Size(93, 17);
            this.mEnableLoggingCheckBox.TabIndex = 0;
            this.mEnableLoggingCheckBox.Text = "createlogging:";
            this.mEnableLoggingCheckBox.UseVisualStyleBackColor = true;
            this.mEnableLoggingCheckBox.CheckedChanged += new System.EventHandler(this.MEnableLoggingCheckBox_CheckedChanged);
            // 
            // mLogLevelComboBox
            // 
            this.mLogLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mLogLevelComboBox.Enabled = false;
            this.mLogLevelComboBox.FormattingEnabled = true;
            this.mLogLevelComboBox.Location = new System.Drawing.Point(93, 3);
            this.mLogLevelComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.mLogLevelComboBox.Name = "mLogLevelComboBox";
            this.mLogLevelComboBox.Size = new System.Drawing.Size(121, 21);
            this.mLogLevelComboBox.TabIndex = 1;
            // 
            // mShowHelpCheckBox
            // 
            this.mShowHelpCheckBox.AutoSize = true;
            this.mShowHelpCheckBox.Location = new System.Drawing.Point(12, 58);
            this.mShowHelpCheckBox.Name = "mShowHelpCheckBox";
            this.mShowHelpCheckBox.Size = new System.Drawing.Size(71, 17);
            this.mShowHelpCheckBox.TabIndex = 2;
            this.mShowHelpCheckBox.Text = "showhelp";
            this.mShowHelpCheckBox.UseVisualStyleBackColor = true;
            // 
            // mUseAdbProviderCheckBox
            // 
            this.mUseAdbProviderCheckBox.AutoSize = true;
            this.mUseAdbProviderCheckBox.Location = new System.Drawing.Point(12, 12);
            this.mUseAdbProviderCheckBox.Name = "mUseAdbProviderCheckBox";
            this.mUseAdbProviderCheckBox.Size = new System.Drawing.Size(99, 17);
            this.mUseAdbProviderCheckBox.TabIndex = 0;
            this.mUseAdbProviderCheckBox.Text = "useadbprovider";
            this.mUseAdbProviderCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 137);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.mShowHelpCheckBox);
            this.Controls.Add(this.mUseAdbProviderCheckBox);
            this.Controls.Add(this.mIgnoreProductModelCheckBox);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "configuration";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mOkButton;
        private System.Windows.Forms.CheckBox mIgnoreProductModelCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox mEnableLoggingCheckBox;
        private System.Windows.Forms.ComboBox mLogLevelComboBox;
        private System.Windows.Forms.CheckBox mShowHelpCheckBox;
        private System.Windows.Forms.CheckBox mUseAdbProviderCheckBox;
    }
}