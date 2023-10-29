using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

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
        public void When_params_are_empty_should_return_usage_test()
        {
            var expectedValue = "USAGE: HttpPing web_address [-d] [-t] [-ts] [-https] [-n count] [-i interval] [-l] [-nc] [-r redirectCount] \r\n";

            string[] parameters = new string[]{};

            web_ping.Program.Main(parameters, _environmentServiceForTestPurpose);


            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedValue));
        }


        [TestMethod]
        public void When_params_webaddress_simple_count_1_interval_500ms_should_response_test()
        {
            var expectedValue = "Sending HTTP requests to [https://www.starwars.com]:\r\nResponse from www.starwars.com: Code=200:OK Size=-1\r\nResponse from www.starwars.com: Code=200:OK Size=-1\r\n";

            string[] parameters = new string[] { "https://www.starwars.com", 
                                                "-n", "1",
                                                "-i", "500" };

            web_ping.Program.Main(parameters, _environmentServiceForTestPurpose);

            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedValue));
        }


        [TestMethod]
        public void When_params_webaddress_contains_dash_should_not_return_error_test()
        {
            //var expectedValue = "Incorrect argument found\r\nUSAGE: HttpPing web_address [-d] [-t] [-ts] [-https] [-n count] [-i interval] [-l] [-nc] [-r redirectCount] \r\n";
            var expectedValue = "Sending HTTP requests to [https://www.star-wars.com]:\r\nResponse from www.star-wars.com: Code=200:OK Size=-1\r\nResponse from www.star-wars.com: Code=200:OK Size=-1\r\n";


            string[] parameters = new string[] { "https://www.star-wars.com",
                                                "-n", "1",
                                                "-i", "500" };

            web_ping.Program.Main(parameters, _environmentServiceForTestPurpose);

            Assert.IsTrue(_consoleOutput.ToString().Equals(expectedValue));
        }
    }
}
