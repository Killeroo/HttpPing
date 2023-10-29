using System;
using System.Reflection;
using System.Net;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;
using HttpPing.Interfaces;
using HttpPing;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace web_ping
{
    class Program
    {
        // Properties
        private static bool Infinite { get; set; } = false;
        private static bool Detailed { get; set; } = false;
        private static bool Timestamp { get; set; } = false;
        private static bool NoColor { get; set; } = false;
        private static bool UseCommonLogFormat { get; set; } = false;
        private static bool ForceHttps { get; set; } = false;
        private static bool ShowOnlyHostName { get; set; } = false;
        private static int Interval { get; set; } = 1000;
        private static int Requests { get; set; } = 5;
        private static int Redirects { get; set; } = 4;

        // Constants
        public const string UsageMessage = "HttpPing.exe address [-d] [-t] [-ts] [-h] [-n count] [-i interval] [-r redirectCount] [-https]";
        public const string HelpMessage = 
@"HttpPing 1.1 

Description:
    Send http requests as if they were pings!

Usage: 
    HttpPing web_address [-d] [-t] [-ts] [-h] [-https] [-n count] [-i interval] [-l] [-nc] [-r redirectCount] 

Arguments:
     [-d]                   Detailed mode: shows server and cache info
     [-t]                   Infinite mode: Keep sending requests until stopped (Ctrl-C)
     [-n count]             Send a specific number of requests
     [-ts]                  Include timestamp of when each request was sent
     [-h]                   Only show the hostname in responses
     [-i interval]          Interval between each request in milliseconds (default 30000)
     [-l]                   Use Common Log Format (https://en.wikipedia.org/wiki/Common_Log_Format)
     [-nc]                  No color
     [-r redirectCount]     Follow redirect requests a maximum number of times (default 4)
     [-https]               Force requests to use https

You can find the project and it's code on github: https://github.com/Killeroo/HttpPing
Maintained by Matthew Carney [matthewcarney64@gmail.com]
";

        private static string resolvedAddress = "";

        private static IEnvironmentService EnvironmentService;


        /// <summary>
        /// This method is the entry point of the program
        /// </summary>
        /// <param name="args">Array of strings that contains the command-line arguments passed to the program</param>
        internal static void Main(string[] args)
        {
            Run(args, new EnvironmentService());
        }

        /// <summary>
        /// This method is the entry point of the program, with the test parameter environment it wrapper exit for test purpose
        /// </summary>
        /// <param name="args">Array of strings that contains the command-line arguments passed to the program</param>
        /// <param name="environmentService">Environment (wrapper for test)</param>
        internal static void Run(string[] args, IEnvironmentService environmentService)
        {
            EnvironmentService = environmentService;

            // Some hack to get tests to work lol
            ResetArgsForTest();

            // Arguments check
            if (args.Length == 0)
            {
                Console.WriteLine(HelpMessage);
                EnvironmentService.Exit(0);
                return;
            }

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
                        case "-help":
                        case "--help":
                        case "/help":
                            Console.WriteLine(HelpMessage);
                            environmentService.Exit(0);
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
                        case "-h":
                        case "--h":
                        case "/h":
                            ShowOnlyHostName = true;
                            break;
                        case "-r":
                        case "--r":
                        case "/r":
                            Redirects = Convert.ToInt32(args[count + 1]);
                            if (Redirects <= 0)
                            {
                                Exit("Number of redirects must be higher than 0");
                            }
                            break;
                        case "-https":
                        case "--https":
                        case "/https":
                        	ForceHttps = true;
                        	break;
                        default:
                            if (count > 0)
                            {
                                if (arg.Contains("-") || arg.Contains("/"))
                                {
                                    // If argument isn't a url then throw and exception
                                    if (!Uri.IsWellFormedUriString(arg, UriKind.Absolute))
                                    {
                                        throw new ArgumentException();
                                    }
                                }

                            }
                            break;

                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Exit("Missing argument parameter");
            }
            catch (ArgumentException)
            {
                Exit("Incorrect argument found");
            }
            catch (FormatException)
            {
                Exit("Could not convert argument");
            }
            catch (OverflowException)
            {
                Exit("Could not convert argument");
            }
            catch (Exception e)
            {
                Exit(e.GetType().ToString());
            }

            if (EnvironmentService.HasExited) return;

            // Find address
            string query = "";
            if (Uri.IsWellFormedUriString(args.First(), UriKind.RelativeOrAbsolute))
                query = args.First();
            else if (Uri.IsWellFormedUriString(args.Last(), UriKind.RelativeOrAbsolute))
                query = args.Last();
            else
            {
                Exit("Could not find URL/Web address");
                if (EnvironmentService.HasExited) return;
            }

			// Modify any exting scheme if we are forcing https
			if (query.Contains("http") && !query.Contains("https") && ForceHttps)
			{
				query = query.Insert(4, "s");
			}
            
            // Add http scheme if not present
            if (!query.Contains("http")) 
            {
            	if (ForceHttps) 
            	{
          			query = query.Insert(0, "https://");
            	} 
            	else 
            	{
          			query = query.Insert(0, "http://");
            	}
            }

            // Extract host and perform DNS lookup
            Uri queryUri = new Uri(query);
            resolvedAddress = LookupAddress(queryUri.Host);

            // Send requests
            HttpRequestLoop(query);

            // Results?
        }

        public static void ResetArgsForTest()
        {
            Infinite = false;
            Detailed = false;
            Timestamp = false;
            NoColor = false;
            UseCommonLogFormat = false;
            ForceHttps = false;
            ShowOnlyHostName = false;
            Interval = 1000;
            Requests = 5;
            Redirects = 4;
        }

        // SO: https://stackoverflow.com/questions/27108264/c-sharp-how-to-properly-make-a-http-web-get-request
        private static void HttpRequestLoop(string query)
        {
            // Setup ServicePointManager to handle http verfication
            // Source: https://stackoverflow.com/a/2904963
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ServerCertificateValidationCallback);

            // Construct request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
            request.MaximumAutomaticRedirections = Redirects;
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Credentials = CredentialCache.DefaultCredentials;

            // Send requests
            int sentRequests = 0;
            Console.WriteLine("Sending HTTP{1} requests to [{0}]:", query, query.Contains("https://") ? "S" : "");

            while(true)
            {
                var response = HttpRequest(request);

                if (response != null)
                    DisplayResponse(response, response.ResponseUri);

                sentRequests++;

                if (!Infinite && sentRequests == Requests)
                {
                    // Finish loop if we have sent all the requests and not infinitely sending
                    break;
                }

                // Wait interval before sending next request
                Thread.Sleep(Interval);
            }
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Verify all requests to verify cerificates on our end
            return true;
        }

        private static HttpWebResponse HttpRequest(HttpWebRequest req) 
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                return response;
            }
            catch (WebException e)
            {
                Error(e.Message + (Timestamp ? " @ " + DateTime.Now.ToString("HH:mm:ss") : ""));
            }
            catch (Exception e)
            {
                Error(e.GetType() + ":" + e.Message + (Timestamp ? " @ " + DateTime.Now.ToString("HH:mm:ss") : ""));
            }

            return null;
        }

        private static void DisplayResponse(HttpWebResponse response, Uri responseAddress)
        {
            // !!HACK ALERT!!

            if (!UseCommonLogFormat)
            {
                Console.Write("Response from {0}: Code=", ShowOnlyHostName ? responseAddress.Host.ToString() : responseAddress.ToString());
            }

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
                Console.WriteLine("{6} {0} [{1:dd/MMM/yyyy HH:mm:ss zzz}] \"GET {2} {3}/1.0\" {4} {5}",
                                  responseAddress.Host,
                                  DateTime.Now,
                                  responseAddress.AbsolutePath,
                                  responseAddress.Scheme.ToString().ToUpper(),
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

        private static string LookupAddress(string address)
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

        private static void Error(string msg)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(msg);
            ResetConsoleColors();
        }

        private static void ResetConsoleColors()
        {
            Console.ResetColor();
        }

        private static void Exit(string message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Error(message);

            Console.WriteLine(UsageMessage);
            EnvironmentService.Exit(1);
        }
    }
}
