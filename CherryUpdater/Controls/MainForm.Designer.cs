namespace CherryUpdater
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mMenuGroupBox = new System.Windows.Forms.GroupBox();
            this.mMenuItemFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mFirstMenuItemControl = new CherryUpdater.MenuItemControl();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.mBackButton = new System.Windows.Forms.Button();
            this.mMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mConfigurationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mAboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mBottomPanel = new System.Windows.Forms.Panel();
            this.mStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mMenuGroupBox.SuspendLayout();
            this.mMenuItemFlowPanel.SuspendLayout();
            this.mMainMenuStrip.SuspendLayout();
            this.mButtonPanel.SuspendLayout();
            this.mBottomPanel.SuspendLayout();
            this.mStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mMenuGroupBox
            // 
            this.mMenuGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mMenuGroupBox.AutoSize = true;
            this.mMenuGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mMenuGroupBox.Controls.Add(this.mMenuItemFlowPanel);
            this.mMenuGroupBox.Location = new System.Drawing.Point(3, 27);
            this.mMenuGroupBox.Name = "mMenuGroupBox";
            this.mMenuGroupBox.Size = new System.Drawing.Size(504, 61);
            this.mMenuGroupBox.TabIndex = 0;
            this.mMenuGroupBox.TabStop = false;
            this.mMenuGroupBox.Text = "menu title";
            // 
            // mMenuItemFlowPanel
            // 
            this.mMenuItemFlowPanel.AutoSize = true;
            this.mMenuItemFlowPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mMenuItemFlowPanel.Controls.Add(this.mFirstMenuItemControl);
            this.mMenuItemFlowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.mMenuItemFlowPanel.Location = new System.Drawing.Point(4, 15);
            this.mMenuItemFlowPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mMenuItemFlowPanel.Name = "mMenuItemFlowPanel";
            this.mMenuItemFlowPanel.Size = new System.Drawing.Size(497, 30);
            this.mMenuItemFlowPanel.TabIndex = 0;
            // 
            // mFirstMenuItemControl
            // 
            this.mFirstMenuItemControl.Location = new System.Drawing.Point(3, 3);
            this.mFirstMenuItemControl.MenuItem = null;
            this.mFirstMenuItemControl.Name = "mFirstMenuItemControl";
            this.mFirstMenuItemControl.ShowHelp = false;
            this.mFirstMenuItemControl.Size = new System.Drawing.Size(491, 24);
            this.mFirstMenuItemControl.TabIndex = 0;
            this.mFirstMenuItemControl.ToolTip = null;
            this.mFirstMenuItemControl.MenuItemSelected += new System.EventHandler<CherryUpdater.MenuItemSelectedEventArgs>(this.MMenuItemControl_MenuItemSelected);
            // 
            // mCloseButton
            // 
            this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloseButton.Location = new System.Drawing.Point(432, 3);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 23);
            this.mCloseButton.TabIndex = 4;
            this.mCloseButton.Text = "close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            this.mCloseButton.Click += new System.EventHandler(this.MCloseButton_Click);
            // 
            // mBackButton
            // 
            this.mBackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mBackButton.Location = new System.Drawing.Point(351, 3);
            this.mBackButton.Name = "mBackButton";
            this.mBackButton.Size = new System.Drawing.Size(75, 23);
            this.mBackButton.TabIndex = 3;
            this.mBackButton.Text = "back";
            this.mBackButton.UseVisualStyleBackColor = true;
            this.mBackButton.Click += new System.EventHandler(this.MBackButton_Click);
            // 
            // mMainMenuStrip
            // 
            this.mMainMenuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.mMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mConfigurationMenuItem,
            this.mAboutMenuItem});
            this.mMainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMainMenuStrip.Name = "mMainMenuStrip";
            this.mMainMenuStrip.Size = new System.Drawing.Size(510, 24);
            this.mMainMenuStrip.TabIndex = 5;
            this.mMainMenuStrip.Text = "mainmenustrip";
            // 
            // mConfigurationMenuItem
            // 
            this.mConfigurationMenuItem.CheckOnClick = true;
            this.mConfigurationMenuItem.Image = global::CherryUpdater.Properties.Resources.settings_16;
            this.mConfigurationMenuItem.Name = "mConfigurationMenuItem";
            this.mConfigurationMenuItem.Size = new System.Drawing.Size(107, 20);
            this.mConfigurationMenuItem.Text = "configuration";
            this.mConfigurationMenuItem.Click += new System.EventHandler(this.MConfigurationMenuItem_Click);
            // 
            // mAboutMenuItem
            // 
            this.mAboutMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mAboutMenuItem.Image = global::CherryUpdater.Properties.Resources.tip_16;
            this.mAboutMenuItem.Name = "mAboutMenuItem";
            this.mAboutMenuItem.Size = new System.Drawing.Size(66, 20);
            this.mAboutMenuItem.Text = "about";
            this.mAboutMenuItem.Click += new System.EventHandler(this.MAboutMenuItem_Click);
            // 
            // mButtonPanel
            // 
            this.mButtonPanel.Controls.Add(this.mCloseButton);
            this.mButtonPanel.Controls.Add(this.mBackButton);
            this.mButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.mButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.mButtonPanel.Name = "mButtonPanel";
            this.mButtonPanel.Size = new System.Drawing.Size(510, 29);
            this.mButtonPanel.TabIndex = 6;
            // 
            // mBottomPanel
            // 
            this.mBottomPanel.Controls.Add(this.mButtonPanel);
            this.mBottomPanel.Controls.Add(this.mStatusStrip);
            this.mBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mBottomPanel.Location = new System.Drawing.Point(0, 92);
            this.mBottomPanel.Name = "mBottomPanel";
            this.mBottomPanel.Size = new System.Drawing.Size(510, 51);
            this.mBottomPanel.TabIndex = 7;
            // 
            // mStatusStrip
            // 
            this.mStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mStatusLabel});
            this.mStatusStrip.Location = new System.Drawing.Point(0, 29);
            this.mStatusStrip.Name = "mStatusStrip";
            this.mStatusStrip.Size = new System.Drawing.Size(510, 22);
            this.mStatusStrip.SizingGrip = false;
            this.mStatusStrip.TabIndex = 0;
            this.mStatusStrip.Text = "statusStrip1";
            // 
            // mStatusLabel
            // 
            this.mStatusLabel.Name = "mStatusLabel";
            this.mStatusLabel.Size = new System.Drawing.Size(71, 17);
            this.mStatusLabel.Text = "tablet status";
            // 
            // mToolTip
            // 
            this.mToolTip.AutoPopDelay = 10000;
            this.mToolTip.InitialDelay = 500;
            this.mToolTip.IsBalloon = true;
            this.mToolTip.ReshowDelay = 100;
            this.mToolTip.ShowAlways = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(510, 143);
            this.Controls.Add(this.mBottomPanel);
            this.Controls.Add(this.mMenuGroupBox);
            this.Controls.Add(this.mMainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mMainMenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cherry Updater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.This_FormClosing);
            this.Load += new System.EventHandler(this.This_Load);
            this.Shown += new System.EventHandler(this.This_Shown);
            this.mMenuGroupBox.ResumeLayout(false);
            this.mMenuGroupBox.PerformLayout();
            this.mMenuItemFlowPanel.ResumeLayout(false);
            this.mMainMenuStrip.ResumeLayout(false);
            this.mMainMenuStrip.PerformLayout();
            this.mButtonPanel.ResumeLayout(false);
            this.mBottomPanel.ResumeLayout(false);
            this.mBottomPanel.PerformLayout();
            this.mStatusStrip.ResumeLayout(false);
            this.mStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox mMenuGroupBox;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.Button mBackButton;
        private System.Windows.Forms.MenuStrip mMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mConfigurationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mAboutMenuItem;
        private System.Windows.Forms.FlowLayoutPanel mButtonPanel;
        private System.Windows.Forms.Panel mBottomPanel;
        private System.Windows.Forms.StatusStrip mStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mStatusLabel;
        private System.Windows.Forms.FlowLayoutPanel mMenuItemFlowPanel;
        private MenuItemControl mFirstMenuItemControl;
        private System.Windows.Forms.ToolTip mToolTip;
    }
}

