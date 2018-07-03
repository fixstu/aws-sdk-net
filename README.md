# Modified AWS SDK for Unity .NET
This fork is a makeshift version of AWS SDK for .NET which was modified to be run on **IL2CPP** scripting backend. The original AWS SDK runs pretty well on Mono scripting backend, so you don't need this project unless you're targeting IL2CPP backend.

[The original README.md](README.aws-sdk-net.md)

I needed to use 'AWSSDK.GameLift' module and got it working. More on that below.

I was challenged by these problems when I tried to use AWS SDK with IL2CPP:

* The entirety of `System.Configuration` is not supported by Mono shipped with Unity.
  * Any part of the code referencing the namespace had to be removed. It was mostly the part where AWS.Core loads configurations from manifest files like app.config. Unity does not support such system, so removing it is justified.
  * Looking at the 'unityv3' branch, the past developers had already known this problem and done the same thing for this problem.
* AWS SDK heavily relies on reflection.
  * This idea of using one of the powerful feature of C# is good idea. If only Unity hadn't decided to make IL2CPP.
  * Minor crash bugs caused by reflection had to be fixed.

## Using GameLift
Read the documentations on how to use GameLift. To use it on your game, you need to ...

1. Create IAM account and its permission
1. Upload build
1. Create fleet
1. Set up alias
1. (optional) Set up Queue and/or FlexMatch

They're all on the docs. Take time to read them through. This document will only talk about Unity specific matters.

* https://docs.aws.amazon.com/gamelift/latest/developerguide/gamelift-intro.html
* https://docs.aws.amazon.com/sdkfornet/v3/apidocs/Index.html

### Building GameLift Module
On Windows, open `sdk/AWSSDK.Net45.sln` or `sdk/AWSSDK.Net35.sln` and build AWSSDK.GameLift module. The assemblies will be generated in `sdk/src/Services/GameLift/bin`.

### Using Module on Unity
Place the following `link.xml` file in Assets directory along with the assemblies.

```
<linker>
  <assembly fullname="mscorlib">
    <namespace fullname="System.Security.Cryptography" preserve="all"/>
  </assembly>
  <assembly fullname="System">
    <namespace fullname="System.Security.Cryptography" preserve="all"/>
    <namespace fullname="System.Net" preserve="all"/>
  </assembly>

  <assembly fullname="AWSSDK.Core" preserve="all"/>
  <assembly fullname="AWSSDK.GameLift" preserve="all"/>
</linker>
```

Before using GameLift module, program the game to run following code. Amazon servers use RSA256 TLS certificates and Unity Mono backend fails to parse them.

```
System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => {
  return true;
};
```

Unity devs really need to update their Mono. It's important to stress that this is only a bodge. Server certificate validation is to prevent man-in-the-middle attack. This fix is a security hole and must be removed immediately once a higher version of Mono is ported to Unity.

* https://answers.unity.com/questions/163526/integrate-aws-net-sdk.html
* https://stackoverflow.com/questions/43543273/invalid-certificate-received-from-server-error-code-0xffffffff800b010a-mono

Since manifest files cannot be used on Unity, you'll need to provide AWS credentials programmatically. There are interfaces for doing it: when instantiating `AmazonGameLiftClient`, the region and the credentials for the instance can be specified.

Following methods have been tested.

* `AmazonGameLiftClient (AWSCredentials credentials, AmazonGameLiftConfig clientConfig)`
* `AmazonGameLiftClient.CreatePlayerSessionAsync (string gameSessionId, string playerId, CancellationToken cancellationToken)`
* `AmazonGameLiftClient.SearchGameSessionsAsync (SearchGameSessionsRequest request, CancellationToken cancellationToken)`
* `AmazonGameLiftClient.DescribeGameSessionDetailsAsync (DescribeGameSessionDetailsRequest request, CancellationToken cancellationToken)`
