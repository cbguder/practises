/*
 * $Id$
 */

using System;
using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Security.Cryptography;
using System.Text;

namespace PractiSES
{
    public class Client
    {
        private String host;
        private IServer server;
        private Core core;
        private String serverKey;

        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return 0;
            }

            String[][] options =
                Util.Getopt(args, "dehH:O:p:r:sv",
                            new String[]
                                {
                                    "--help", "--initialize", "--finalize-initialize", "--update", "--finalize-update",
                                    "--remove", "--finalize-remove", "--strip", "--sign-detached", "--verify-detached"
                                });
            String file = args[args.Length - 1];
            String host = "practises2.no-ip.org";
            String passphrase = null;
            String recipient = null;
            String command = "help";            
            String outfile = null;

            foreach (String[] item in options)
            {
                switch (item[0])
                {
                    case "-d":
                        command = "decrypt";
                        break;
                    case "-e":
                        command = "encrypt";
                        break;
                    case "-h":
                        command = "help";
                        break;
                    case "-H":
                        host = item[1];
                        break;
                    case "-O":
                        outfile = item[1];
                        break;
                    case "-p":
                        passphrase = item[1];
                        break;
                    case "-r":
                        recipient = item[1];
                        break;
                    case "-s":
                        command = "sign";
                        break;
                    case "-v":
                        command = "verify";
                        break;
                    case "--initialize":
                        command = "initialize";
                        break;
                    case "--finalize-initialize":
                        command = "finalizeInitialize";
                        break;
                    case "--update":
                        command = "update";
                        break;
                    case "--finalize-update":
                        command = "finalizeUpdate";
                        break;
                    case "--remove":
                        command = "remove";
                        break;
                    case "--finalize-remove":
                        command = "finalizeRemove";
                        break;
                    case "--sign-detached":
                        command = "signDetached";
                        break;
                    case "--verify-detached":
                        command = "verifyDetached";
                        break;
                    case "--strip":
                        command = "strip";
                        break;
                }
            }

            Client client = new Client(host);

            if (outfile == null)
            {
                outfile = file + ".pses";
            }

            switch (command)
            {
                case "encrypt":
                    client.Encrypt(file, recipient, outfile);
                    break;
                case "decrypt":
                    client.Decrypt(file, passphrase, outfile);
                    break;
                case "sign":
                    client.Sign(file, passphrase);
                    break;
                case "verify":
                    return client.Verify(file, recipient);
                case "initialize":
                    client.Initialize(passphrase);
                    break;
                case "finalizeInitialize":
                    return client.FinalizeInitialize(file, passphrase);
                case "update":
                    client.Update(passphrase);
                    break;
                case "finalizeUpdate":
                    return client.FinalizeUpdate(file, passphrase);
                case "remove":
                    client.Remove(passphrase);
                    break;
                case "finalizeRemove":
                    return client.FinalizeRemove(file, passphrase);
                case "strip":
                    Strip(file);
                    break;
                case "signDetached":
                    client.SignDetached(file, passphrase, outfile);
                    break;
                case "verifyDetached":
                    return client.VerifyDetached(file, recipient);
                case "help":
                    Usage();
                    break;
            }

            return 0;
        }

        public Client(String host)
        {
            this.host = host;
            core = new Core("", false);
            serverKey = File.ReadAllText(Path.Combine(core.ApplicationDataFolder, "server.key"));
        }

        private bool Connect()
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.Error.WriteLine("Connecting to PractiSES server ({0})...", host);

            server = (IServer) Activator.GetObject(typeof (IServer), "http://" + host + "/PractiSES");

            return true;
        }

