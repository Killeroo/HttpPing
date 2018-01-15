# Requester

[![](https://img.shields.io/badge/version-1.0-brightgreen.svg)]() ![](https://img.shields.io/maintenance/yes/2018.svg)

Small improved command line ICMP ping program lovingly inspired by windows and linux, written in C#.

Download it here: [[Stable Release]](https://github.com/Killeroo/PowerPing/releases)
***
![alt text](docs/screenshots/screenshot.png "PowerPing in action")

## Features

- Results coloration 
- Ping like functionality
- HTTP and HTTPS support

## Usage: 
     Requester [--?] | [--li] | [--whoami] | [--loc] | [--g] | [--cg] | 
               [--fl] | [--sc] | [--t] [--c count] [--w timeout] [--dm]
               [--i TTL] [--in interval] [--pt type] [--pc code] [--b level]
	           [--4] [--short] [--nocolor] [--ts] [--ti timing] [--nt] target_name
               
## Arguments:
    Ping Options:
        --help       [--?]            Displays this help message"
        --version    [--v]            Shows version and build information
        --examples   [--ex]           Shows example usage
        --infinite   [--t]            Ping the target until stopped (Ctrl-C to stop)
        --displaymsg [--dm]           Display ICMP messages
        --ipv4       [--4]            Force using IPv4
        --random     [--rng]          Generates random ICMP message
        --beep       [--b]   number   Beep on timeout (1) or on reply (2)
        --count      [--c]   number   Number of pings to send
        --timeout    [--w]   number   Time to wait for reply (in milliseconds)
        --ttl        [--i]   number   Time To Live for packet
        --interval   [--in]  number   Interval between each ping (in milliseconds)
        --type       [--pt]  number   Use custom ICMP type
        --code       [--pc]  number   Use custom ICMP code value
        --message    [--m]   message  Ping packet message
        --timing     [--ti]  timing   Timing levels:
                                            0 - Paranoid    4 - Nimble
                                            1 - Sneaky      5 - Speedy
                                            2 - Quiet       6 - Insane
                                            3 - Polite
    
    Display Options:
        --shorthand  [--sh]           Show less detailed replies
        --timestamp  [--ts]           Display timestamp
        --nocolor    [--nc]           No colour
        --noinput    [--ni]           Require no user input
        --symbols    [--s]            Renders replies and timeouts as ASCII symbols
        --request    [--r]            Show request packets
        --notimeouts [--nt]           Don't display timeout messages
        --quiet      [--q]            No output, only shows summary upon completion or exit
        --limit      [--l]   number   Limits output to just replies (0) or requests (1)
        --decimals   [--dp]  number   Num of decimal places to use (0 to 3)

    Features:
        --scan       [--sc]  address  Network scanning, specify range "127.0.0.1-55"
        --listen     [--li]  address  Listen for ICMP packets
        --flood      [--fl]  address  Send high volume of pings to address
        --graph      [--g]   address  Graph view
        --compact    [--cg]  address  Compact graph view
        --location   [--loc] address  Location info for an address
        --whoami                      Location info for current host



## License

Requester is licensed under the [MIT license](LICENSE).

*Written by Matthew Carney [matthewcarney64@gmail.com] =^-^=*
