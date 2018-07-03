using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Amazon.Util.Internal
{
    /// <summary>
    /// Root AWS config
    /// </summary>
    public partial class RootConfig
    {
        public LoggingConfig Logging { get; private set; }
        public ProxyConfig Proxy { get; private set; }
        public string EndpointDefinition { get; set; }
        public string Region { get; set; }
        public string ProfileName { get; set; }
        public string ProfilesLocation { get; set; }
        public RegionEndpoint RegionEndpoint
        {
            get
            {
                if (string.IsNullOrEmpty(Region))
                    return null;
                return RegionEndpoint.GetBySystemName(Region);
            }
            set
            {
                if (value == null)
                    Region = null;
                else
                    Region = value.SystemName;
            }
        }
        public bool UseSdkCache { get; set; }

        public bool CorrectForClockSkew { get; set; }

        public string ApplicationName { get; set; }

        private const string _rootAwsSectionName = "aws";
        public RootConfig()
        {
            Logging = new LoggingConfig();
            Proxy = new ProxyConfig();

            EndpointDefinition = AWSConfigs._endpointDefinition;
            Region = AWSConfigs._awsRegion;
            ProfileName = AWSConfigs._awsProfileName;
            ProfilesLocation = AWSConfigs._awsAccountsLocation;
            ApplicationName = "";
            UseSdkCache = AWSConfigs._useSdkCache;
            CorrectForClockSkew = true;
        }

        // If a is not null-or-empty, returns a; otherwise, returns b.
        private static string Choose(string a, string b)
        {
            return (string.IsNullOrEmpty(a) ? b : a);
        }

        public XElement GetServiceSection(string service)
        {
            return null;
        }
    }

}
