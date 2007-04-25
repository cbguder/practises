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
        private const String host = "practises2.no-ip.org";
        private IServer server;
        private Core core;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            Client client = new Client();
            String file = args[args.Length - 1];
            String passphrase = null;
            String recipient = null;
            String command = null;
            
            switch (args[0])
            {
                case "--encrypt":
                case "-e":
                    command = "encrypt";
                    break;
                case "--decrypt":
                case "-d":
                    command = "decrypt";
                    break;
                case "--sign":
                case "-s":
                    command = "sign";
                    break;
                case "--verify":
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
                case "--strip":
                    command = "strip";
                    break;
                default:
                    command = "help";
                    break;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-p":
                        passphrase = args[i + 1];
                        break;
                    case "-r":
                        recipient = args[i + 1];
                        break;
                }
            }
          
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
                    client.Verify(file);
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
                case "help":
                    Usage();
                    break;
            }

            return;
        }

        public Client()
        {
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
            bool write = true;

            if (File.Exists(path))
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

            while (recipient == null || recipient == "")
            {
                Console.Write("Recipient: ");
                recipient = Console.ReadLine();
                recipient.Trim();
            }

            try
            {
                Connect(host);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error: {0}", e.Message);
                return;
            }

            String publicKey = server.KeyObt(recipient);

            if (publicKey == "No records exist")
            {
                Console.Error.WriteLine("Invalid recipient");
                return;
            }

            byte[] clearText = File.ReadAllBytes(filename);
            String cipherText = Crypto.Encrypt(clearText, publicKey);

            if (Write(outFile, cipherText))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        private void Decrypt(String filename, String passphrase)
        {
            core = new Core(passphrase);

            String cipherText = File.ReadAllText(filename);
            byte[] clearText = Crypto.Decrypt(cipherText, core.PrivateKey);

            String outFile = filename + ".pses";

            if (Write(outFile, clearText))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        private void Sign(String filename, String passphrase)
        {
            core = new Core(passphrase);

            String outFile = filename + ".pses";

            String clearText = File.ReadAllText(filename, Encoding.UTF8);
            String signed = Crypto.Sign(clearText, core.PrivateKey);

            if (Write(outFile, signed))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        private void Verify(String filename)
        {
        }

        private void Initialize(String passphrase)
        {
            core = new Core(passphrase);

            Console.Write("Username: ");
            String username = Console.ReadLine();
            Console.Write("Email: ");
            String email = Console.ReadLine();

            StreamWriter sw = new StreamWriter(Path.Combine(core.ApplicationDataFolder, "identity"));
            sw.WriteLine(username);
            sw.WriteLine(email);
            sw.Close();
            
            Connect(host);

            String questions = server.InitKeySet_AskQuestions(username, email);
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

            server.InitKeySet_EnvelopeAnswers(username, email, encrypted);

            Console.Error.WriteLine("Answers sent. Please check your email to finalize PractiSES initialization.");
        }

        private void FinalizeInitialize(String filename, String passphrase)
        {
            core = new Core(passphrase);

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
            core = new Core(passphrase, false);
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
            core = new Core(passphrase);

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
            core = new Core(passphrase);

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
            core = new Core(passphrase);

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
            Console.Error.WriteLine("    -d, --decrypt");
            Console.Error.WriteLine("    -e, --encrypt");
            Console.Error.WriteLine("    -s, --sign");
            Console.Error.WriteLine("    -v, --verify");
            Console.Error.WriteLine("        --initialize");
            Console.Error.WriteLine("        --remove");
            Console.Error.WriteLine("        --update");
        }
    }
}
