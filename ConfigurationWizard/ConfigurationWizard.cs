using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace PractiSES
{
    public partial class ConfigurationWizard : Form
    {
        private Core core;
        private IServer server;
        private String serverKey;
        private Panel currentPanel;
        private ArrayList pastPanels;
        private String username;
        private String email;
        private String mode;
        private String serverURL;

        public ConfigurationWizard()
        {
            InitializeComponent();
            pastPanels = new ArrayList();
            pastPanels.Add(mainPanel);
            currentPanel = mainPanel;
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);
        }

        new private void TextChanged(object sender, EventArgs e)
        {
            initializeNext.Enabled = (Username.Text != "") && (Email.Text != "") && (Server.Text != "") && (Passphrase.Text != "") &&
                           (ConfirmPassphrase.Text != "") && (Passphrase.Text == ConfirmPassphrase.Text);
        }

        private void SetStatus(String status)
        {
            Status.Text = status;
            Refresh();
        }

        private void Error(String message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            SetStatus("");
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            questionsPanel.Enabled = false;

            SetStatus("Preparing Answers...");

            String answers = Answer.Text;
            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"),
               (byte[])key.ToArray(Type.GetType("System.Byte")));

            SetStatus("Sending Answers...");
            try
            {
                switch (mode)
                {
                    case "initialize":
                        server.InitKeySet_EnvelopeAnswers(Username.Text, Email.Text, encrypted);
                        break;
                    case "update":
                        server.USKeyUpdate_EnvelopeAnswers(username, email, encrypted);
                        break;
                    case "remove":
                        server.USKeyRem_EnvelopeAnswers(username, email, encrypted);
                        break;
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                questionsPanel.Enabled = true;
                return;
            }

            SetStatus("Done.");

            MessageBox.Show(
                "Correct answers are sent successfully. Please check your email to finalize this action.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Close();
        }

        private void Answer_TextChanged(object sender, EventArgs e)
        {
            Finish.Enabled = (Answer.Text != "");
        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            mode = "initialize";
            switchPanel(initializationPanel);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            mode = "update";
            switchPanel(passphrasePanel);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            mode = "remove";
            switchPanel(passphrasePanel);
        }

        private void switchPanel(Panel newPanel)
        {
            pastPanels.Add(currentPanel);
            currentPanel.Enabled = false;
            currentPanel.Visible = false;
            currentPanel = newPanel;
            newPanel.Enabled = true;
            newPanel.Visible = true;

            if ((String)newPanel.Tag == "")
            {
                this.Text = (String)this.Tag;
            }
            else
            {
                this.Text = (String)this.Tag + ": " + (String)newPanel.Tag;
            }

            Back.Enabled = (currentPanel != mainPanel);
        }

        private void Back_Click(object sender, EventArgs e)
        {
            Panel prevPanel = (Panel)pastPanels[pastPanels.Count - 1];
            currentPanel.Enabled = false;
            currentPanel.Visible = false;
            currentPanel = prevPanel;
            prevPanel.Enabled = true;
            prevPanel.Visible = true;
            pastPanels.RemoveAt(pastPanels.Count - 1);
            
            if ((String)prevPanel.Tag == "")
            {
                this.Text = (String)this.Tag;
            }
            else
            {
                this.Text = (String)this.Tag + ": " + (String)prevPanel.Tag;
            }

            Back.Enabled = (currentPanel != mainPanel);
        }

        private void initializeNext_Click(object sender, EventArgs e)
        {
            serverURL = Server.Text;

            preNext();

            String questionsFromServer;
            try
            {
                questionsFromServer = server.InitKeySet_AskQuestions(username, email);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                currentPanel.Enabled = true;
                return;
            }

            postNext(questionsFromServer);
        }

        private void passphraseNext_Click(object sender, EventArgs e)
        {
            serverURL = passphraseServer.Text;

            preNext();

            String questionsFromServer = "";

            try
            {
                if (mode == "update")
                    questionsFromServer = server.USKeyUpdate_AskQuestions(username, email);
                else if (mode == "remove")
                    questionsFromServer = server.USKeyRem_AskQuestions(username, email);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                currentPanel.Enabled = true;
                return;
            }

            postNext(questionsFromServer);
        }

        private void genericPassphrase_TextChanged(object sender, EventArgs e)
        {
            passphraseNext.Enabled = (genericPassphrase.Text != "") && (passphraseServer.Text != "");
        }

        private void mainPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (mainPanel.Visible)
            {
                initializeButton.Focus();
                AcceptButton = initializeButton;
            }
        }

        private void initializationPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (initializationPanel.Visible)
            {
                Server.Focus();
                AcceptButton = initializeNext;
            }
        }

        private void questionsPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (questionsPanel.Visible)
            {
                Answer.Focus();
                AcceptButton = Finish;
            }
        }

        private void passphrasePanel_VisibleChanged(object sender, EventArgs e)
        {
            if (passphrasePanel.Visible)
            {
                passphraseServer.Focus();
                AcceptButton = passphraseNext;
            }
        }

        private void preNext()
        {
            currentPanel.Enabled = false;

            core = new Core("", false);

            if (File.Exists(Path.Combine(core.ApplicationDataFolder, "server.key")))
            {
                if (File.Exists(core.KeyFile))
                {
                    if (
                        MessageBox.Show("Are you ABSOLUTELY sure that you want to delete your existing keys FOREVER?",
                                        "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        File.Delete(core.KeyFile);
                    }
                    else
                    {
                        currentPanel.Enabled = true;
                        return;
                    }
                }

                SetStatus("Reading Server Key...");
                serverKey = File.ReadAllText(Path.Combine(core.ApplicationDataFolder, "server.key"));

                SetStatus("Initializing Keys...");
                String passphrase = "";
                
                if (currentPanel == initializationPanel)
                    passphrase = Passphrase.Text;
                else if (currentPanel == passphrasePanel)
                    passphrase = genericPassphrase.Text;

                core.InitializeKeys(passphrase);

                if (currentPanel == initializationPanel)
                {
                    SetStatus("Writing Identity...");
                    username = Username.Text;
                    email = Email.Text;
                    
                    StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
                    sw.WriteLine(username);
                    sw.WriteLine(email);
                    sw.Close();
                }
                else if(currentPanel == passphrasePanel)
                {
                    SetStatus("Reading Identity...");
                    StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
                    username = sr.ReadLine();
                    email = sr.ReadLine();
                    sr.Close();
                }

                SetStatus("Connecting to Server...");
                server = (IServer)Activator.GetObject(typeof(IServer), "http://" + serverURL + "/PractiSES");
            }
            else
            {
                MessageBox.Show("Server key not installed. Please download server key and install it.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentPanel.Enabled = true;
                return;
            }
        }

        private void postNext(String questionsFromServer)
        {
            Message questions = new Message(questionsFromServer);

            if (!questions.Verify(serverKey))
            {
                Error("Message from server is tampered with.");
                currentPanel.Enabled = true;
                return;
            }

            Question.Text = questions.getCleartext();
            switchPanel(questionsPanel);
            SetStatus("");
        }
    }
}