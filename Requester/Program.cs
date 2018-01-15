using System;
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
        public const string USAGE = "USAGE: Requester.exe web_address [-d] [-t] [-ts] [-n count] [-i interval]";

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
                for (int count = 0; count <= args.Length; count++)
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
                            Interval = args[count + 1]
                        case "-t":
                        case "--t":
                        case "/t":
                            Infinite = true;
                            break;
                        case "-n":
                        case "--n":
                        case "/n":
                            count = arg[count + 1]
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
            catch (Exception e)
            {
                Error(e.GetType().GetString());
                Console.WriteLine(USAGE);
                Environment.Exit(1);
            }
            
            // Find address
            if (IsWellFormedUriString(args.first(), UriKind.RelativeOrAbsolute))
                query = args.first();
            else if (IsWellFormedUriString(args.last(), UriKind.RelativeOrAbsolute))
                query = args.last();
            else
            {
                Error("Could not find URL/Web address");
                Enviroment.Exit(1);
            }

            // validate and sanitise address (add http part)
            string query = "";
            //Uri result;
            query = args[0];
            if (!query.Contains("http"))
            {
                query = query.Insert(0, "http://");
            }
            //if (Uri.TryCreate(query, UriKind.Absolute, out result)
            //    && (result.Scheme != Uri.UriSchemeHttp || result.Scheme != Uri.UriSchemeHttps))
            //{
            //    Console.Write("Incorrect URL format");
            //    Environment.Exit(1);
            //}

            // Send requests
            HttpRequestLoop(query);

            // Results?
            
        }

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
                    Error(e.Message);
                }
                catch (Exception e)
                {
                    Error(e.GetType().ToString() + ":" + e.Message);
                }

                // Wait interval and increment requests sent
                index++;
                Thread.Sleep(Interval);
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
                Console.Write("Server={0} Cached={1}", response.Server, response.IsFromCache);
        
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
