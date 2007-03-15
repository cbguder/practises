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

namespace PractiSES
{
    class Client
    {
        private const String host = "10.90.36.198";
        private ServerObject server;
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
                case "--initialize":
                case "-i":
                    command = "initialize";
                    break;
                case "--confirm":
                case "-c":
                    command = "confirm";
                    break;
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
                case "initialize":
                    client.Initialize(passphrase);
                    break;
                case "confirm":
                    client.Confirm(file, passphrase);
                    break;
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
                case "help":
                    Usage();
                    break;
            }

            return;
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
        }

        public Client()
        {
        }

        private bool Connect(String host)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.Error.WriteLine("Connecting to PractiSES server ({0})...", host);

            server = (ServerObject)Activator.GetObject(typeof(ServerObject), "http://" + host + "/PractiSES");

            return true;
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
            Console.WriteLine(questions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            String serverPublicKey = server.KeyObt("server");
            byte[] message = Encoding.UTF8.GetBytes(answers);
            String encrypted = Crypto.Encrypt(message, serverPublicKey);
            File.WriteAllText(Path.Combine(core.ApplicationDataFolder, "answers"), encrypted);
            server.InitKeySet_EnvelopeAnswers(username, email, encrypted);
        }

        private void Confirm(String file, String passphrase)
        {
            core = new Core(passphrase);

            StreamReader sr = new StreamReader(Path.Combine(core.ApplicationDataFolder, "identity"));
            String username = sr.ReadLine();
            String email = sr.ReadLine();
            sr.Close();

            username.Trim();
            email.Trim();

            Connect(host);

            String encrypted = File.ReadAllText(Path.Combine(core.ApplicationDataFolder, "answers"));
            AESInfo info = Crypto.Destruct(encrypted, core.PrivateKey);

            Rijndael aes = Rijndael.Create();
            
            String e_macpass = File.ReadAllText(file);
            byte[] macpass = Crypto.AESDecrypt(Convert.FromBase64String(e_macpass), aes.CreateDecryptor(info.key, info.IV));
            String abik = Convert.ToBase64String(macpass);

            HMAC hmac = HMACSHA1.Create();
            hmac.Key = macpass;
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(core.PublicKey));

            if (server.InitKeySet_SendPublicKey(username, email, core.PublicKey, Convert.ToBase64String(hash)))
            {
                Console.WriteLine("Public key sent. Please check your email.");
            }
            else
            {
                Console.WriteLine("Public key could not be sent, please try again.");
            }
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

        public void Decrypt(String filename, String passphrase)
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

        public void Sign(String filename, String passphrase)
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

        public void Verify(String filename)
        {
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
    }
}
