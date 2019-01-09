# HttpPing

[![](https://img.shields.io/badge/version-1.0-brightgreen.svg)]() [![Build status](https://ci.appveyor.com/api/projects/status/q7lchn78v07pmemj?svg=true)](https://ci.appveyor.com/project/Killeroo/httpping)

A small tool that sends HTTP requests, presented in a ping-like style with status codes and colored results.

Download it here: [[Stable Releases]](https://github.com/Killeroo/HttpPing/releases) [[Nightly Build]](https://ci.appveyor.com/api/projects/killeroo/httpping/artifacts/HttpPing%2Fbin%2FDebug%2FHttpPing.exe)
***
![alt text](HttpPing/Screenshots/screenshot1.png "HttpPing in action")

## Features

- Results coloration 
- Ping like functionality
- HTTP and HTTPS support
- Common Log Format (NCSA log format)

## Usage: 
     HttpPing.exe address [-d] [-t] [-ts] [-n count] [-i interval] [-r redirectCount] [-https]
               
## Arguments:
     [-d]                   Detailed mode: shows server and cache info
     [-t]                   Infinite mode: Keep sending requests until stopped (Ctrl-C)
     [-n count]             Send a specific number of requests
     [-ts]                  Include timestamp of when each request was sent
     [-i interval]          Interval between each request in milliseconds (default 30000)
     [-l]                   Use Common Log Format (https://en.wikipedia.org/wiki/Common_Log_Format)
     [-nc]                  No color
     [-r redirectCount]     Follow redirect requests a maximum number of times (default 4)
     [-https]               Force requests to use https
     
## Example 
     HttpPing.exe google.com -t -l
     
## More Screenshots
![alt text](HttpPing/Screenshots/screenshot2.png "Supports Common Log Format")
![alt text](HttpPing/Screenshots/screenshot3.png "and with no color too!")

## License

Requester is licensed under the [MIT license](LICENSE).

*Written by Matthew Carney [matthewcarney64@gmail.com] =^-^=*
