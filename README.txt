# Client and Server with User Interface
-----------------------------------------------------

# Contents
- Client (location) with GUI
- Server (serverlocation) with GUI

-----------------------------------------------------

# Client Functionality
- Look up and update for whois, HTTP/0.9, HTTP/1.0, and HTTP/1.1
- Launch with GUI upon start with no arguments
- Set 
  - debug mode
  - timeout
  - port number
  - host name
  - protocol

-----------------------------------------------------

# Server Functionality
- Handling of look up and update requests for whois, HTTP/0.9, HTTP/1.0, and HTTP/1.1
- Multithreaded with the ability to run over 1000 client requests on their own threads simultaneously
- Asynchronous accepting of clients as well as other asynchronous operations with usage of Tasks
- Logging
- Saving to file
- Launch with GUI upon provision of -w argument
- Set 
  - debug mode
  - timeout
  - save path
  - log path
