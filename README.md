# OvhNet
C# wrapper for Ovh REST API and wrapper for OpenStack object storage API

# Example of use

# Object Storage API based on OpenStack

First of all you need to create user to your cloud account on OVH. You can do it here
![addUser](https://goo.gl/vfTygE)
After this go to: https://horizon.cloud.ovh.net and log in.
Now go to API Access section and click "View Credentials"
Project ID is your tenant id.
```
string containerName = "test";
string containerName2 = "test2";
string fileName = "avatar.jpg";
string pathToFile = Path.Combine(Environment.CurrentDirectory, fileName);

string userName = ""; // OpenStack username 
string password = "";
string tenantId = "";
var ovh = new OvhStorage(userName, password, tenantId, "GRA1");

//create test container
Assert.IsTrue(ovh.Container.Create(containerName));
Assert.IsTrue(ovh.Container.Create(containerName2));

//put file to testContainer
Assert.IsTrue(ovh.ContainerObject.Set(containerName, fileName, pathToFile));
var items = ovh.Container.List(containerName);
Assert.IsTrue(items.Any());
Assert.IsTrue(items.Any(z => z.Name.Equals(fileName)));

//get info about file
ovh.ContainerObject.Info(containerName, fileName);

//copy file to pptest2 container
Assert.IsTrue(ovh.ContainerObject.Copy(containerName, fileName, containerName2));

//get file from container
var bytes = ovh.ContainerObject.Get(containerName, fileName);
Assert.IsTrue(bytes.Any());

//delete files from test containers
Assert.IsTrue(ovh.ContainerObject.Delete(containerName, fileName));
Assert.IsTrue(ovh.ContainerObject.Delete(containerName2, fileName));

//delete container
Assert.IsTrue(ovh.Container.Delete(containerName));
//delete container
Assert.IsTrue(ovh.Container.Delete(containerName2));


//create test container
Assert.IsTrue(ovh.Container.Create(containerName));
Assert.IsTrue(ovh.Container.Create(containerName2));
Assert.IsTrue(ovh.ContainerObject.Set(containerName, fileName, pathToFile));
Assert.IsTrue(ovh.ContainerObject.Copy(containerName, fileName, containerName2));
//delete objects with container
Assert.IsTrue(ovh.Container.DeleteForce(containerName));
Assert.IsTrue(ovh.Container.DeleteForce(containerName2));
```

# OVH API

Here you must create your own Token. You can do it here:
https://api.ovh.com/createToken/

Your rights can be for example: GET /*, POST /*
Generated token can only use GET and POST methods.

```
string ApplicationKey = "";
string ApplicationSecret = "";
string ConsumerKey = "";
string ServiceName = ""; // Project ID visible in Cloud tab in ovh site

var ovh = new OvhWrapper("ovh-eu", ApplicationKey, ApplicationSecret, ConsumerKey, ServiceName);
var meUser = ovh.Get<Nichandle>("/me");
var storages = ovh.Get<List<Container>>("/cloud/project/{serviceName}/storage");
```
