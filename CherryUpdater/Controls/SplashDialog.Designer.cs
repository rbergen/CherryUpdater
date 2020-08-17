namespace CherryUpdater
{
    partial class SplashDialog
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
            this.mCopyrightLabel = new System.Windows.Forms.Label();
            this.mVersionLabel = new System.Windows.Forms.Label();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mProductLabel = new System.Windows.Forms.Label();
            this.mCreditLabel1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mDisclaimerLabel = new System.Windows.Forms.Label();
            this.mCreditLabel2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mInitializingLabel = new CherryUpdater.FadeLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCopyrightLabel
            // 
            this.mCopyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCopyrightLabel.Location = new System.Drawing.Point(3, 262);
            this.mCopyrightLabel.Name = "mCopyrightLabel";
            this.mCopyrightLabel.Size = new System.Drawing.Size(327, 19);
            this.mCopyrightLabel.TabIndex = 0;
            this.mCopyrightLabel.Text = "copyright";
            this.mCopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mVersionLabel
            // 
            this.mVersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mVersionLabel.Location = new System.Drawing.Point(3, 243);
            this.mVersionLabel.Name = "mVersionLabel";
            this.mVersionLabel.Size = new System.Drawing.Size(327, 19);
            this.mVersionLabel.TabIndex = 0;
            this.mVersionLabel.Text = "version";
            this.mVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Enabled = false;
            this.mOKButton.Location = new System.Drawing.Point(128, 329);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(76, 23);
            this.mOKButton.TabIndex = 1;
            this.mOKButton.Text = "ok";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mProductLabel
            // 
            this.mProductLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mProductLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mProductLabel.Location = new System.Drawing.Point(3, 117);
            this.mProductLabel.Name = "mProductLabel";
            this.mProductLabel.Size = new System.Drawing.Size(327, 27);
            this.mProductLabel.TabIndex = 0;
            this.mProductLabel.Text = "product";
            this.mProductLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mCreditLabel1
            // 
            this.mCreditLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCreditLabel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.mCreditLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mCreditLabel1.ForeColor = System.Drawing.Color.Blue;
            this.mCreditLabel1.Location = new System.Drawing.Point(3, 144);
            this.mCreditLabel1.Name = "mCreditLabel1";
            this.mCreditLabel1.Size = new System.Drawing.Size(327, 19);
            this.mCreditLabel1.TabIndex = 0;
            this.mCreditLabel1.Text = "credit1";
            this.mCreditLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mCreditLabel1.Click += new System.EventHandler(this.MCreditLabel1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Image = global::CherryUpdater.Properties.Resources.cherry;
            this.pictureBox1.Location = new System.Drawing.Point(91, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // mDisclaimerLabel
            // 
            this.mDisclaimerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mDisclaimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDisclaimerLabel.Location = new System.Drawing.Point(3, 203);
            this.mDisclaimerLabel.Name = "mDisclaimerLabel";
            this.mDisclaimerLabel.Size = new System.Drawing.Size(327, 19);
            this.mDisclaimerLabel.TabIndex = 0;
            this.mDisclaimerLabel.Text = "disclaimer";
            this.mDisclaimerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mCreditLabel2
            // 
            this.mCreditLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mCreditLabel2.Location = new System.Drawing.Point(3, 163);
            this.mCreditLabel2.Name = "mCreditLabel2";
            this.mCreditLabel2.Size = new System.Drawing.Size(327, 19);
            this.mCreditLabel2.TabIndex = 0;
            this.mCreditLabel2.Text = "credit2";
            this.mCreditLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.mProductLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.mOKButton, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.mCreditLabel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.mCopyrightLabel, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.mVersionLabel, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.mDisclaimerLabel, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.mCreditLabel2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 11);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(333, 355);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel1.Controls.Add(this.mInitializingLabel);
            this.panel1.Location = new System.Drawing.Point(85, 302);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(162, 23);
            this.panel1.TabIndex = 3;
            // 
            // mInitializingLabel
            // 
            this.mInitializingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mInitializingLabel.Location = new System.Drawing.Point(0, 0);
            this.mInitializingLabel.Margin = new System.Windows.Forms.Padding(0);
            this.mInitializingLabel.Name = "mInitializingLabel";
            this.mInitializingLabel.Size = new System.Drawing.Size(162, 23);
            this.mInitializingLabel.TabIndex = 4;
            this.mInitializingLabel.Text = "initializing";
            // 
            // SplashDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(333, 355);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SplashDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CherryUpdater";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label mCopyrightLabel;
        private System.Windows.Forms.Label mVersionLabel;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Label mProductLabel;
        private System.Windows.Forms.Label mCreditLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label mDisclaimerLabel;
        private System.Windows.Forms.Label mCreditLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private FadeLabel mInitializingLabel;

    }
}