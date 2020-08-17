namespace CherryUpdater
{
    partial class TaskProgressForm
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
            this.mStatusGroupBox = new System.Windows.Forms.GroupBox();
            this.mActivityProgressBar = new System.Windows.Forms.ProgressBar();
            this.mTaskProgressBar = new System.Windows.Forms.ProgressBar();
            this.mActivityProgressLabel = new System.Windows.Forms.Label();
            this.mTaskLabel = new System.Windows.Forms.Label();
            this.mLogGroupBox = new System.Windows.Forms.GroupBox();
            this.mLogTextBox = new System.Windows.Forms.TextBox();
            this.mAbortButton = new System.Windows.Forms.Button();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.mStartButton = new System.Windows.Forms.Button();
            this.mStatusGroupBox.SuspendLayout();
            this.mLogGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mStatusGroupBox
            // 
            this.mStatusGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mStatusGroupBox.Controls.Add(this.mActivityProgressBar);
            this.mStatusGroupBox.Controls.Add(this.mTaskProgressBar);
            this.mStatusGroupBox.Controls.Add(this.mActivityProgressLabel);
            this.mStatusGroupBox.Controls.Add(this.mTaskLabel);
            this.mStatusGroupBox.Location = new System.Drawing.Point(5, 12);
            this.mStatusGroupBox.Name = "mStatusGroupBox";
            this.mStatusGroupBox.Size = new System.Drawing.Size(578, 111);
            this.mStatusGroupBox.TabIndex = 0;
            this.mStatusGroupBox.TabStop = false;
            this.mStatusGroupBox.Text = "status";
            // 
            // mActivityProgressBar
            // 
            this.mActivityProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mActivityProgressBar.Location = new System.Drawing.Point(6, 79);
            this.mActivityProgressBar.Name = "mActivityProgressBar";
            this.mActivityProgressBar.Size = new System.Drawing.Size(566, 23);
            this.mActivityProgressBar.TabIndex = 1;
            // 
            // mTaskProgressBar
            // 
            this.mTaskProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mTaskProgressBar.Location = new System.Drawing.Point(6, 32);
            this.mTaskProgressBar.Name = "mTaskProgressBar";
            this.mTaskProgressBar.Size = new System.Drawing.Size(566, 23);
            this.mTaskProgressBar.TabIndex = 1;
            // 
            // mActivityProgressLabel
            // 
            this.mActivityProgressLabel.AutoSize = true;
            this.mActivityProgressLabel.Location = new System.Drawing.Point(6, 63);
            this.mActivityProgressLabel.Name = "mActivityProgressLabel";
            this.mActivityProgressLabel.Size = new System.Drawing.Size(83, 13);
            this.mActivityProgressLabel.TabIndex = 0;
            this.mActivityProgressLabel.Text = "activity progress";
            // 
            // mTaskLabel
            // 
            this.mTaskLabel.AutoSize = true;
            this.mTaskLabel.Location = new System.Drawing.Point(6, 16);
            this.mTaskLabel.Name = "mTaskLabel";
            this.mTaskLabel.Size = new System.Drawing.Size(53, 13);
            this.mTaskLabel.TabIndex = 0;
            this.mTaskLabel.Text = "executing";
            // 
            // mLogGroupBox
            // 
            this.mLogGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mLogGroupBox.Controls.Add(this.mLogTextBox);
            this.mLogGroupBox.Location = new System.Drawing.Point(5, 129);
            this.mLogGroupBox.Name = "mLogGroupBox";
            this.mLogGroupBox.Size = new System.Drawing.Size(578, 234);
            this.mLogGroupBox.TabIndex = 0;
            this.mLogGroupBox.TabStop = false;
            this.mLogGroupBox.Text = "log";
            // 
            // mLogTextBox
            // 
            this.mLogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mLogTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.mLogTextBox.Location = new System.Drawing.Point(6, 19);
            this.mLogTextBox.Multiline = true;
            this.mLogTextBox.Name = "mLogTextBox";
            this.mLogTextBox.ReadOnly = true;
            this.mLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.mLogTextBox.Size = new System.Drawing.Size(566, 209);
            this.mLogTextBox.TabIndex = 0;
            // 
            // mAbortButton
            // 
            this.mAbortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mAbortButton.Location = new System.Drawing.Point(427, 369);
            this.mAbortButton.Name = "mAbortButton";
            this.mAbortButton.Size = new System.Drawing.Size(75, 23);
            this.mAbortButton.TabIndex = 2;
            this.mAbortButton.Text = "abort";
            this.mAbortButton.UseVisualStyleBackColor = true;
            this.mAbortButton.Click += new System.EventHandler(this.MAbortButton_Click);
            // 
            // mCloseButton
            // 
            this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCloseButton.Location = new System.Drawing.Point(508, 369);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 23);
            this.mCloseButton.TabIndex = 3;
            this.mCloseButton.Text = "close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            // 
            // mStartButton
            // 
            this.mStartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mStartButton.Location = new System.Drawing.Point(346, 369);
            this.mStartButton.Name = "mStartButton";
            this.mStartButton.Size = new System.Drawing.Size(75, 23);
            this.mStartButton.TabIndex = 1;
            this.mStartButton.Text = "start";
            this.mStartButton.UseVisualStyleBackColor = true;
            this.mStartButton.Click += new System.EventHandler(this.MStartButton_Click);
            // 
            // TaskProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 404);
            this.ControlBox = false;
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.mStartButton);
            this.Controls.Add(this.mAbortButton);
            this.Controls.Add(this.mLogGroupBox);
            this.Controls.Add(this.mStatusGroupBox);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "TaskProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form1";
            this.VisibleChanged += new System.EventHandler(this.This_VisibleChanged);
            this.mStatusGroupBox.ResumeLayout(false);
            this.mStatusGroupBox.PerformLayout();
            this.mLogGroupBox.ResumeLayout(false);
            this.mLogGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox mStatusGroupBox;
        private System.Windows.Forms.ProgressBar mTaskProgressBar;
        private System.Windows.Forms.Label mTaskLabel;
        private System.Windows.Forms.GroupBox mLogGroupBox;
        private System.Windows.Forms.TextBox mLogTextBox;
        private System.Windows.Forms.Button mAbortButton;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.Button mStartButton;
        private System.Windows.Forms.ProgressBar mActivityProgressBar;
        private System.Windows.Forms.Label mActivityProgressLabel;
    }
}