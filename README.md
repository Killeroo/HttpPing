# Requester

[![](https://img.shields.io/badge/version-1.0-brightgreen.svg)]() ![](https://img.shields.io/maintenance/yes/2018.svg)

A small tool that sends HTTP requests, presented in a ping-like style with status codes and colored results.

Download it here: [[Stable Releases]](https://github.com/Killeroo/Requester/releases)
***
![alt text](docs/screenshots/screenshot.png "PowerPing in action")

## Features

- Results coloration 
- Ping like functionality
- HTTP and HTTPS support
- Common Log Format (NCSA log format)

## Usage: 
     Requester web_address [-d] [-t] [-ts] [-n count] [-i interval]
               
## Arguments:
     [-d]           Detailed mode: shows server and cache info
     [-t]           Infinite mode: Keep sending requests until stopped (Ctrl-C)
     [-n count]     Send a specific number of requests
     [-ts]          Include timestamp of when each request was sent
     [-i interval]  Interval between each request in milliseconds (default 30000)
     [-l]           Use Common Log Format (https://en.wikipedia.org/wiki/Common_Log_Format)
     [-nc]          No color
     
## License

Requester is licensed under the [MIT license](LICENSE).

*Written by Matthew Carney [matthewcarney64@gmail.com] =^-^=*
