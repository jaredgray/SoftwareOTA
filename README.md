# SoftwareOTA
An Azure based Software/Firmware OTA (over the air) delivery service. Initially designed for the esp8266 module.

This webapi is intended to allow a device to update it's firmware OTA. It also allows you to push a new firmware version for a particular application so that when the device checks for an update again it will automatically download the update to the latest version.

### Security
There is no security currently, the api is wide open which means that if deployed publically, a malicious attacker would have the ability to upload code that would be downloaded to your devices. I'm currently running this in my local network so there isn't a lot of risk. The intention is to deploy the application publically to Azure. At that time a security layer will be added to the application.

### Limitations
As mentioned above, there is no security on the application at this time though security will be added
There is no way of assigning a particular version of firmware for a device. The application assumes that the device will always run the latest version of the software. This is something I may add in the future; not until there is a need for it.

Only one firmware file can be applied to an update and no address of where it needs to get loaded into memory, it is assumed that the software will be loaded at address 0x0.

There is no UI to the api besides a very rudimentary html file that allows you to upload a package update.

### Usage
The api is based on a sigle controller UpdateController.

#### Upload a file
There are 2 components to an update:
- a json manifest file with the name manifest.json. This has the properties of the model Manifest.cs
- the binary firmware file
These two files are then to be zipped up, the zip file name is not persisted so it doesn't need to be unique - I usually name it with the same name as the binary file. Using a date or version number along with the app name for naming the firmware file is a good strategy. ie: WifiScan.1.0.0.bin.

Using a POST with one or more zip files to the uri /api/Update will publish a file to the api.

#### Example of updating firmware on esp8266 using arduino
``` C++
void check_for_updates()
{
	Serial.println("attempting to check update from server");

	auto uri = String("/api/Update/Esp8266WifiScan/1.0.0");

	// http://192.168.1.253:43863/api/Update/Esp8266WifiScan/1.0.0
	auto ret = ESPhttpUpdate.update("192.168.1.253", 43863, uri, Version);

	if (ret == HTTP_UPDATE_FAILED)
		Failed();
	else if (ret == HTTP_UPDATE_NO_UPDATES)
		NoUpdate();
	else if (ret == HTTP_UPDATE_OK)
		UpdateSuccessful();
	else
		Serial.println("no matching response.");
	
	Serial.println("finished check update from server");
}
```
This code calls the Update controller and passes in the PackageName and the current version of the software. Each version of the firmware you will need to bump this version. You should be able to imagine what the missing methods do by the names.