using System;
using System.Reflection;
using System.Net;
using System.Threading;
using System.Linq;

namespace web_ping
{
    class Program
    {
        // Properties
        public static bool Infinite = false;
        public static bool Detailed = false;
        public static bool Timestamp = false;
        public static bool NoColor = false;
        public static bool UseCommonLogFormat = false; 
        public static int Interval = 30000;
        public static int Requests = 5;

        // Console Properties
        public static ConsoleColor DefaultBackgroundColor;
        public static ConsoleColor DefaultForegroundColor;

        // Constants
        public const string USAGE = "USAGE: Requester web_address [-d] [-t] [-ts] [-n count] [-i interval] [-l] [-nc]";

        private static string resolvedAddress = "";

        static void Main(string[] args)
        {
            // Arguments check
            if (args.Length == 0)
            {
                Console.Write(USAGE);
                Environment.Exit(0);
            }

            // Save console colors
            DefaultBackgroundColor = Console.BackgroundColor;
            DefaultForegroundColor = Console.ForegroundColor;

            // Parse arguments
            try 
            {
                for (int count = 0; count < args.Length; count++)
                {
                    string arg = args[count];
                    switch (arg)
                    {
                        case "-?":
                        case "--?":
                        case "/?":
                            Console.WriteLine(USAGE);
                            Environment.Exit(0);
                            break;
                        case "-i":
                        case "--i":
                        case "/i":
                            Interval = Convert.ToInt32(args[count + 1]);
                            break;
                        case "-t":
                        case "--t":
                        case "/t":
                            Infinite = true;
                            break;
                        case "-n":
                        case "--n":
                        case "/n":
                            Requests = Convert.ToInt32(args[count + 1]);
                            break;
                        case "-l":
                        case "--l":
                        case "/l":
                            UseCommonLogFormat = true;
                            break;
                        case "-nc":
                        case "--nc":
                        case "/nc":
                            NoColor = true;
                            break;
                        case "-d":
                        case "--d":
                        case "/d":
                            Detailed = true;
                            break;
                        case "-ts":
                        case "--ts":
                        case "/ts":
                            Timestamp = true;
                            break;
                        default:
                            if (arg.Contains("-"))
                                throw new ArgumentException();
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Error("Missing argument parameter");
                Console.WriteLine(USAGE);
                Environment.Exit(1);
            }
            catch (ArgumentException)
            {
                Error("Incorrect argument found");
                Console.WriteLine(USAGE);
                Environment.Exit(1);
            }
            catch (FormatException)
            {
                Error("Could not convert argument");
                Console.WriteLine(USAGE);
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Error(e.GetType().ToString());
                Console.WriteLine(USAGE);
                Environment.Exit(1);
            }

            // Find address
            string query = "";
            if (Uri.IsWellFormedUriString(args.First(), UriKind.RelativeOrAbsolute))
                query = args.First();
            else if (Uri.IsWellFormedUriString(args.Last(), UriKind.RelativeOrAbsolute))
                query = args.Last();
            else
            {
                Error("Could not find URL/Web address");
                Environment.Exit(1);
            }
            // Add http part if not already there
            if (!query.Contains("http"))
                query = query.Insert(0, "http://");

            // Extract host and perform DNS lookup
            Uri queryUri = new Uri(query);
            resolvedAddress = LookupAddress(queryUri.Host);

            // Send requests
            HttpRequestLoop(query);

            // Results?
        }

        // SO: https://stackoverflow.com/questions/27108264/c-sharp-how-to-properly-make-a-http-web-get-request
        static void HttpRequestLoop(string query)
        {
            // Construct request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.MaximumAutomaticRedirections = 4;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Credentials = CredentialCache.DefaultCredentials;

            // Send requests
            int index = 0;
            Console.WriteLine("Sending HTTP requests to {0}:", query);
            while (Infinite ? true : index <= Requests)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    DisplayResponse(response, query);
                }
                catch (WebException e)
                {
                    Error(e.Message + (Timestamp ? " @ " + DateTime.Now.ToString("HH:mm:ss") : ""));
                }
                catch (Exception e)
                {
                    Error(e.GetType().ToString() + ":" + e.Message + (Timestamp ? " @ " + DateTime.Now.ToString("HH:mm:ss") : ""));
                }

                if ((index+1) != Requests)
                {
                    // Wait interval and increment requests sent
                    index++;
                    Thread.Sleep(Interval);
                }
                else
                {
                    break;
                }
            }
        }

        static void DisplayResponse(HttpWebResponse response, string address)
        {
            // !!HACK ALERT!!

            if (!UseCommonLogFormat)
                Console.Write("Response from {0}: Code=", new Uri(address).Host.ToString());

            if (!NoColor)
            {
                // Apply colour rules to http status code
                Console.ForegroundColor = ConsoleColor.Black;
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 100 && statusCode <= 199)
                    // Informative
                    Console.BackgroundColor = ConsoleColor.Blue;
                else if (statusCode >= 200 && statusCode <= 299)
                    // Success
                    Console.BackgroundColor = ConsoleColor.Green;
                else if (statusCode >= 300 && statusCode <= 399)
                    // Redirection
                    Console.BackgroundColor = ConsoleColor.Cyan;
                else if (statusCode >= 400 && statusCode <= 499)
                    // Client errors
                    Console.BackgroundColor = ConsoleColor.Yellow;
                else if (statusCode >= 500 && statusCode <= 599)
                    // Server errors
                    Console.BackgroundColor = ConsoleColor.Red;
                else
                    // Other
                    Console.BackgroundColor = ConsoleColor.Magenta;
            }

            if (UseCommonLogFormat)
            {
                Uri addrUri = new Uri(address);
                Console.WriteLine("{6} {0} [{1:dd/MMM/yyyy HH:mm:ss zzz}] \"GET {2} {3}/1.0\" {4} {5}",
                                  addrUri.Host,
                                  DateTime.Now,
                                  addrUri.AbsolutePath,
                                  addrUri.Scheme.ToString().ToUpper(),
                                  ((int)response.StatusCode).ToString(),
                                  response.ContentLength,
                                  resolvedAddress);
                if (!NoColor)
                    ResetConsoleColors();

                return;
            }

            // Display response content
            Console.Write("{0}:{1}",
                (int)response.StatusCode,
                response.StatusCode);
            if (!NoColor)
                ResetConsoleColors();
            Console.Write(" Size={0}", response.ContentLength);
            
            if (Detailed)
                Console.Write(" Server={0} Cached={1}", response.Server, response.IsFromCache);
        
            if (Timestamp)
                Console.Write(" @ {0}", DateTime.Now.ToString("HH:mm:ss"));

            Console.WriteLine();
        }

        static string LookupAddress(string address)
        {
            IPAddress ipAddr = null;

            // Only resolve address if not already in IP address format
            if (IPAddress.TryParse(address, out ipAddr))
                return ipAddr.ToString();

            try
            {
                // Query DNS for host address
                foreach (IPAddress a in Dns.GetHostEntry(address).AddressList)
                {
                    // Run through addresses until we find one that matches the family we are forcing
                    if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipAddr = a;
                        break;
                    }
                }
            }
            catch (Exception) { }

            // If no address resolved then exit
            if (ipAddr == null)
                return "";

            return ipAddr.ToString();
        }

        static void Error(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(msg);
            ResetConsoleColors();
        }

        static void ResetConsoleColors()
        {
            Console.BackgroundColor = DefaultBackgroundColor;
            Console.ForegroundColor = DefaultForegroundColor;
        }
    }
}
