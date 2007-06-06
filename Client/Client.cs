/*
 * $Id$
 */

using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

namespace PractiSES
{
    class Client
    {
        private String host;
        private IServer server;
        private Core core;
        private String serverKey;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            String[][] options = Util.Getopt(args, "dehH:O:p:r:sv", new String[] { "--help", "--initialize", "--finalize-initialize", "--update", "--finalize-update", "--remove",  "--finalize-remove", "--strip", "--sign-detached", "--verify-detached"});
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

            switch (command)
            {
                case "encrypt":
                    client.Encrypt(file, recipient);
                    break;
                case "decrypt":
                    client.Decrypt(file, passphrase);
                    break;
                case "sign":
                    client.Sign(file, passphrase);
                    break;
                case "verify":
                    client.Verify(file, recipient);
                    break;
                case "initialize":
                    client.Initialize(passphrase);
                    break;
                case "finalizeInitialize":
                    client.FinalizeInitialize(file, passphrase);
                    break;
                case "update":
                    client.Update(passphrase);
                    break;
                case "finalizeUpdate":
                    client.FinalizeUpdate(file, passphrase);
                    break;
                case "remove":
                    client.Remove(passphrase);
                    break;
                case "finalizeRemove":
                    client.FinalizeRemove(file, passphrase);
                    break;
                case "strip":
                    client.Strip(file);
                    break;
                case "signDetached":
                    client.SignDetached(file, passphrase, outfile);
                    break;
                case "verifyDetached":
                    client.VerifyDetached(file, recipient);
                    break;
                case "help":
                    Usage();
                    break;
            }

            return;
        }

        public Client(String host)
        {
            this.host = host;
            this.core = new Core("", false);
            this.serverKey = File.ReadAllText(Path.Combine(core.ApplicationDataFolder, "server.key"));
        }

        private bool Connect(String host)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.Error.WriteLine("Connecting to PractiSES server ({0})...", host);

            server = (IServer)Activator.GetObject(typeof(IServer), "http://" + host + "/PractiSES");

