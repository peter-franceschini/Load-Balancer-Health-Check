# Azure Load Balancer Health Check
This service is intended to be installed alongside an existing web application on each load balanced server that the application runs on, such that the application's load balancer can be configured to query this service for it's health checks. This service handles the logic of deciding whether the application is healthy or not for the load balancer to determine if the server should be sent traffic.

This allows administrators to add or remove their applications from load balancers without modifying load balancer configurations, changing files in the application or logging into another portal. Simply stop the health check service in IIS (or where ever it is hosted) to remove your server frm the load balancer. Then start the health check service again to add your server back in the load balancer.

Additionally, this service can be extended to perform more complex health check logic. 

## How It Works
The health check service can determine the health of your application in a number of ways. The simplest is always to return healthy when the service is running. You can also configure it to ping your application locally and check if the response code is a 200 OK. It can also be configured to check for a specific text on the page.

See [Common Configurations](#common-configurations) for more details.

### Response codes
The health check service reports the targets health in the form of an HTTP response code when checked.

* Healthly - The service will return a `200 OK` response code when the target application is healthy
* Unhealthy - The service will return a `500 Internal Server Error` response code when the target application is unhealthy

## Setup Instructions
These steps will guide you through setting up the Health Check service on a Windows server using IIS. The health check service is written in .Net Core allowing it to be used on a wide range of other platforms.

### 1. Download the latest release package
The latest release package can be found here: **[NEED LINK]**

### 2. Install the health check service
Perform the following steps on each server.
1. Installed all [prerequisistes](#Prerequisites)
1. Create a new site in IIS
1. Extract the release package into the new IIS site directory
1. Update the application pool so the .NET CLR Version is set to "No Managed Code" 
1. [Configure](#Configuration) the service by updating the web.config file
1. Configure IIS site bindings to use a new, unused port (in this example we will use port 1002)
1. Open port 1002 in the windows firewall
1. Test by visiting localhost:1002/api/health in your web browser. If a 200 is returned
1. Repeat on each server

### 3. Configure the load balancer
Configure your load balancer to check for application health on each server using the newly created service by pointing its health checks to localhost:1002/api/health on each server.

## Prerequisites
* [.NET Core 2.2 Runtime & Hosting Bundle](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* Windows Server with IIS installed

## Configuration
All configuration is done in the appsettings.json file, in the configuration section `Settings`. This is shown below:

```json
"Settings": {
    "Url": "https://www.verndale.com",
    "ValidationText": "Verndale",
    "Retries": 3,
    "Timeout": 5000,
    "ForceOnline": false,
    "RequireValidSSL": true
  }
```

### Configuration Parameters

| Parameter         | Type    | Description                                                                                                                                        | Required |
|-------------------|---------|----------------------------------------------------------------------------------------------------------------------------------------------------|----------|
| `Url`             | string  | URL to load and check for a 200 status code. Include full protocol, ex: "https://www.verndale.com"                                                 | Yes      |
| `ValidationText`  | string  | Text to look for on the page that will be loaded (set by the URL parameter above). To not check for specific text, leave this field empty, ex: "" | Yes      |
| `Retries`         | integer | The number of retries to attempt if the health test of the URL fails. Set to 0 if no retries are desired.                                          | Yes      |
| `Timeout`         | integer | The number of milliseconds to set the timeout of the health test page load                                                                         | Yes      |
| `ForceOnline`     | boolean | When set, no health test will be done and the application will always return health                                                                | Yes      |
| `RequireValidSSL` | boolean | Set to true to require a valid SSL certificate, set to false to allow invalid SSL certificates, such as self signed certificates.                  | Yes      |


### Common Configurations
Below are a number of common configurations you may want to use.

#### Manual
In this configuration the health check service always returns that the application is healthy. The only way to remove the application from a load balancer is to manually stop the health check service from running.

In this configuration you must set `ForceOnline` to `true`. All other configurations are ignored.

#### Web ping only
In this configuration the health check service does a GET request to the `Url` and checks to see if it returns a `200 OK` status code. If it does the application is considered healthy, anything else is consdiered unhealthy.

In this configuration you must set `ForceOnline` to `false`. `Url`, `Retries`, `Timeout` and `RequireValidSSL` must all be set as desired. `ValidationText` must be left empty.

#### Web ping and contains text on page
In this configuration the health check service does a GET request to the `Url` and checks to see if it returns a `200 OK` status code. It then checks the content of the page body to see if it contains the `ValidationText`. If the response code is `200 OK` and the page body contains the `ValidationText` then the application is considered healthy, otherwise it is consdiered unhealthy.

In this configuration you must set `ForceOnline` to `false`. `Url`, `Retries`, `Timeout`, `RequireValidSSL` and `ValidationText` must all be set as desired.