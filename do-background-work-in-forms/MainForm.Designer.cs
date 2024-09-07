namespace do_background_work_in_forms
{
    partial class MainForm
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
            buttonStartWorkload = new Button();
            label = new Label();
            SuspendLayout();
            // 
            // buttonStartWorkload
            // 
            buttonStartWorkload.Location = new Point(166, 113);
            buttonStartWorkload.Name = "buttonStartWorkload";
            buttonStartWorkload.Size = new Size(105, 87);
            buttonStartWorkload.TabIndex = 0;
            buttonStartWorkload.Text = "Start Workload";
            buttonStartWorkload.UseVisualStyleBackColor = true;
            // 
            // label
            // 
            label.BackColor = Color.WhiteSmoke;
            label.BorderStyle = BorderStyle.FixedSingle;
            label.Font = new Font("Segoe UI", 20F);
            label.Location = new Point(135, 23);
            label.Name = "label";
            label.Size = new Size(179, 72);
            label.TabIndex = 1;
            label.Text = "00:00:00";
            label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 244);
            Controls.Add(label);
            Controls.Add(buttonStartWorkload);
            Name = "MainForm";
            Text = "Main Form";
            ResumeLayout(false);
        }

        #endregion

        private Button buttonStartWorkload;
        private Label label;
    }
}
