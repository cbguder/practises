namespace PractiSES
{
    partial class ConfigurationWizard
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
            System.Windows.Forms.Label UsernameLabel;
            System.Windows.Forms.Label EmailLabel;
            System.Windows.Forms.Label ServerLabel;
            System.Windows.Forms.Label ConfirmPassphraseLabel;
            System.Windows.Forms.Label QuestionLabel;
            this.Username = new System.Windows.Forms.TextBox();
            this.Email = new System.Windows.Forms.TextBox();
            this.Server = new System.Windows.Forms.TextBox();
            this.Next = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.PassphraseLabel = new System.Windows.Forms.Label();
            this.Passphrase = new System.Windows.Forms.TextBox();
            this.ConfirmPassphrase = new System.Windows.Forms.TextBox();
            this.Status = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Finish = new System.Windows.Forms.Button();
            this.Question = new System.Windows.Forms.Label();
            this.Answer = new System.Windows.Forms.TextBox();
            this.AnswerLabel = new System.Windows.Forms.Label();
            UsernameLabel = new System.Windows.Forms.Label();
            EmailLabel = new System.Windows.Forms.Label();
            ServerLabel = new System.Windows.Forms.Label();
            ConfirmPassphraseLabel = new System.Windows.Forms.Label();
            QuestionLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsernameLabel
            // 
            UsernameLabel.AutoSize = true;
            UsernameLabel.Location = new System.Drawing.Point(12, 41);
            UsernameLabel.Name = "UsernameLabel";
            UsernameLabel.Size = new System.Drawing.Size(109, 13);
            UsernameLabel.TabIndex = 2;
            UsernameLabel.Text = "PractiSES Username:";
            UsernameLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // EmailLabel
            // 
            EmailLabel.AutoSize = true;
            EmailLabel.Location = new System.Drawing.Point(45, 67);
            EmailLabel.Name = "EmailLabel";
            EmailLabel.Size = new System.Drawing.Size(76, 13);
            EmailLabel.TabIndex = 3;
            EmailLabel.Text = "Email Address:";
            EmailLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ServerLabel
            // 
            ServerLabel.AutoSize = true;
            ServerLabel.Location = new System.Drawing.Point(29, 15);
            ServerLabel.Name = "ServerLabel";
            ServerLabel.Size = new System.Drawing.Size(92, 13);
            ServerLabel.TabIndex = 7;
            ServerLabel.Text = "PractiSES Server:";
            ServerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ConfirmPassphraseLabel
            // 
            ConfirmPassphraseLabel.AutoSize = true;
            ConfirmPassphraseLabel.Location = new System.Drawing.Point(18, 119);
            ConfirmPassphraseLabel.Name = "ConfirmPassphraseLabel";
            ConfirmPassphraseLabel.Size = new System.Drawing.Size(103, 13);
            ConfirmPassphraseLabel.TabIndex = 9;
            ConfirmPassphraseLabel.Text = "Confirm Passphrase:";
            ConfirmPassphraseLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // QuestionLabel
            // 
            QuestionLabel.AutoSize = true;
            QuestionLabel.Location = new System.Drawing.Point(12, 15);
            QuestionLabel.Name = "QuestionLabel";
            QuestionLabel.Size = new System.Drawing.Size(52, 13);
            QuestionLabel.TabIndex = 1;
            QuestionLabel.Text = "Question:";
            QuestionLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Username
            // 
            this.Username.Location = new System.Drawing.Point(127, 38);
            this.Username.MaxLength = 255;
            this.Username.Name = "Username";
            this.Username.Size = new System.Drawing.Size(200, 20);
            this.Username.TabIndex = 2;
            this.Username.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // Email
            // 
            this.Email.Location = new System.Drawing.Point(127, 64);
            this.Email.MaxLength = 255;
            this.Email.Name = "Email";
            this.Email.Size = new System.Drawing.Size(200, 20);
            this.Email.TabIndex = 3;
            this.Email.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // Server
            // 
            this.Server.Location = new System.Drawing.Point(127, 12);
            this.Server.MaxLength = 255;
            this.Server.Name = "Server";
            this.Server.Size = new System.Drawing.Size(200, 20);
            this.Server.TabIndex = 1;
            this.Server.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // Next
            // 
            this.Next.Enabled = false;
            this.Next.Location = new System.Drawing.Point(173, 146);
            this.Next.Name = "Next";
            this.Next.Size = new System.Drawing.Size(75, 23);
            this.Next.TabIndex = 6;
            this.Next.Text = "Next >";
            this.Next.UseVisualStyleBackColor = true;
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(92, 146);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 9;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // PassphraseLabel
            // 
            this.PassphraseLabel.AutoSize = true;
            this.PassphraseLabel.Location = new System.Drawing.Point(56, 93);
            this.PassphraseLabel.Name = "PassphraseLabel";
            this.PassphraseLabel.Size = new System.Drawing.Size(65, 13);
            this.PassphraseLabel.TabIndex = 8;
            this.PassphraseLabel.Text = "Passphrase:";
            this.PassphraseLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Passphrase
            // 
            this.Passphrase.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.Passphrase.Location = new System.Drawing.Point(127, 90);
            this.Passphrase.MaxLength = 255;
            this.Passphrase.Name = "Passphrase";
            this.Passphrase.PasswordChar = 'l';
            this.Passphrase.Size = new System.Drawing.Size(200, 20);
            this.Passphrase.TabIndex = 4;
            this.Passphrase.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // ConfirmPassphrase
            // 
            this.ConfirmPassphrase.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.ConfirmPassphrase.Location = new System.Drawing.Point(127, 116);
            this.ConfirmPassphrase.MaxLength = 255;
            this.ConfirmPassphrase.Name = "ConfirmPassphrase";
            this.ConfirmPassphrase.PasswordChar = 'l';
            this.ConfirmPassphrase.Size = new System.Drawing.Size(200, 20);
            this.ConfirmPassphrase.TabIndex = 5;
            this.ConfirmPassphrase.TextChanged += new System.EventHandler(this.TextChanged);
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Status.Location = new System.Drawing.Point(0, 177);
            this.Status.Name = "Status";
            this.Status.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Status.Size = new System.Drawing.Size(0, 16);
            this.Status.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(ServerLabel);
            this.panel1.Controls.Add(UsernameLabel);
            this.panel1.Controls.Add(this.ConfirmPassphrase);
            this.panel1.Controls.Add(this.Next);
            this.panel1.Controls.Add(EmailLabel);
            this.panel1.Controls.Add(this.Passphrase);
            this.panel1.Controls.Add(this.Username);
            this.panel1.Controls.Add(ConfirmPassphraseLabel);
            this.panel1.Controls.Add(this.Email);
            this.panel1.Controls.Add(this.PassphraseLabel);
            this.panel1.Controls.Add(this.Server);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(341, 193);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Finish);
            this.panel2.Controls.Add(QuestionLabel);
            this.panel2.Controls.Add(this.Question);
            this.panel2.Controls.Add(this.Answer);
            this.panel2.Controls.Add(this.AnswerLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(341, 193);
            this.panel2.TabIndex = 10;
            this.panel2.Visible = false;
            // 
            // Finish
            // 
            this.Finish.Enabled = false;
            this.Finish.Location = new System.Drawing.Point(173, 146);
            this.Finish.Name = "Finish";
            this.Finish.Size = new System.Drawing.Size(75, 23);
            this.Finish.TabIndex = 8;
            this.Finish.Text = "Finish";
            this.Finish.UseVisualStyleBackColor = true;
            this.Finish.Click += new System.EventHandler(this.Finish_Click);
            // 
            // Question
            // 
            this.Question.AutoSize = true;
            this.Question.Location = new System.Drawing.Point(70, 15);
            this.Question.Name = "Question";
            this.Question.Size = new System.Drawing.Size(0, 13);
            this.Question.TabIndex = 4;
            // 
            // Answer
            // 
            this.Answer.Location = new System.Drawing.Point(70, 38);
            this.Answer.Name = "Answer";
            this.Answer.Size = new System.Drawing.Size(257, 20);
            this.Answer.TabIndex = 7;
            this.Answer.TextChanged += new System.EventHandler(this.Answer_TextChanged);
            // 
            // AnswerLabel
            // 
            this.AnswerLabel.AutoSize = true;
            this.AnswerLabel.Location = new System.Drawing.Point(19, 41);
            this.AnswerLabel.Name = "AnswerLabel";
            this.AnswerLabel.Size = new System.Drawing.Size(45, 13);
            this.AnswerLabel.TabIndex = 2;
            this.AnswerLabel.Text = "Answer:";
            this.AnswerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ConfigurationWizard
            // 
            this.AcceptButton = this.Next;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(341, 193);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationWizard";
            this.Text = "PractiSES Configuration Wizard";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.TextBox Server;
        private System.Windows.Forms.Button Next;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label PassphraseLabel;
        private System.Windows.Forms.TextBox Passphrase;
        private System.Windows.Forms.TextBox ConfirmPassphrase;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button Finish;
        private System.Windows.Forms.TextBox Answer;
        private System.Windows.Forms.Label AnswerLabel;
        private System.Windows.Forms.Label Question;
    }
}