/*
        private void WriteIdentity(String username, String email)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
            sw.WriteLine(username);
            sw.WriteLine(email);
            sw.Close();
        }
*/

        private void Encrypt(String filename, String recipient, String outfile)
        {
            String publicKey = FetchPublicKey(recipient);
            if (publicKey == null)
                return;

            Message message = new Message(File.ReadAllBytes(filename));
            message.Encrypt(publicKey);

            if (Util.Write(outfile, message.ToString()))
            {
                Console.Error.WriteLine("Output written to {0}", outfile);
            }
        }

        private void Decrypt(String filename, String passphrase, String outfile)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return;
            }

            Message message = new Message(File.ReadAllText(filename));
            message.Decrypt(core.PrivateKey);

            if (Util.Write(outfile, message.Cleartext))
            {
                Console.Error.WriteLine("Output written to {0}", outfile);
            }
        }

        private void Sign(String filename, String passphrase)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return;
            }

            String outFile = filename + ".pses";

            Message message = new Message(File.ReadAllText(filename, Encoding.UTF8));
            message.Sign(core.PrivateKey);

            if (Util.Write(outFile, message.ToString()))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        private void SignDetached(String filename, String passphrase, String outfile)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return;
            }

            Message message = new Message(File.ReadAllBytes(filename));
            message.Sign(core.PrivateKey, false);

            if (Util.Write(outfile, message.getSignature(), true))
            {
                Console.Error.WriteLine("Output written to {0}", outfile);
            }
        }

        private int Verify(String filename, String sender)
        {
            Message message;

            try
            {
                message = new Message(File.ReadAllText(filename, Encoding.UTF8));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 2;
            }

            if (Verify(message, sender, true))
                return 0;
            else
                return 1;
        }

        private int VerifyDetached(String filename, String sender)
        {
            Message message = new Message(File.ReadAllBytes(filename));
            String[] siglines = Util.GetLines(File.ReadAllText(filename + ".pses"));
            message.Signature = Convert.FromBase64String(String.Join("", siglines, 1, siglines.Length - 2));

            if (Verify(message, sender, false))
                return 0;
            else
                return 1;
        }

        private bool Verify(Message message, String sender, bool includeComments)
        {
            String publicKey = FetchPublicKey(sender);

            if (publicKey == null)
                return false;

            bool result = message.Verify(publicKey, includeComments);

            if (result)
                Console.Error.WriteLine("Message verification succeeded.");
            else
                Console.Error.WriteLine("Message verification failed.");

            return result;
        }

        private void Initialize(String passphrase)
        {
            if (File.Exists(core.KeyFile))
            {
                Console.Write("Are you ABSOLUTELY sure that you want to delete your existing keys FOREVER? (y/N): ");
                String response = Console.ReadLine();
                response.Trim();

                if (response == "y")
                    File.Delete(core.KeyFile);
                else
                    return;
            }

            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return;
            }

            Console.Write("Username: ");
            String username = Console.ReadLine();
            Console.Write("Email: ");
            String email = Console.ReadLine();

            StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
            sw.WriteLine(username);
            sw.WriteLine(email);
            sw.Close();

            Connect();
            String questionsFromServer;

            try
            {
                questionsFromServer = server.InitKeySet_AskQuestions(username, email);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            Message questions = new Message(questionsFromServer);

            if (!questions.Verify(serverKey))
            {
                Console.Error.WriteLine("WARNING: Message from server is tampered with.");
                Console.Error.WriteLine(questionsFromServer);
                return;
            }

            Console.WriteLine("Questions:");
            Console.WriteLine(questions.getCleartext());
            Console.Write("Answers: ");
            String answers = Console.ReadLine();

            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"),
                               (byte[]) key.ToArray(Type.GetType("System.Byte")));

            try
            {
                server.InitKeySet_EnvelopeAnswers(username, email, encrypted);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES initialization.");
        }

        private int FinalizeInitialize(String filename, String passphrase)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return 1;
            }

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            username.Trim();
            email.Trim();

            Connect();

            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key")));
            AESInfo info = new AESInfo();
            info.key = (byte[]) key.GetRange(0, Crypto.AESKeySize/8).ToArray(Type.GetType("System.Byte"));
            info.IV =
                (byte[]) key.GetRange(Crypto.AESKeySize/8, Crypto.AESIVSize/8).ToArray(Type.GetType("System.Byte"));

            Rijndael aes = Rijndael.Create();

            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass =
                Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(core.PublicKey));

            try
            {
                if (server.InitKeySet_SendPublicKey(username, email, core.PublicKey, Convert.ToBase64String(hash)))
                {
                    Console.WriteLine("Public key successfully sent.");
                    File.Delete(Path.Combine(core.ApplicationDataFolder, "answers"));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            return 0;
        }

        private void Update(String passphrase)
        {
            File.Delete(core.KeyFile);
            core.InitializeKeys(passphrase);

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            String questions;
            Connect();

            try
            {
                questions = server.USKeyUpdate_AskQuestions(username, email);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            String strippedQuestions = Crypto.StripMessage(questions);
            Console.WriteLine("Questions:");
            Console.WriteLine(strippedQuestions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"),
                               (byte[]) key.ToArray(Type.GetType("System.Byte")));

            try
            {
                server.USKeyUpdate_EnvelopeAnswers(username, email, encrypted);
                Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES key update.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        private int FinalizeUpdate(String filename, String passphrase)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return 1;
            }

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            username.Trim();
            email.Trim();

            Connect();

            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key")));
            AESInfo info = new AESInfo();
            info.key = (byte[]) key.GetRange(0, Crypto.AESKeySize/8).ToArray(Type.GetType("System.Byte"));
            info.IV =
                (byte[]) key.GetRange(Crypto.AESKeySize/8, Crypto.AESIVSize/8).ToArray(Type.GetType("System.Byte"));

            Rijndael aes = Rijndael.Create();

            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass =
                Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(core.PublicKey));

            if (server.USKeyUpdate_SendPublicKey(username, email, core.PublicKey, Convert.ToBase64String(hash)))
            {
                Console.WriteLine("Public key successfully sent.");
            }
            else
            {
                Console.WriteLine("Public key could not be sent, please try again.");
            }

            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            return 0;
        }

        private void Remove(String passphrase)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return;
            }

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            Connect();
            String questions;

            try
            {
                questions = server.USKeyRem_AskQuestions(username, email);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return;
            }

            String strippedQuestions = Crypto.StripMessage(questions);
            Console.WriteLine("Questions:");
            Console.WriteLine(strippedQuestions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"),
                               (byte[]) key.ToArray(Type.GetType("System.Byte")));

            try
            {
                server.USKeyRem_EnvelopeAnswers(username, email, encrypted);
                Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES key removal.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        private int FinalizeRemove(String filename, String passphrase)
        {
            try
            {
                core.InitializeKeys(passphrase);
            }
            catch
            {
                Console.Error.WriteLine("Invalid passphrase");
                return 1;
            }

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            username.Trim();
            email.Trim();

            Connect();

            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key")));
            AESInfo info = new AESInfo();
            info.key = (byte[]) key.GetRange(0, Crypto.AESKeySize/8).ToArray(Type.GetType("System.Byte"));
            info.IV =
                (byte[]) key.GetRange(Crypto.AESKeySize/8, Crypto.AESIVSize/8).ToArray(Type.GetType("System.Byte"));

            Rijndael aes = Rijndael.Create();

            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass =
                Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("I want to remove my current public key"));

            try
            {
                if (server.USKeyRem_SendRemoveRequest(username, email, Convert.ToBase64String(hash)))
                {
                    Console.WriteLine("Removal request successfully sent.");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            return 0;
        }

        private static void Strip(String filename)
        {
            String message = File.ReadAllText(filename);
            Console.WriteLine(Crypto.StripMessage(message));
        }

        private static void Usage()
        {
            Console.Error.WriteLine("Usage: practises command filename");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Commands:");
            Console.Error.WriteLine("    -d                  Decrypt");
            Console.Error.WriteLine("    -e                  Encrypt");
            Console.Error.WriteLine("    -s                  Sign");
            Console.Error.WriteLine("    -v                  Verify");
            Console.Error.WriteLine("        --initialize");
            Console.Error.WriteLine("        --remove");
            Console.Error.WriteLine("        --update");
        }

        private String FetchPublicKey(String userID)
        {
            String publicKey;

            while (userID == null || userID == "")
            {
                Console.Write("Sender: ");
                userID = Console.ReadLine();
                userID.Trim();
            }

            try
            {
                Connect();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: {0}", e.Message);
                return null;
            }

            try
            {
                publicKey = server.KeyObt(userID);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: {0}", e.Message);
                return null;
            }

            Message message = new Message(publicKey);

            if (message.Verify(serverKey))
            {
                return Encoding.UTF8.GetString(message.Cleartext);
            }
            else
            {
                Console.Error.WriteLine("WARNING: Message from server is tampered with.");
                return null;
            }
        }
    }
}