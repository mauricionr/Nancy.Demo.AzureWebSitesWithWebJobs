# Nancy.Demo.AzureWebSitesWithWebJobs

**Nancy running as an Azure Website using WebJobs**

###Example site running on
####https://nancydemoazurewebsiteswithwebjobs.azurewebsites.net/

###In order to use this project you need to:
1. Log into your Azure account and create a new storage data service and service bus namespace
2. Edit the web- and app.config files. Add your own Azure storage account
credentials which can be found when managing keys in the newly created azure storage data service. 
The account name is the name of your storage.
Get the connection string of the service bus from the newly created.
3. Build and run the web application
4. If you want to use a CDN, go to the Azure portal and add a CDN endpoint to the storage account just created. Enter the value in the "storagecdn" application setting entry. Be aware of the 60 minute propagation period for CDN.

###Docs
- http://azure.microsoft.com/en-us/documentation/articles/websites-dotnet-webjobs-sdk-get-started/
- http://azure.microsoft.com/en-us/documentation/articles/websites-dotnet-webjobs-sdk-service-bus/
- http://azure.microsoft.com/en-us/documentation/articles/websites-dotnet-webjobs-sdk-storage-tables-how-to/#ingress
- http://azure.microsoft.com/blog/2014/10/25/announcing-the-1-0-0-rtm-of-microsoft-azure-webjobs-sdk/
- http://www.asp.net/signalr/overview/performance/scaleout-with-windows-azure-service-bus
- http://www.asp.net/signalr/overview/advanced/dependency-injection
- http://www.asp.net/signalr/overview/guide-to-the-api/hubs-api-guide-server#callfromoutsidehub

Example of connection string: 
```xml
<connectionStrings>
  <add name="storage" connectionString="DefaultEndpointsProtocol=https;AccountName=storagesample;AccountKey=KWPLd0r[...]DHptbeIHy5l/Yhg==" />
  <add name="servicebus" connectionString="Endpoint=sb://servicebusnamespace.servicebus.windows.net/;SharedAccessKeyName=username;SharedAccessKey=key" />
</connectionStrings>
<appSettings>
  <add key="storagecdn" value="mycdn.domain.net" />
</appSettings>
```
