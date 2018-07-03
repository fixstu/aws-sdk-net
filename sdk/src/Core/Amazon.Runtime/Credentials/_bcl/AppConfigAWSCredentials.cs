/*
 * Copyright 2011-2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime.CredentialManagement.Internal;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;
using System;
using System.Collections.Specialized;
using System.Globalization;

namespace Amazon.Runtime
{
    /// <summary>
    /// Obtains credentials from access key/secret key or AWSProfileName settings
    /// in the application's app.config or web.config file.
    /// </summary>
    public class AppConfigAWSCredentials : AWSCredentials
    {
        private const string ACCESSKEY = "AWSAccessKey";
        private const string SECRETKEY = "AWSSecretKey";

        private AWSCredentials _wrappedCredentials;

        #region Public constructors 

        public AppConfigAWSCredentials()
        {
            var logger = Logger.GetLogger(typeof(AppConfigAWSCredentials));

            if (!string.IsNullOrEmpty(AWSConfigs.AWSProfileName)) {
                CredentialProfileStoreChain chain = new CredentialProfileStoreChain(AWSConfigs.AWSProfilesLocation);
                CredentialProfile profile;
                if (chain.TryGetProfile(AWSConfigs.AWSProfileName, out profile)) {
                    // Will throw a descriptive exception if profile.CanCreateAWSCredentials is false.
                    _wrappedCredentials = profile.GetAWSCredentials(profile.CredentialProfileStore, true);
                }
            }

            if (this._wrappedCredentials == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "The app.config/web.config files for the application did not contain credential information"));
            }
        }
        #endregion

        #region Abstract class overrides

        /// <summary>
        /// Returns an instance of ImmutableCredentials for this instance
        /// </summary>
        /// <returns></returns>
        public override ImmutableCredentials GetCredentials()
        {
            return this._wrappedCredentials.GetCredentials();
        }

        #endregion
    }
}
