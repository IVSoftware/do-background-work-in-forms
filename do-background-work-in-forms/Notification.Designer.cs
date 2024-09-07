namespace do_background_work_in_forms
{
    partial class Notification
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
            textBoxMessage = new TextBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // textBoxMessage
            // 
            textBoxMessage.BorderStyle = BorderStyle.None;
            textBoxMessage.Dock = DockStyle.Top;
            textBoxMessage.Location = new Point(10, 10);
            textBoxMessage.Multiline = true;
            textBoxMessage.Name = "textBoxMessage";
            textBoxMessage.ReadOnly = true;
            textBoxMessage.Size = new Size(358, 82);
            textBoxMessage.TabIndex = 2;
            textBoxMessage.TabStop = false;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(23, 98);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(130, 34);
            buttonOK.TabIndex = 0;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(196, 98);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(130, 34);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // Notification
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(378, 144);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(textBoxMessage);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Notification";
            Padding = new Padding(10, 10, 10, 0);
            Text = "Notification";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxMessage;
        private Button buttonOK;
        private Button buttonCancel;
    }
}