namespace CherryUpdater
{
    partial class MenuItemControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mTitleLabel = new System.Windows.Forms.Label();
            this.mExecuteButton = new System.Windows.Forms.Button();
            this.mQuestionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTitleLabel
            // 
            this.mTitleLabel.AutoSize = true;
            this.mTitleLabel.Location = new System.Drawing.Point(3, 5);
            this.mTitleLabel.MaximumSize = new System.Drawing.Size(380, 13);
            this.mTitleLabel.Name = "mTitleLabel";
            this.mTitleLabel.Size = new System.Drawing.Size(23, 13);
            this.mTitleLabel.TabIndex = 0;
            this.mTitleLabel.Text = "title";
            // 
            // mExecuteButton
            // 
            this.mExecuteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mExecuteButton.Location = new System.Drawing.Point(380, 0);
            this.mExecuteButton.Margin = new System.Windows.Forms.Padding(0);
            this.mExecuteButton.Name = "mExecuteButton";
            this.mExecuteButton.Size = new System.Drawing.Size(88, 23);
            this.mExecuteButton.TabIndex = 1;
            this.mExecuteButton.Text = "execute";
            this.mExecuteButton.UseVisualStyleBackColor = true;
            this.mExecuteButton.Click += new System.EventHandler(this.MExecuteButtonClicked);
            // 
            // mQuestionLabel
            // 
            this.mQuestionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mQuestionLabel.Image = global::CherryUpdater.Properties.Resources.question_16;
            this.mQuestionLabel.Location = new System.Drawing.Point(356, 0);
            this.mQuestionLabel.Name = "mQuestionLabel";
            this.mQuestionLabel.Size = new System.Drawing.Size(25, 23);
            this.mQuestionLabel.TabIndex = 2;
            // 
            // MenuItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mQuestionLabel);
            this.Controls.Add(this.mExecuteButton);
            this.Controls.Add(this.mTitleLabel);
            this.Name = "MenuItemControl";
            this.Size = new System.Drawing.Size(468, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTitleLabel;
        private System.Windows.Forms.Button mExecuteButton;
        private System.Windows.Forms.Label mQuestionLabel;
    }
}
