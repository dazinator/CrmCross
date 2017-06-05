// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using CrmCross.Authentication;
using CrmCross.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

// Implementation notes:
// Some SDK classes have the following methods.
// 1. LoadFromXml - It takes an XElement and returns its class instance, which is
// a static method so that other classes can call it without instantiating the class.
// 2. ToXml - It returns XML data for the SOAP request. This is not a static method
// as it needs the actual instance data to XML.
// 3. Some classes have a ToValueXml method, which is a core part of ToXml. The reason 
// is that different methods may need a different tag.

// There is some duplicate code in places which may be consolidated if necessary.
// The implementation is similar to that used in Sdk.Soap.js with slight changes 
// due to the language differences.
// For more information about Sdk.Soap.js, see http://code.msdn.microsoft.com/SdkSoapjs-9b51b99a

namespace CrmCross
{
    public class OrganizationDataWebServiceProxy : BaseOrganizationDataWebServiceProxy
    {
        #region class members

        private IAuthenticationTokenProvider _authenticationTokenProvider = null;
        private IHttpClientFactory _httpClientFactory = null;
        private CrmServerDetails _crmServerDetails = null;
             

        #endregion class members

        #region constructor

        public OrganizationDataWebServiceProxy(CrmServerDetails crmServer, IAuthenticationTokenProvider authenticationTokenProvider)
            : this(crmServer, authenticationTokenProvider, new DefaultHttpClientFactory())
        {

        }

        public OrganizationDataWebServiceProxy(CrmServerDetails crmServerDetails, IAuthenticationTokenProvider authenticationTokenProvider, IHttpClientFactory httpClientFactory)
        {
            if (authenticationTokenProvider == null)
            {
                throw new ArgumentNullException("authenticationTokenProvider");
            }

            if (httpClientFactory == null)
            {
                throw new ArgumentNullException("httpClientFactory");
            }

            if (crmServerDetails == null)
            {
                throw new ArgumentNullException("crmServerDetails");
            }

            _httpClientFactory = httpClientFactory;
            _authenticationTokenProvider = authenticationTokenProvider;
            _crmServerDetails = crmServerDetails;
        }

        #endregion      
               

        protected override string GetOrgRestEndpointUrl()
        {
            return _crmServerDetails.OrganisationRestEndpointUrl.ToString();
        }

        protected override string GetOrgWebEndpointUrl()
        {
            return _crmServerDetails.OrganisationWebEndpointUrl.ToString();
        }

        protected override async Task<string> GetAccessToken()
        {
            var tokenResult = await _authenticationTokenProvider.GetAuthenticateTokenAsync();
            return tokenResult.AccessToken;
        }

        protected override HttpClient GetHttpClient()
        {
            return _httpClientFactory.GetHttpClient();
        }
    }

   
}