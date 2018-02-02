namespace Route66
{
    partial class FormStatistics
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
            this.btnOk = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numSpeed = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.lblUptoLastTime = new System.Windows.Forms.Label();
            this.lblTotalTime = new System.Windows.Forms.Label();
            this.lblSpreadingTime = new System.Windows.Forms.Label();
            this.lblDrivingTime = new System.Windows.Forms.Label();
            this.lblUptolastDistance = new System.Windows.Forms.Label();
            this.lblTotalDistance = new System.Windows.Forms.Label();
            this.lblSpreadingDistance = new System.Windows.Forms.Label();
            this.lblDrivingDistance = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numPersentage = new System.Windows.Forms.NumericUpDown();
            this.lblArea = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblWet = new System.Windows.Forms.Label();
            this.lblDry = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPersentage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(529, 279);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numSpeed);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lblUptoLastTime);
            this.groupBox1.Controls.Add(this.lblTotalTime);
            this.groupBox1.Controls.Add(this.lblSpreadingTime);
            this.groupBox1.Controls.Add(this.lblDrivingTime);
            this.groupBox1.Controls.Add(this.lblUptolastDistance);
            this.groupBox1.Controls.Add(this.lblTotalDistance);
            this.groupBox1.Controls.Add(this.lblSpreadingDistance);
            this.groupBox1.Controls.Add(this.lblDrivingDistance);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(399, 256);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Distance / Time";
            // 
            // numSpeed
            // 
            this.numSpeed.Location = new System.Drawing.Point(298, 72);
            this.numSpeed.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSpeed.Name = "numSpeed";
            this.numSpeed.Size = new System.Drawing.Size(42, 22);
            this.numSpeed.TabIndex = 15;
            this.numSpeed.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numSpeed.ValueChanged += new System.EventHandler(this.numSpeed_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(346, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 17);
            this.label10.TabIndex = 14;
            this.label10.Text = "km/h";
            // 
            // lblUptoLastTime
            // 
            this.lblUptoLastTime.AutoSize = true;
            this.lblUptoLastTime.Location = new System.Drawing.Point(237, 204);
            this.lblUptoLastTime.Name = "lblUptoLastTime";
            this.lblUptoLastTime.Size = new System.Drawing.Size(48, 17);
            this.lblUptoLastTime.TabIndex = 13;
            this.lblUptoLastTime.Text = "0:40 h";
            // 
            // lblTotalTime
            // 
            this.lblTotalTime.AutoSize = true;
            this.lblTotalTime.Location = new System.Drawing.Point(237, 169);
            this.lblTotalTime.Name = "lblTotalTime";
            this.lblTotalTime.Size = new System.Drawing.Size(48, 17);
            this.lblTotalTime.TabIndex = 12;
            this.lblTotalTime.Text = "0:40 h";
            // 
            // lblSpreadingTime
            // 
            this.lblSpreadingTime.AutoSize = true;
            this.lblSpreadingTime.Location = new System.Drawing.Point(237, 114);
            this.lblSpreadingTime.Name = "lblSpreadingTime";
            this.lblSpreadingTime.Size = new System.Drawing.Size(48, 17);
            this.lblSpreadingTime.TabIndex = 11;
            this.lblSpreadingTime.Text = "0:30 h";
            // 
            // lblDrivingTime
            // 
            this.lblDrivingTime.AutoSize = true;
            this.lblDrivingTime.Location = new System.Drawing.Point(237, 74);
            this.lblDrivingTime.Name = "lblDrivingTime";
            this.lblDrivingTime.Size = new System.Drawing.Size(48, 17);
            this.lblDrivingTime.TabIndex = 10;
            this.lblDrivingTime.Text = "0:40 h";
            // 
            // lblUptolastDistance
            // 
            this.lblUptolastDistance.AutoSize = true;
            this.lblUptolastDistance.Location = new System.Drawing.Point(160, 204);
            this.lblUptolastDistance.Name = "lblUptolastDistance";
            this.lblUptolastDistance.Size = new System.Drawing.Size(58, 17);
            this.lblUptolastDistance.TabIndex = 9;
            this.lblUptolastDistance.Text = "34,4 km";
            // 
            // lblTotalDistance
            // 
            this.lblTotalDistance.AutoSize = true;
            this.lblTotalDistance.Location = new System.Drawing.Point(160, 169);
            this.lblTotalDistance.Name = "lblTotalDistance";
            this.lblTotalDistance.Size = new System.Drawing.Size(58, 17);
            this.lblTotalDistance.TabIndex = 8;
            this.lblTotalDistance.Text = "36,5 km";
            // 
            // lblSpreadingDistance
            // 
            this.lblSpreadingDistance.AutoSize = true;
            this.lblSpreadingDistance.Location = new System.Drawing.Point(160, 114);
            this.lblSpreadingDistance.Name = "lblSpreadingDistance";
            this.lblSpreadingDistance.Size = new System.Drawing.Size(58, 17);
            this.lblSpreadingDistance.TabIndex = 7;
            this.lblSpreadingDistance.Text = "20,3 km";
            // 
            // lblDrivingDistance
            // 
            this.lblDrivingDistance.AutoSize = true;
            this.lblDrivingDistance.Location = new System.Drawing.Point(160, 74);
            this.lblDrivingDistance.Name = "lblDrivingDistance";
            this.lblDrivingDistance.Size = new System.Drawing.Size(58, 17);
            this.lblDrivingDistance.TabIndex = 6;
            this.lblDrivingDistance.Text = "16,2 km";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Upto last action";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Total";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(12, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(357, 3);
            this.label4.TabIndex = 3;
            this.label4.Text = "Spreading / Spaying";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Spreading / Spaying";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Drinving";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(160, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Distance      Time      Speed";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.numPersentage);
            this.groupBox2.Controls.Add(this.lblArea);
            this.groupBox2.Controls.Add(this.lblTotalAmount);
            this.groupBox2.Controls.Add(this.lblWet);
            this.groupBox2.Controls.Add(this.lblDry);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(417, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 256);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Amount";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(153, 115);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(20, 17);
            this.label13.TabIndex = 21;
            this.label13.Text = "%";
            // 
            // numPersentage
            // 
            this.numPersentage.Location = new System.Drawing.Point(105, 112);
            this.numPersentage.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numPersentage.Name = "numPersentage";
            this.numPersentage.Size = new System.Drawing.Size(42, 22);
            this.numPersentage.TabIndex = 20;
            this.numPersentage.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numPersentage.ValueChanged += new System.EventHandler(this.numPersentage_ValueChanged);
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArea.Location = new System.Drawing.Point(48, 205);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(84, 17);
            this.lblArea.TabIndex = 19;
            this.lblArea.Text = "55.456 m2";
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalAmount.Location = new System.Drawing.Point(48, 170);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(57, 17);
            this.lblTotalAmount.TabIndex = 18;
            this.lblTotalAmount.Text = "549 kg";
            // 
            // lblWet
            // 
            this.lblWet.AutoSize = true;
            this.lblWet.Location = new System.Drawing.Point(48, 115);
            this.lblWet.Name = "lblWet";
            this.lblWet.Size = new System.Drawing.Size(51, 17);
            this.lblWet.TabIndex = 17;
            this.lblWet.Text = "470 kg";
            // 
            // lblDry
            // 
            this.lblDry.AutoSize = true;
            this.lblDry.Location = new System.Drawing.Point(48, 75);
            this.lblDry.Name = "lblDry";
            this.lblDry.Size = new System.Drawing.Size(43, 17);
            this.lblDry.TabIndex = 16;
            this.lblDry.Text = "89 kg";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label12.Location = new System.Drawing.Point(8, 148);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(162, 3);
            this.label12.TabIndex = 15;
            this.label12.Text = "Spreading / Spaying";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 204);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 17);
            this.label11.TabIndex = 14;
            this.label11.Text = "Area";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(40, 17);
            this.label9.TabIndex = 13;
            this.label9.Text = "Total";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 114);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 17);
            this.label8.TabIndex = 12;
            this.label8.Text = "Wet";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 74);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "Dry";
            // 
            // FormStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 314);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Name = "FormStatistics";
            this.Text = "Route statistics";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpeed)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPersentage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numSpeed;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblUptoLastTime;
        private System.Windows.Forms.Label lblTotalTime;
        private System.Windows.Forms.Label lblSpreadingTime;
        private System.Windows.Forms.Label lblDrivingTime;
        private System.Windows.Forms.Label lblUptolastDistance;
        private System.Windows.Forms.Label lblTotalDistance;
        private System.Windows.Forms.Label lblSpreadingDistance;
        private System.Windows.Forms.Label lblDrivingDistance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblWet;
        private System.Windows.Forms.Label lblDry;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numPersentage;
    }
}