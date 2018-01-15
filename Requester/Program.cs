using System;
using System.Net;
using System.Threading;

namespace web_ping
{
    class Program
    {
        // Properties
        public static bool Infinite = false;
        public static int Interval = 30000;
        public static int Requests = 5;

        // Console Properties
        public static ConsoleColor DefaultBackgroundColor;
        public static ConsoleColor DefaultForegroundColor;

        static void Main(string[] args)
        {
            // Arguments check
            if (args.Length == 0 || args.Length > 1)
            {
                Console.Write("USAGE: ");
                Environment.Exit(0);
            }

            // Save console colors
            DefaultBackgroundColor = Console.BackgroundColor;
            DefaultForegroundColor = Console.ForegroundColor;

            // Parse arguments
            //for (int count = 0; count <= args.Length; count++)
            //{
            //    string arg = args[count];
            //    switch (arg)
            //    {
            //        case "-t":
            //            Infinite = true;
            //            break;
            //        case "-n":
            //            break;
            //        default:
            //            if (arg.Contains("-"))
            //                throw new ArgumentException();
            //            break;
            //    }
            //}

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

            // Construct request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.MaximumAutomaticRedirections = 4;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Credentials = CredentialCache.DefaultCredentials;

            // Send requests
            int index = 0;
            Console.WriteLine("Sending HTTP requests to {0}...", query);
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
            
            Console.ReadKey();
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
            Console.WriteLine(" Size={0} Server={1} Cached={2}",
                response.ContentLength,
                response.Server,
                response.IsFromCache);
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
