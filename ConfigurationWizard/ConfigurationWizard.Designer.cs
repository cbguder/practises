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
            System.Windows.Forms.Label genericPassphraseLabel;
            System.Windows.Forms.Label passphraseServerLabel;
            this.Username = new System.Windows.Forms.TextBox();
            this.Email = new System.Windows.Forms.TextBox();
            this.Server = new System.Windows.Forms.TextBox();
            this.initializeNext = new System.Windows.Forms.Button();
            this.Back = new System.Windows.Forms.Button();
            this.PassphraseLabel = new System.Windows.Forms.Label();
            this.Passphrase = new System.Windows.Forms.TextBox();
            this.ConfirmPassphrase = new System.Windows.Forms.TextBox();
            this.Status = new System.Windows.Forms.Label();
            this.initializationPanel = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.removeButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.initializeButton = new System.Windows.Forms.Button();
            this.passphrasePanel = new System.Windows.Forms.Panel();
            this.passphraseServer = new System.Windows.Forms.TextBox();
            this.passphraseNext = new System.Windows.Forms.Button();
            this.genericPassphrase = new System.Windows.Forms.TextBox();
            this.questionsPanel = new System.Windows.Forms.Panel();
            this.Finish = new System.Windows.Forms.Button();
            this.Question = new System.Windows.Forms.Label();
            this.Answer = new System.Windows.Forms.TextBox();
            this.AnswerLabel = new System.Windows.Forms.Label();
            UsernameLabel = new System.Windows.Forms.Label();
            EmailLabel = new System.Windows.Forms.Label();
            ServerLabel = new System.Windows.Forms.Label();
            ConfirmPassphraseLabel = new System.Windows.Forms.Label();
            QuestionLabel = new System.Windows.Forms.Label();
            genericPassphraseLabel = new System.Windows.Forms.Label();
            passphraseServerLabel = new System.Windows.Forms.Label();
            this.initializationPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.passphrasePanel.SuspendLayout();
            this.questionsPanel.SuspendLayout();
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
            // genericPassphraseLabel
            // 
            genericPassphraseLabel.AutoSize = true;
            genericPassphraseLabel.Location = new System.Drawing.Point(39, 35);
            genericPassphraseLabel.Name = "genericPassphraseLabel";
            genericPassphraseLabel.Size = new System.Drawing.Size(65, 13);
            genericPassphraseLabel.TabIndex = 0;
            genericPassphraseLabel.Text = "Passphrase:";
            genericPassphraseLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // passphraseServerLabel
            // 
            passphraseServerLabel.AutoSize = true;
            passphraseServerLabel.Location = new System.Drawing.Point(12, 9);
            passphraseServerLabel.Name = "passphraseServerLabel";
            passphraseServerLabel.Size = new System.Drawing.Size(92, 13);
            passphraseServerLabel.TabIndex = 3;
            passphraseServerLabel.Text = "PractiSES Server:";
            passphraseServerLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            // initializeNext
            // 
            this.initializeNext.Enabled = false;
            this.initializeNext.Location = new System.Drawing.Point(173, 146);
            this.initializeNext.Name = "initializeNext";
            this.initializeNext.Size = new System.Drawing.Size(75, 23);
            this.initializeNext.TabIndex = 6;
            this.initializeNext.Text = "Next >";
            this.initializeNext.UseVisualStyleBackColor = true;
            this.initializeNext.Click += new System.EventHandler(this.initializeNext_Click);
            // 
            // Back
            // 
            this.Back.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Back.Location = new System.Drawing.Point(92, 146);
            this.Back.Name = "Back";
            this.Back.Size = new System.Drawing.Size(75, 23);
            this.Back.TabIndex = 9;
            this.Back.Text = "< Back";
            this.Back.UseVisualStyleBackColor = true;
            this.Back.Click += new System.EventHandler(this.Back_Click);
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
            // initializationPanel
            // 
            this.initializationPanel.Controls.Add(ServerLabel);
            this.initializationPanel.Controls.Add(UsernameLabel);
            this.initializationPanel.Controls.Add(this.ConfirmPassphrase);
            this.initializationPanel.Controls.Add(this.initializeNext);
            this.initializationPanel.Controls.Add(EmailLabel);
            this.initializationPanel.Controls.Add(this.Passphrase);
            this.initializationPanel.Controls.Add(this.Username);
            this.initializationPanel.Controls.Add(ConfirmPassphraseLabel);
            this.initializationPanel.Controls.Add(this.Email);
            this.initializationPanel.Controls.Add(this.PassphraseLabel);
            this.initializationPanel.Controls.Add(this.Server);
            this.initializationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initializationPanel.Enabled = false;
            this.initializationPanel.Location = new System.Drawing.Point(0, 0);
            this.initializationPanel.Name = "initializationPanel";
            this.initializationPanel.Size = new System.Drawing.Size(341, 193);
            this.initializationPanel.TabIndex = 0;
            this.initializationPanel.Tag = "Key Initialization";
            this.initializationPanel.Visible = false;
            this.initializationPanel.VisibleChanged += new System.EventHandler(this.initializationPanel_VisibleChanged);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.removeButton);
            this.mainPanel.Controls.Add(this.updateButton);
            this.mainPanel.Controls.Add(this.initializeButton);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(341, 177);
            this.mainPanel.TabIndex = 10;
            this.mainPanel.Tag = "";
            this.mainPanel.VisibleChanged += new System.EventHandler(this.mainPanel_VisibleChanged);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(14, 126);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(312, 48);
            this.removeButton.TabIndex = 2;
            this.removeButton.Text = "Key Removal";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(14, 72);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(312, 48);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "Key Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(14, 18);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(312, 48);
            this.initializeButton.TabIndex = 0;
            this.initializeButton.Text = "Key Initialization";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // passphrasePanel
            // 
            this.passphrasePanel.Controls.Add(this.passphraseServer);
            this.passphrasePanel.Controls.Add(passphraseServerLabel);
            this.passphrasePanel.Controls.Add(this.passphraseNext);
            this.passphrasePanel.Controls.Add(this.genericPassphrase);
            this.passphrasePanel.Controls.Add(genericPassphraseLabel);
            this.passphrasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passphrasePanel.Enabled = false;
            this.passphrasePanel.Location = new System.Drawing.Point(0, 0);
            this.passphrasePanel.Name = "passphrasePanel";
            this.passphrasePanel.Size = new System.Drawing.Size(341, 177);
            this.passphrasePanel.TabIndex = 1;
            this.passphrasePanel.Tag = "Passphrase";
            this.passphrasePanel.Visible = false;
            this.passphrasePanel.VisibleChanged += new System.EventHandler(this.passphrasePanel_VisibleChanged);
            // 
            // passphraseServer
            // 
            this.passphraseServer.Location = new System.Drawing.Point(110, 6);
            this.passphraseServer.Name = "passphraseServer";
            this.passphraseServer.Size = new System.Drawing.Size(219, 20);
            this.passphraseServer.TabIndex = 0;
            this.passphraseServer.TextChanged += new System.EventHandler(this.genericPassphrase_TextChanged);
            // 
            // passphraseNext
            // 
            this.passphraseNext.Enabled = false;
            this.passphraseNext.Location = new System.Drawing.Point(173, 146);
            this.passphraseNext.Name = "passphraseNext";
            this.passphraseNext.Size = new System.Drawing.Size(75, 23);
            this.passphraseNext.TabIndex = 2;
            this.passphraseNext.Text = "Next >";
            this.passphraseNext.UseVisualStyleBackColor = true;
            this.passphraseNext.Click += new System.EventHandler(this.passphraseNext_Click);
            // 
            // genericPassphrase
            // 
            this.genericPassphrase.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.genericPassphrase.Location = new System.Drawing.Point(110, 32);
            this.genericPassphrase.MaxLength = 255;
            this.genericPassphrase.Name = "genericPassphrase";
            this.genericPassphrase.PasswordChar = 'l';
            this.genericPassphrase.Size = new System.Drawing.Size(219, 20);
            this.genericPassphrase.TabIndex = 1;
            this.genericPassphrase.TextChanged += new System.EventHandler(this.genericPassphrase_TextChanged);
            // 
            // questionsPanel
            // 
            this.questionsPanel.Controls.Add(this.Finish);
            this.questionsPanel.Controls.Add(QuestionLabel);
            this.questionsPanel.Controls.Add(this.Question);
            this.questionsPanel.Controls.Add(this.Answer);
            this.questionsPanel.Controls.Add(this.AnswerLabel);
            this.questionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.questionsPanel.Enabled = false;
            this.questionsPanel.Location = new System.Drawing.Point(0, 0);
            this.questionsPanel.Name = "questionsPanel";
            this.questionsPanel.Size = new System.Drawing.Size(341, 193);
            this.questionsPanel.TabIndex = 10;
            this.questionsPanel.Tag = "Server Questions";
            this.questionsPanel.Visible = false;
            this.questionsPanel.VisibleChanged += new System.EventHandler(this.questionsPanel_VisibleChanged);
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
            this.AcceptButton = this.initializeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Back;
            this.ClientSize = new System.Drawing.Size(341, 193);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.Back);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.passphrasePanel);
            this.Controls.Add(this.initializationPanel);
            this.Controls.Add(this.questionsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationWizard";
            this.Tag = "PractiSES Configuration Wizard";
            this.Text = "PractiSES Configuration Wizard";
            this.initializationPanel.ResumeLayout(false);
            this.initializationPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.passphrasePanel.ResumeLayout(false);
            this.passphrasePanel.PerformLayout();
            this.questionsPanel.ResumeLayout(false);
            this.questionsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Username;
        private System.Windows.Forms.TextBox Email;
        private System.Windows.Forms.TextBox Server;
        private System.Windows.Forms.Button initializeNext;
        private System.Windows.Forms.Button Back;
        private System.Windows.Forms.Label PassphraseLabel;
        private System.Windows.Forms.TextBox Passphrase;
        private System.Windows.Forms.TextBox ConfirmPassphrase;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Panel initializationPanel;
        private System.Windows.Forms.Panel questionsPanel;
        private System.Windows.Forms.Button Finish;
        private System.Windows.Forms.TextBox Answer;
        private System.Windows.Forms.Label AnswerLabel;
        private System.Windows.Forms.Label Question;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.Panel passphrasePanel;
        private System.Windows.Forms.TextBox genericPassphrase;
        private System.Windows.Forms.Button passphraseNext;
        private System.Windows.Forms.TextBox passphraseServer;
    }
}

