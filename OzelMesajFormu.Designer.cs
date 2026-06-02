namespace _2.sınıf_2._dönem_projesi
{
    partial class OzelMesajFormu
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
            lblMesaj = new Label();
            btnTamam = new Button();
            SuspendLayout();
            // 
            // lblMesaj
            // 
            lblMesaj.AutoSize = true;
            lblMesaj.Font = new Font("Constantia", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            lblMesaj.ForeColor = Color.DarkRed;
            lblMesaj.Location = new Point(12, 49);
            lblMesaj.Name = "lblMesaj";
            lblMesaj.Size = new Size(65, 24);
            lblMesaj.TabIndex = 0;
            lblMesaj.Text = "label1";
            // 
            // btnTamam
            // 
            btnTamam.BackColor = Color.Black;
            btnTamam.FlatAppearance.BorderSize = 3;
            btnTamam.FlatStyle = FlatStyle.Flat;
            btnTamam.ForeColor = Color.DarkRed;
            btnTamam.Location = new Point(107, 123);
            btnTamam.Name = "btnTamam";
            btnTamam.Size = new Size(94, 34);
            btnTamam.TabIndex = 1;
            btnTamam.Text = "Tamam";
            btnTamam.UseVisualStyleBackColor = false;
            btnTamam.Click += btnTamam_Click;
            // 
            // OzelMesajFormu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(318, 190);
            Controls.Add(btnTamam);
            Controls.Add(lblMesaj);
            ForeColor = Color.Black;
            FormBorderStyle = FormBorderStyle.None;
            Name = "OzelMesajFormu";
            StartPosition = FormStartPosition.CenterParent;
            Text = "OzelMesajFormu";
            TransparencyKey = Color.Black;
            Load += OzelMesajFormu_Load;
            Paint += OzelMesajFormu_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblMesaj;
        private Button btnTamam;
    }
}