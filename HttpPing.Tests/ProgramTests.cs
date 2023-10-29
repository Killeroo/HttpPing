using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using web_ping;

namespace HttpPing.Tests
{
    [TestClass]
    public class ProgramTests
    {
        private EnvironmentServiceForTestPurpose _environmentServiceForTestPurpose { get; set; }
        private StringBuilder _consoleOutput { get; set; }

        [TestInitialize]
        public void Init()
        {
            _environmentServiceForTestPurpose = new EnvironmentServiceForTestPurpose();

            _consoleOutput = new StringBuilder();
            Console.SetOut(new StringWriter(_consoleOutput));   
            _consoleOutput.Clear();   
        }

        [TestMethod]
        public void When_params_are_empty_should_return_help_test()
        {
            string expectedValue = Program.HelpMessage.Replace("\r\n", "");

            string[] parameters = new string[]{};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.ToString().Equals(expectedValue));
        }

        [TestMethod]
        public void When_show_help_argument_show_help_message_test()
        {
            string expectedValue = Program.HelpMessage.Replace("\r\n", "");

            string[] parameters = new string[] { "-help" };

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.ToString().Equals(expectedValue));
        }

        [TestMethod]
        public void When_full_uri_path_should_be_displayed_test()
        {
            string expectedOutput = 
@"Sending HTTPS requests to [https://github.com/Killeroo]:
Response from https://github.com/Killeroo: Code=200:OK Size=-1
";

            string[] parameters = new string[] { "https://github.com/Killeroo",
                                                "-n", "1"};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);

            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedOutput));
        }

        [TestMethod]
        public void When_hosts_only_argument_uri_path_should_not_be_displayed_test()
        {
            string expectedOutput =
@"Sending HTTPS requests to [https://github.com/Killeroo]:
Response from github.com: Code=200:OK Size=-1
";

            string[] parameters = new string[] { "https://github.com/Killeroo",
                                                "-n", "1",
                                                "-h"};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);

            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedOutput));
        }

        [TestMethod]
        public void When_bad_argument_throw_exception_test()
        {
            string expectedOutput =
@"Incorrect argument found
" + Program.UsageMessage + @"
";

            string[] parameters = new string[] { "https://github.com/Killeroo",
                                                "-n", "1",
                                                "-notrealargument"};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);

            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedOutput));
        }

        [TestMethod]
        public void When_request_count_is_one_only_one_request_is_sent_test()
        {
            string expectedOutput =
@"Sending HTTPS requests to [https://github.com/Killeroo]:
Response from https://github.com/Killeroo: Code=200:OK Size=-1
".Replace("\r\n", "");

            string[] parameters = new string[] { "https://github.com/Killeroo",
                                                "-n", "1"};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.Equals(expectedOutput));
        }

        [TestMethod]
        public void When_request_count_is_two_then_two_requests_are_sent_test()
        {
            string expectedOutput =
@"Sending HTTPS requests to [https://github.com/]:
Response from https://github.com/: Code=200:OK Size=-1
Response from https://github.com/: Code=200:OK Size=-1
".Replace("\r\n", "");

            string[] parameters = new string[] { "https://github.com/",
                                                "-n", "2"};

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.Equals(expectedOutput));
        }

        [TestMethod]
        public void When_no_params_url_is_google_http_should_respond_test()
        {
            string expectedOutput =
@"Sending HTTP requests to [http://www.google.com]:
Response from http://www.google.com/: Code=200:OK Size=-1
Response from http://www.google.com/: Code=200:OK Size=-1
Response from http://www.google.com/: Code=200:OK Size=-1
Response from http://www.google.com/: Code=200:OK Size=-1
Response from http://www.google.com/: Code=200:OK Size=-1
".Replace("\r\n", "");

            string[] parameters = new string[] { "http://www.google.com" };

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.Equals(expectedOutput));
        }

        [TestMethod]
        public void When_no_params_url_is_google_https_should_respond_test()
        {
            string expectedOutput =
@"Sending HTTPS requests to [https://www.google.com]:
Response from https://www.google.com/: Code=200:OK Size=-1
Response from https://www.google.com/: Code=200:OK Size=-1
Response from https://www.google.com/: Code=200:OK Size=-1
Response from https://www.google.com/: Code=200:OK Size=-1
Response from https://www.google.com/: Code=200:OK Size=-1
".Replace("\r\n", "");

            string[] parameters = new string[] { "https://www.google.com" };

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.Equals(expectedOutput));
        }

        [TestMethod]
        public void When_params_webaddress_contains_dash_should_not_return_error_test()
        {
            string expectedOutput =
@"Sending HTTPS requests to [https://www.star-wars.com]:
Response from https://www.starwars.com/: Code=200:OK Size=-1
".Replace("\r\n", "");

            string[] parameters = new string[] { "https://www.star-wars.com",
                                                "-n", "1",
                                                "-i", "500" };

            web_ping.Program.Run(parameters, _environmentServiceForTestPurpose);
            string actualOutput = _consoleOutput.ToString().Replace("\r\n", "");

            Assert.IsTrue(actualOutput.Equals(expectedOutput));
        }
    }
}
