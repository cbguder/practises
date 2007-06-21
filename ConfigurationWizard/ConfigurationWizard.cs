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

        public ConfigurationWizard()
        {
            InitializeComponent();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;

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
                        return;
                    }
                }

                SetStatus("Reading Server Key..."); 
                serverKey = File.ReadAllText(Path.Combine(core.ApplicationDataFolder, "server.key")); 
                
                SetStatus("Initializing Keys...");
                core.InitializeKeys(Passphrase.Text);

                SetStatus("Writing Identity...");
                StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
                sw.WriteLine(Username.Text);
                sw.WriteLine(Email.Text);
                sw.Close();

                SetStatus("Connecting to Server...");
                HttpClientChannel chan = new HttpClientChannel();
                ChannelServices.RegisterChannel(chan, false);
                server = (IServer)Activator.GetObject(typeof(IServer), "http://" + Server.Text + "/PractiSES");
                String questionsFromServer;

                try
                {
                    questionsFromServer = server.InitKeySet_AskQuestions(Username.Text, Email.Text);
                }
                catch (Exception ex)
                {
                    Error(ex.Message);
                    panel1.Enabled = true;
                    return;
                }

                Message questions = new Message(questionsFromServer);

                if (!questions.Verify(serverKey))
                {
                    Error("Message from server is tampered with.");
                    panel1.Enabled = true;
                    return;
                }

                Question.Text = questions.getCleartext();
                panel1.Visible = panel1.Enabled = false;
                panel2.Visible = panel2.Enabled = true;
                AcceptButton = Finish;
                Answer.Focus();
                SetStatus("");
            }
            else
            {
                MessageBox.Show("Server key not installed. Please download server key and install it.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                panel1.Enabled = true;
                return;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        new private void TextChanged(object sender, EventArgs e)
        {
            Next.Enabled = (Username.Text != "") && (Email.Text != "") && (Server.Text != "") && (Passphrase.Text != "") &&
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
            panel2.Enabled = false;

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
                server.InitKeySet_EnvelopeAnswers(Username.Text, Email.Text, encrypted);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
                panel2.Enabled = true;
                return;
            }

            SetStatus("Done.");

            MessageBox.Show(
                "Correct answers are sent successfully. Please check your email to finalize PractiSES initialization.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            Close();
        }

        private void Answer_TextChanged(object sender, EventArgs e)
        {
            Finish.Enabled = (Answer.Text != "");
        }
    }
}