            return true;
        }

        private bool Write(String path, byte[] contents)
        {
            return this.Write(path, contents, false);
        }

        private bool Write(String path, byte[] contents, Boolean overwrite)
        {
            bool write = true;

            if (!overwrite && File.Exists(path))
            {
                Console.Write("{0} exists, overwrite? (y/N): ", path);
                String response = Console.ReadLine();
                response.Trim();

                if (response != "y")
                {
                    write = false;
                }
            }

            if (write)
            {
                File.WriteAllBytes(path, contents);
                return true;
            }

            return false;
        }

        private bool Write(String path, String contents)
        {
            return Write(path, Encoding.UTF8.GetBytes(contents));
        }

        private bool Write(String path, String contents, Boolean overwrite)
        {
            return Write(path, Encoding.UTF8.GetBytes(contents), overwrite);
        }

        private void WriteIdentity(String username, String email)
        {
            StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
            sw.WriteLine(username);
            sw.WriteLine(email);
            sw.Close();
        }
        
        private void Encrypt(String filename, String recipient)
        {
            String outFile = filename + ".pses";

            String publicKey = FetchPublicKey(recipient);
            if (publicKey == null)
                return;

            Message message = new Message(File.ReadAllBytes(filename));
            message.Encrypt(publicKey);

            if (Write(outFile, message.ToString()))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        private void Decrypt(String filename, String passphrase)
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

            String outFile = filename + ".pses";

            if (Write(outFile, message.Cleartext))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
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

            if (Write(outFile, message.ToString()))
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

            byte[] data = File.ReadAllBytes(filename);
            String signature = Crypto.SignDetached(data, core.PrivateKey);

            if (outfile == null)
            {
                outfile = filename + ".pses";
            }

            if (Write(outfile, signature, true))
            {
                Console.Error.WriteLine("Output written to {0}", outfile);
            }
        }

        private void Verify(String filename, String sender)
        {
            Message message;

            try
            {
                message = new Message(File.ReadAllText(filename, Encoding.UTF8));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            Verify(message, sender);
        }

        private void VerifyDetached(String filename, String sender)
        {
            Message message = new Message(File.ReadAllBytes(filename));
            String[] siglines = Util.GetLines(File.ReadAllText(filename + ".pses"));
            message.Signature = Convert.FromBase64String(String.Join("", siglines, 1, siglines.Length - 2));
            Verify(message, sender);
        }

        private void Verify(Message message, String sender)
        {
            String publicKey = FetchPublicKey(sender);

            if (publicKey == null)
                return;

            bool result = message.Verify(publicKey);

            if (result)
                Console.WriteLine("Message verification succeeded.");
            else
                Console.WriteLine("Message verification failed.");
        }

        private void Initialize(String passphrase)
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

            Console.Write("Username: ");
            String username = Console.ReadLine();
            Console.Write("Email: ");
            String email = Console.ReadLine();

            StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
            sw.WriteLine(username);
            sw.WriteLine(email);
            sw.Close();
            
            Connect(host);

            Message questions = new Message(server.InitKeySet_AskQuestions(username, email));

            if (!questions.Verify(serverKey))
            {
                Console.Error.WriteLine("WARNING: Message from server is tampered with.");
                return;
            }

            Console.WriteLine("Questions:");
            Console.WriteLine(questions.getCleartext());
            Console.Write("Answers: ");
            String answers = Console.ReadLine();

            Message reply = new Message(answers);
            reply.Encrypt(serverKey);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers"), reply.Ciphertext);

            try
            {
                server.InitKeySet_EnvelopeAnswers(username, email, reply.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return;
            }

            Console.Error.WriteLine("Answers could not be sent. Please check your email to finalize PractiSES initialization.");
        }

        private void FinalizeInitialize(String filename, String passphrase)
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

            username.Trim();
            email.Trim();

            Connect(host);

            byte[] answers = File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers"));
            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers")));

            AESInfo info = new AESInfo();
            info.key = (byte[])key.GetRange(0, Crypto.AESKeySize / 8).ToArray(Type.GetType("System.Byte"));
            info.IV = (byte[])key.GetRange(Crypto.AESKeySize / 8, Crypto.AESIVSize / 8).ToArray(Type.GetType("System.Byte"));
            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            Rijndael aes = Rijndael.Create();
            
            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass = Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));
            String abik = Convert.ToBase64String(macpass);

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(core.PublicKey));

            if (server.InitKeySet_SendPublicKey(username, email, core.PublicKey, Convert.ToBase64String(hash)))
            {
                Console.WriteLine("Public key successfully sent.");
            }
            else
            {
                Console.WriteLine("Public key could not be sent, please try again.");
            }
        }

        private void Update(String passphrase)
        {
            File.Delete(core.KeyFile);
            core.InitializeKeys(passphrase);

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            Connect(host);

            String questions = server.USKeyUpdate_AskQuestions(username, email);
            String strippedQuestions = Crypto.StripMessage(questions);
            Console.WriteLine("Questions:");
            Console.WriteLine(strippedQuestions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            String serverPublicKey = server.KeyObt("server");
            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverPublicKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"), (byte[])key.ToArray(Type.GetType("System.Byte")));

            server.USKeyUpdate_EnvelopeAnswers(username, email, encrypted);

            Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES key update.");
        }

        private void FinalizeUpdate(String filename, String passphrase)
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

            username.Trim();
            email.Trim();

            Connect(host);

            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key")));
            AESInfo info = new AESInfo();
            info.key = (byte[])key.GetRange(0, Crypto.AESKeySize / 8).ToArray(Type.GetType("System.Byte"));
            info.IV = (byte[])key.GetRange(Crypto.AESKeySize / 8, Crypto.AESIVSize / 8).ToArray(Type.GetType("System.Byte"));
            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            Rijndael aes = Rijndael.Create();

            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass = Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));
            String abik = Convert.ToBase64String(macpass);

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

            Connect(host);

            String questions = server.USKeyRem_AskQuestions(username, email);
            String strippedQuestions = Crypto.StripMessage(questions);
            Console.WriteLine("Questions:");
            Console.WriteLine(strippedQuestions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            String serverPublicKey = server.KeyObt("server");
            byte[] message = Encoding.UTF8.GetBytes(answers);
            Rijndael aes = Rijndael.Create();
            String encrypted = Crypto.Encrypt(message, serverPublicKey, aes);

            ArrayList key = new ArrayList();
            key.AddRange(aes.Key);
            key.AddRange(aes.IV);

            File.WriteAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key"), (byte[])key.ToArray(Type.GetType("System.Byte")));

            server.USKeyRem_EnvelopeAnswers(username, email, encrypted);

            Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES key removal.");
        }

        private void FinalizeRemove(String filename, String passphrase)
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

            username.Trim();
            email.Trim();

            Connect(host);

            ArrayList key = new ArrayList(File.ReadAllBytes(Path.Combine(core.ApplicationDataFolder, "answers.key")));
            AESInfo info = new AESInfo();
            info.key = (byte[])key.GetRange(0, Crypto.AESKeySize / 8).ToArray(Type.GetType("System.Byte"));
            info.IV = (byte[])key.GetRange(Crypto.AESKeySize / 8, Crypto.AESIVSize / 8).ToArray(Type.GetType("System.Byte"));
            File.Delete(Path.Combine(core.ApplicationDataFolder, "answers.key"));

            Rijndael aes = Rijndael.Create();

            String e_macpass = File.ReadAllText(filename);
            e_macpass = Crypto.StripMessage(e_macpass);

            byte[] macpass = Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));
            String abik = Convert.ToBase64String(macpass);

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("I want to remove my current public key"));

            if (server.USKeyRem_SendRemoveRequest(username, email, Convert.ToBase64String(hash)))
            {
                Console.WriteLine("Removal request successfully sent.");
            }
            else
            {
                Console.WriteLine("Removal request could not be sent, please try again.");
            }
        }

        private void Strip(String filename)
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
            while (userID == null || userID == "")
            {
                Console.Write("Sender: ");
                userID = Console.ReadLine();
                userID.Trim();
            }

            try
            {
                Connect(host);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: {0}", e);
                return null;
            }

            String publicKey = server.KeyObt(userID);

            if (publicKey == null)
            {
                Console.Error.WriteLine("Invalid recipient");
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
