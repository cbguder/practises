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

            String command = args[0];
            String file = args[args.Length - 1];
            String passphrase = null;
            String recipient = null;

            if (args.Length > 1)
            {
                if (args[1] == "-p")
                {
                    passphrase = args[2];
                }
                else
                {
                    Console.Write("Enter passphrase: ");
                    passphrase = Console.ReadLine();
                    passphrase.Trim();
                }

                if (args[1] == "-r")
                {
                    recipient = args[2];
                }
            }

            Client client = new Client(passphrase);

            switch (command)
            {
                case "--initialize":
                case "-i":
                    client.Initialize();
                    break;
                
                case "--confirm":
                case "-c":
                    client.Confirm();
                    break;

                case "--encrypt":
                case "-e":
                    client.Encrypt(file, recipient);
                    break;

                case "--decrypt":
                case "-d":
                    client.Decrypt(file);
                    break;

                case "--sign":
                case "-s":
                    client.Sign(file);
                    break;

                case "--verify":
                case "-v":
                    client.Verify(file);
                    break;

                default:
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
            core = null;
        }

        public Client(String passphrase)
        {
            core = new Core(passphrase);
        }

        private bool Connect(String host)
        {
            HttpClientChannel chan = new HttpClientChannel();
            ChannelServices.RegisterChannel(chan, false);

            Console.Error.WriteLine("Connecting to PractiSES server ({0})...", host);

            server = (ServerObject)Activator.GetObject(typeof(ServerObject), "http://" + host + "/PractiSES");

            return true;
        }

        private void Initialize()
        {
            Connect(host);
            String questions = server.InitKeySet_AskQuestions("cbguder", "cbguder@su.sabanciuniv.edu");
            Console.WriteLine(questions);
            Console.Write("Answers: ");
            String answers = Console.ReadLine();
            String serverPublicKey = server.KeyObt("server");
            byte[] message = Encoding.UTF8.GetBytes(answers);
            String encrypted = Crypto.Encrypt(message, serverPublicKey);
            server.InitKeySet_EnvelopeAnswers("cbguder", "cbguder@su.sabanciuniv.edu", encrypted);
        }

        private void Confirm()
        {
            Connect(host);

            String macpass = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "asd.txt"));
            
            HMAC hmac = HMACSHA1.Create();
            hmac.Key = Convert.FromBase64String(macpass);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(core.PublicKey));

            if (server.InitKeySet_SendPublicKey("cbguder", "cbguder@su.sabanciuniv.edu", core.PublicKey, Convert.ToBase64String(hash)))
            {
                Console.WriteLine("YÝHUUUU!!!");
            }
            else
            {
                Console.WriteLine("ühühühü");
            }
        }

        private void Encrypt(String filename, String recipient)
        {
            String outFile = filename + ".pses";

            String publicKey;

            if (recipient == null)
            {
                Console.Write("Recipient (leave empty for self): ");
                recipient = Console.ReadLine();
                recipient.Trim();
            }

            if (recipient == "" || recipient == "self")
            {
                Console.Error.WriteLine("Encrypting for self...");
                publicKey = core.PublicKey;
            }
            else
            {
                try
                {
                    Connect(host);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: {0}", e.Message);
                    return;
                }

                publicKey = server.KeyObt(recipient);
            }

            byte[] clearText = File.ReadAllBytes(filename);
            String cipherText = Crypto.Encrypt(clearText, publicKey);

            if (Write(outFile, cipherText))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        public void Decrypt(String filename)
        {
            String cipherText = File.ReadAllText(filename);
            byte[] clearText = Crypto.Decrypt(cipherText, core.PrivateKey);

            String outFile = filename + ".pses";

            if (Write(outFile, clearText))
            {
                Console.Error.WriteLine("Output written to {0}", outFile);
            }
        }

        public void Sign(String filename)
        {
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
