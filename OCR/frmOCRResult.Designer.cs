namespace IWT_OCR.OCR
{
    partial class frmOCRResult
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblImg = new System.Windows.Forms.Label();
            this.lblOK = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNG = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(22, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "読取画像：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblImg
            // 
            this.lblImg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblImg.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblImg.Location = new System.Drawing.Point(148, 20);
            this.lblImg.Name = "lblImg";
            this.lblImg.Size = new System.Drawing.Size(154, 34);
            this.lblImg.TabIndex = 1;
            this.lblImg.Text = "10";
            this.lblImg.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblImg.Click += new System.EventHandler(this.label2_Click);
            // 
            // lblOK
            // 
            this.lblOK.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblOK.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblOK.Location = new System.Drawing.Point(212, 61);
            this.lblOK.Name = "lblOK";
            this.lblOK.Size = new System.Drawing.Size(90, 34);
            this.lblOK.TabIndex = 3;
            this.lblOK.Text = "10";
            this.lblOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(22, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 39);
            this.label4.TabIndex = 2;
            this.label4.Text = "認識結果：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNG
            // 
            this.lblNG.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNG.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblNG.Location = new System.Drawing.Point(212, 102);
            this.lblNG.Name = "lblNG";
            this.lblNG.Size = new System.Drawing.Size(90, 34);
            this.lblNG.TabIndex = 5;
            this.lblNG.Text = "10";
            this.lblNG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(84, 231);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(157, 38);
            this.button1.TabIndex = 6;
            this.button1.Text = "確認";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(26, 156);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(276, 53);
            this.panel1.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("ＭＳ ゴシック", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(272, 49);
            this.label2.TabIndex = 3;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(148, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 39);
            this.label3.TabIndex = 8;
            this.label3.Text = "成功";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("ＭＳ ゴシック", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(148, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 39);
            this.label6.TabIndex = 9;
            this.label6.Text = "失敗";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmOCRResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 281);
            this.ControlBox = false;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblNG);
            this.Controls.Add(this.lblOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblImg);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "frmOCRResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ＯＣＲ認識処理が終了しました";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmOCRResult_FormClosing);
            this.Load += new System.EventHandler(this.frmOCRResult_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblImg;
        private System.Windows.Forms.Label lblOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNG;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
    }
}