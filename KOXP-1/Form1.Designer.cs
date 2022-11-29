namespace KOXP_1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInject = new System.Windows.Forms.Button();
            this.txtWindowsName = new System.Windows.Forms.ComboBox();
            this.btnTown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(154, 12);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(159, 59);
            this.btnInject.TabIndex = 0;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // txtWindowsName
            // 
            this.txtWindowsName.FormattingEnabled = true;
            this.txtWindowsName.Location = new System.Drawing.Point(12, 12);
            this.txtWindowsName.Name = "txtWindowsName";
            this.txtWindowsName.Size = new System.Drawing.Size(121, 23);
            this.txtWindowsName.TabIndex = 1;
            this.txtWindowsName.DropDown += new System.EventHandler(this.txtWindowsName_DropDown);
            // 
            // btnTown
            // 
            this.btnTown.Location = new System.Drawing.Point(154, 96);
            this.btnTown.Name = "btnTown";
            this.btnTown.Size = new System.Drawing.Size(159, 60);
            this.btnTown.TabIndex = 2;
            this.btnTown.Text = "Town";
            this.btnTown.UseVisualStyleBackColor = true;
            this.btnTown.Click += new System.EventHandler(this.btnTown_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 165);
            this.Controls.Add(this.btnTown);
            this.Controls.Add(this.txtWindowsName);
            this.Controls.Add(this.btnInject);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnInject;
        private ComboBox txtWindowsName;
        private Button btnTown;
    }
}