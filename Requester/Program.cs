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
        public static int Interval = 30000;
        public static int Requests = 5;

        // Console Properties
        public static ConsoleColor DefaultBackgroundColor;
        public static ConsoleColor DefaultForegroundColor;

        // Constants
        public const string USAGE = "USAGE: Requester web_address [-d] [-t] [-ts] [-n count] [-i interval]";

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
            Console.Write("Reponse from {0}: Code=", address);

            // Apply colour rules to http status code
            Console.ForegroundColor = ConsoleColor.Black;
            int statusCode = (int)response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
                Console.BackgroundColor = ConsoleColor.Green;
            else if (statusCode >= 300 && statusCode <= 500)
                Console.BackgroundColor = ConsoleColor.Yellow;
            else
                Console.BackgroundColor = ConsoleColor.Red;

            // Display response content
            Console.Write("{0}:{1}",
                (int)response.StatusCode,
                response.StatusCode);
            ResetConsoleColors();
            Console.Write(" Size={0}", response.ContentLength);
            
            if (Detailed)
                Console.Write(" Server={0} Cached={1}", response.Server, response.IsFromCache);
        
            if (Timestamp)
                Console.Write(" @ {0}", DateTime.Now.ToString("HH:mm:ss"));

            Console.WriteLine();
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
