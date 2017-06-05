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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
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
    public abstract class BaseOrganizationDataWebServiceProxy : IOrganizationService
    {
        #region class members
        
        // public string ServiceUrl;
        public Guid CallerId;
        public int TimeoutInSeconds;

        static private IEnumerable<TypeInfo> types = null;

        #endregion class members
       

        #region IOrganizationService Soap Methods

        // Provide same methods as IOrganizationService with same parameter and types
        // so that developer can use this class without confusion.

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            AsyncHelper.RunSync(() => AssociateAsync(entityName, entityId, relationship, relatedEntities));
        }

        public Guid Create(Entity entity)
        {
            var result = AsyncHelper.RunSync(() => CreateAsync(entity));
            return result;

            //var task = this.CreateAsync(entity);
            // task.Wait();
            // return task.Result;
        }

        public void Delete(string entityName, Guid id)
        {
            var task = this.DeleteAsync(entityName, id);
            task.Wait();
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            var task = this.DisassociateAsync(entityName, entityId, relationship, relatedEntities);
            task.Wait();
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {

            var result = AsyncHelper.RunSync(() => ExecuteAsync(request));
            return result;

            //var task = this.ExecuteAsync(request);
            //task.Wait();
            //return task.Result;
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            var task = this.RetrieveAsync(entityName, id, columnSet);
            task.Wait();
            return task.Result;
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            var task = this.RetrieveMultipleAsync(query);
            task.Wait();
            return task.Result;
        }

        public void Update(Entity entity)
        {
            var task = this.UpdateAsync(entity);
            task.Wait();
        }

        /// <summary>
        /// Creates a link between records.
        /// </summary>
        /// <param name="entityName">The logical name of the entity that is specified in the entityId parameter.</param>
        /// <param name="entityId">The ID of the record to which the related records are associated.</param>
        /// <param name="relationship">The name of the relationship to be used to create the link.</param>
        /// <param name="relatedEntities">A collection of entity references (references to records) to be associated.</param>
        public async Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Associate";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Associate>");
                content.Append("<d:entityName>" + entityName + "</d:entityName>");
                content.Append("<d:entityId>" + entityId.ToString() + "</d:entityId>");
                content.Append(Util.ObjectToXml(relationship, "d:relationship", true));
                content.Append(Util.ObjectToXml(relatedEntities, "d:relatedEntities", true));
                content.Append("</d:Associate>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    //do nothing
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }
            }
        }

        /// <summary>
        /// Creates a record.
        /// </summary>
        /// <param name="entity">An entity instance that contains the properties to set in the newly created record.</param>
        /// <returns>The ID of the newly created record.</returns>
        public async Task<Guid> CreateAsync(Entity entity)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Create";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Create>");
                content.Append(Util.ObjectToXml(entity, "d:entity", true));
                content.Append("</d:Create>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                Guid createdRecordId = Guid.Empty;
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Obtain Guid values from result.
                    XDocument xdoc = XDocument.Parse(httpResponse.Content.ReadAsStringAsync().Result, LoadOptions.None);
                    foreach (var result in xdoc.Descendants(Util.ns.d + "CreateResponse"))
                    {
                        createdRecordId = Util.LoadFromXml<Guid>(result);
                    }
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }

                return createdRecordId;
            }
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        /// <param name="entityName">The logical name of the entity that is specified in the entityId parameter.</param>
        /// <param name="id">The ID of the record that you want to delete.</param>
        public async Task DeleteAsync(string entityName, Guid id)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Delete";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Delete>");
                content.Append("<d:entityName>" + entityName + "</d:entityName>");
                content.Append("<d:id>" + id.ToString() + "</d:id>");
                content.Append("</d:Delete>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    // Do nothing as it returns void.
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }
            }
        }

        /// <summary>
        /// Deletes a link between records.
        /// </summary>
        /// <param name="entityName">The logical name of the entity that is specified in the entityId parameter.</param>
        /// <param name="entityId">The ID of the record from which the related records are disassociated.</param>
        /// <param name="relationship">The name of the relationship to be used to remove the link.</param>
        /// <param name="relatedEntities">A collection of entity references (references to records) to be disassociated.</param>
        public async Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship,
            EntityReferenceCollection relatedEntities)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Disassociate";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Disassociate>");
                content.Append("<d:entityName>" + entityName + "</d:entityName>");
                content.Append("<d:entityId>" + entityId.ToString() + "</d:entityId>");
                content.Append(Util.ObjectToXml(relationship, "d:relationship", true));
                content.Append(Util.ObjectToXml(relatedEntities, "d:relatedEntities", true));
                content.Append("</d:Disassociate>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    //do nothing
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }


            }
        }

        /// <summary>
        /// Executes a message in the form of a request, and returns a response.
        /// </summary>
        /// <param name="request">A request instance that defines the action to be performed.</param>
        /// <returns>The response from the request. You must cast the return value of this method to 
        /// the specific instance of the response that corresponds to the Request parameter.</returns>
        public async Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Execute>");
                content.Append(request.GetRequestBody());
                content.Append("</d:Execute>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    // 1. Request contains instance of its corresponding Response.
                    // 2. Response has override StoreResult method to extract data
                    // from result if necessary.
                    request.ResponseType.StoreResult(httpResponse);
                    return request.ResponseType;
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }
            }
        }

        /// <summary>
        /// Retrieves a record.
        /// </summary>
        /// <param name="entityName">The logical name of the entity that is specified in the entityId parameter.</param>
        /// <param name="id">The ID of the record that you want to retrieve.</param>
        /// <param name="columnSet">A query that specifies the set of columns, or attributes, to retrieve.</param>
        /// <returns>The requested entity. If EnableProxyTypes called, returns early bound type.</returns>
        public async Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Retrieve";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Retrieve>");
                content.Append("<d:entityName>" + entityName + "</d:entityName>");
                content.Append("<d:id>" + id.ToString() + "</d:id>");
                content.Append(Util.ObjectToXml(columnSet, "d:columnSet", true));
                content.Append("</d:Retrieve>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                Entity Entity = new Entity();

                // Chech the returned code
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Extract Entity from result.
                    XDocument xdoc = XDocument.Parse(httpResponse.Content.ReadAsStringAsync().Result, LoadOptions.None);
                    foreach (var result in xdoc.Descendants(Util.ns.d + "RetrieveResult"))
                    {
                        Entity = Entity.LoadFromXml(result);
                    }
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }

                // If Entity if not casted yet, then try to cast to early-bound
                if (Entity.GetType().Equals(typeof(Entity)))
                    Entity = ConvertToEarlyBound(Entity);

                return Entity;
            }
        }

        /// <summary>
        /// Retrieves a collection of records.
        /// </summary>
        /// <param name="query">A query that determines the set of records to retrieve.</param>
        /// <returns>The collection of entities returned from the query. If EnableProxyTypes called, returns early bound type.</returns>
        public async Task<EntityCollection> RetrieveMultipleAsync(QueryBase query)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/RetrieveMultiple";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:RetrieveMultiple>");
                content.Append(Util.ObjectToXml(query, "d:query"));
                content.Append("</d:RetrieveMultiple>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                EntityCollection entityCollection = null;

                // Check the returned code
                if (httpResponse.IsSuccessStatusCode)
                {
                    // Extract EntityCollection from result.
                    XDocument xdoc = XDocument.Parse(httpResponse.Content.ReadAsStringAsync().Result, LoadOptions.None);
                    foreach (var results in xdoc.Descendants(Util.ns.d + "RetrieveMultipleResult"))
                    {
                        entityCollection = EntityCollection.LoadFromXml(results);
                    }
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }

                return entityCollection;
            }
        }

        /// <summary>
        /// Updates an existing record.
        /// </summary>
        /// <param name="entity">An entity instance that has one or more properties set to be updated in the record.</param>
        public async Task UpdateAsync(Entity entity)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string SOAPAction = "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Update";

                StringBuilder content = new StringBuilder();
                content.Append(GetEnvelopeHeader());
                content.Append("<s:Body>");
                content.Append("<d:Update>");
                content.Append(Util.ObjectToXml(entity, "d:entity", true));
                content.Append("</d:Update>");
                content.Append("</s:Body>");
                content.Append("</s:Envelope>");

                // Send the request asychronously and wait for the response.
                HttpResponseMessage httpResponse;
                httpResponse = await SendRequest(httpClient, SOAPAction, content.ToString());

                if (httpResponse.IsSuccessStatusCode)
                {
                    // Do nothing as it returns void.
                }
                else
                {
                    OrganizationServiceFault fault = RestoreFault(httpResponse);
                    if (!String.IsNullOrEmpty(fault.Message))
                        throw fault;
                    else
                        throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);
                }
            }
        }

        #endregion methods

        #region Rest Methods

        /// <summary>
        /// create record
        /// </summary>
        /// <param name="entity">record to create</param>
        /// <returns></returns>
        public async Task<Guid> RestCreate(Entity entity)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                DataContractJsonSerializer jasonSerializer = new DataContractJsonSerializer(entity.GetType());
                string json;
                using (MemoryStream ms = new MemoryStream())
                {
                    jasonSerializer.WriteObject(ms, entity);
                    json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                string ODataAction = entity.GetType().GetRuntimeField("SchemaName").ToString() + "Set";

                // Build and send the HTTP request.
                var accessToken = await GetAccessToken();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Use PostAsync to Post data.
                HttpResponseMessage response = await httpClient.PostAsync(GetOrgRestEndpointUrl() + ODataAction, content);

                // Check the response result.
                if (response.IsSuccessStatusCode)
                {
                    Entity result;
                    // Deserialize response to JToken 
                    byte[] resultbytes = Encoding.UTF8.GetBytes(response.Content.ReadAsStringAsync().Result);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        result = (Entity)jasonSerializer.ReadObject(ms);
                    }
                    return result.Id;
                }
                else
                    throw new Exception("REST Create failed.");
            }
        }

        /// <summary>
        /// Deletes a record.
        /// </summary>
        /// <param name="schemaName">The Schema name of the entity that is specified in the entityId parameter.</param>
        /// <param name="id">The ID of the record that you want to delete.</param>
        public async Task RestDelete(string schemaName, Guid id)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                string ODataAction = schemaName + "Set(guid'" + id + "')";

                // Build and send the HTTP request.
                var accessToken = await GetAccessToken();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Use DeleteAsync to Post data.
                HttpResponseMessage response = await httpClient.DeleteAsync(GetOrgRestEndpointUrl() + "/" + ODataAction);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("REST Delete failed.");
            }
        }

        /// <summary>
        /// Retrieve a record
        /// </summary>
        /// <param name="schemaName">Entity SchemaName.</param>
        /// <param name="id">id of record to be retireved</param>
        /// <param name="columnSet">retrieved columns</param>
        /// <returns></returns>
        public async Task<Entity> RestRetrieve(string schemaName, Guid id, ColumnSet columnSet)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                StringBuilder select = new StringBuilder();
                foreach (string column in columnSet.Columns)
                {
                    select.Append("," + column);
                }

                // The URL for the OData organization web service.
                string ODataAction = schemaName + "Set(guid'" + id + "')?$select=" + select.ToString().Remove(0, 1) + "";

                // Build and send the HTTP request.          
                var accessToken = await GetAccessToken();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Wait for the web service response.
                HttpResponseMessage response = await httpClient.GetAsync(GetOrgRestEndpointUrl() + "/" + ODataAction);

                // Check the response result.
                if (response.IsSuccessStatusCode)
                {
                    // Check type.
                    TypeInfo currentType;
                    if (types == null)
                        throw new Exception("Early-bound types must be enabled for a REST Retrieve.");
                    else
                    {
                        currentType = types.Where(x => x.Name.ToLower() == schemaName.ToLower()).FirstOrDefault();
                        if (currentType == null)
                            throw new Exception("Early-bound types must be enabled for a REST Retrieve.");
                    }
                    // Deserialize response to JToken 
                    JToken jtoken = JObject.Parse(response.Content.ReadAsStringAsync().Result)["d"];
                    return (Entity)JsonConvert.DeserializeObject(jtoken.ToString(), currentType.AsType());
                }
                else
                    throw new Exception("REST Retrieve failed.");
            }
        }

        /// <summary>
        /// Need to consider how to implement this. Let developer create URL or implement convert method form QueryBase to URL?
        /// For now, just let user pass schemaName and ColumnSet.
        /// </summary>
        /// <param name="schemaName"></param>
        /// <param name="columnSet"></param>
        /// <returns></returns>
        public async Task<EntityCollection> RestRetrieveMultiple(string schemaName, ColumnSet columnSet)
        {
            // Create HttpClient with Compression enabled.
            using (HttpClient httpClient = GetHttpClient())
            {
                StringBuilder select = new StringBuilder();
                foreach (string column in columnSet.Columns)
                {
                    select.Append("," + column);
                }

                // The URL for the OData organization web service.
                string ODataAction = schemaName + "Set?$select=" + select.ToString().Remove(0, 1) + "";

                // Build and send the HTTP request.
                var accessToken = await GetAccessToken();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Wait for the web service response.
                HttpResponseMessage response = await httpClient.GetAsync(GetOrgRestEndpointUrl() + "/" + ODataAction);

                // Check the response result.
                if (response.IsSuccessStatusCode)
                {
                    EntityCollection results = new EntityCollection();
                    results.EntityName = schemaName.ToLower();

                    // Check type.
                    TypeInfo currentType;
                    if (types == null)
                        throw new Exception("Early-bound types must be enabled for a REST RetrieveMultiple.");
                    else
                    {
                        currentType = types.Where(x => x.Name.ToLower() == schemaName.ToLower()).FirstOrDefault();
                        if (currentType == null)
                            throw new Exception("Early-bound types must be enabled for a REST RetrieveMultiple.");
                    }

                    // Deserialize response to JToken IList
                    IList<JToken> jTokens = JObject.Parse(response.Content.ReadAsStringAsync().Result)["d"]["results"].Children().ToList();
                    foreach (JToken jToken in jTokens)
                    {
                        // Deserialize result to Type T
                        results.Entities.Add((Entity)JsonConvert.DeserializeObject(jToken.ToString(), currentType.AsType()));
                    }
                    results.TotalRecordCount = jTokens.Count();
                    return results;
                }
                else
                    throw new Exception("REST RetrieveMultiple failed.");
            }
        }

        /// <summary>
        /// Update a record
        /// </summary>
        /// <param name="entity">record to update</param>
        /// <returns></returns>
        public async Task RestUpdate(Entity entity)
        {
            // Create HttpClient with Compression enabled.

            using (HttpClient httpClient = GetHttpClient())
            {
                DataContractJsonSerializer jasonSerializer = new DataContractJsonSerializer(entity.GetType());
                MemoryStream ms = new MemoryStream();
                jasonSerializer.WriteObject(ms, entity);
                string json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // The URL for the OData organization web service.
                string ODataAction = entity.GetType().GetRuntimeField("SchemaName").ToString() + "Set(guid'" + entity.Id + "')";

                // Build and send the HTTP request.
                var accessToken = await GetAccessToken();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                // Specify MERGE to update record
                httpClient.DefaultRequestHeaders.Add("X-HTTP-Method", "MERGE");

                // Use PostAsync to Post data.
                HttpResponseMessage response = await httpClient.PostAsync(GetOrgRestEndpointUrl() + "/" + ODataAction, content);

                // Check the response result.
                if (!response.IsSuccessStatusCode)
                    throw new Exception("REST Update failed.");
            }
        }

        protected abstract HttpClient GetHttpClient();

        #endregion

        #region helpercode

#if __ANDROID__
// Android-specific code
             //// To make this project Xamarin compatible, you need to comment out this method.
        //public async Task EnableProxyTypes()
        //{
        //    List<TypeInfo> typeList = new List<TypeInfo>();
        //    // Obtain folder of executing application.
        //    var folder = Package.Current.InstalledLocation;
        //    foreach (var file in await folder.GetFilesAsync())
        //    {
        //        // not only .dll but .exe also contains types.
        //        if (file.FileType == ".dll" || file.FileType == ".exe")
        //        {
        //            var assemblyName = new AssemblyName(file.DisplayName);
        //            var assembly = Assembly.Load(assemblyName);
        //            foreach (TypeInfo type in assembly.DefinedTypes)
        //            {
        //                // Store only CRM Entities.
        //                if (type.BaseType == typeof(Entity))
        //                    typeList.Add(type);
        //            }
        //        }
        //    }
        //    types = typeList.ToArray();
        //}

#endif




        /// <summary>
        /// Create HTTPRequest and returns the HTTPRequestMessage.
        /// </summary>
        /// <param name="httpClient">httpClient instance.</param>
        /// <param name="SOAPAction">Action for the endpoint, like Execute, Retrieve, etc.</param>
        /// <param name="content">Request Body</param>
        /// <returns>HTTPRequest</returns>
        private async Task<HttpResponseMessage> SendRequest(HttpClient httpClient, string SOAPAction, string content)
        {
            // Set the HTTP authorization header using the access token.
            var accessToken = await GetAccessToken();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (TimeoutInSeconds > 0)
                httpClient.Timeout = new TimeSpan(0, 0, 0, TimeoutInSeconds, 0);
            // Finish setting up the HTTP request.
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, GetOrgWebEndpointUrl());
            req.Headers.Add("SOAPAction", SOAPAction);
            req.Method = HttpMethod.Post;
            req.Content = new StringContent(content);
            req.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/xml; charset=utf-8");

            // Send the request asychronously and wait for the response.            
            return await httpClient.SendAsync(req);
        }

        /// <summary>
        /// Create Envelope for SOAP request.
        /// </summary>
        /// <returns>SOAP Envelope with namespaces.</returns>
        /// <summary>
        /// Enable Early Bound by storing all Types in the application.
        /// </summary>
        /// <returns>SOAP Envelope with namespaces.</returns>
        ///       
        private string GetEnvelopeHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/' xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts' xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns:b='http://schemas.datacontract.org/2004/07/System.Collections.Generic' xmlns:c='http://www.w3.org/2001/XMLSchema' xmlns:d='http://schemas.microsoft.com/xrm/2011/Contracts/Services' xmlns:e='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:f='http://schemas.microsoft.com/2003/10/Serialization/Arrays' xmlns:g='http://schemas.microsoft.com/crm/2011/Contracts' xmlns:h='http://schemas.microsoft.com/xrm/2011/Metadata' xmlns:j='http://schemas.microsoft.com/xrm/2011/Metadata/Query' xmlns:k='http://schemas.microsoft.com/xrm/2013/Metadata' xmlns:l='http://schemas.microsoft.com/xrm/2012/Contracts'>");
            sb.Append("<s:Header>");
            if (this.CallerId != null && this.CallerId != Guid.Empty)
                sb.Append("<a:CallerId>" + this.CallerId.ToString() + "</a:CallerId>");
            sb.Append("<a:SdkClientVersion>6.0</a:SdkClientVersion>");
            sb.Append("</s:Header>");
            return sb.ToString();
        }

        private OrganizationServiceFault RestoreFault(HttpResponseMessage httpResponse)
        {
            // Has to have length to be XML
            var responseContent = httpResponse.Content.ReadAsStringAsync().Result;
            bool parseAsXml = false;

            if (!string.IsNullOrEmpty(responseContent))
            {
                // If it starts with a < after trimming then it probably is XML
                // Need to do an empty check again in case the string is all white space.
                var trimmedData = responseContent.TrimStart();
                if (string.IsNullOrEmpty(trimmedData))
                {
                    parseAsXml = false;
                }

                if (trimmedData[0] == '<')
                {
                    parseAsXml = true;
                }
            }

            if (parseAsXml)
            {
                try
                {
                    XDocument xdoc = XDocument.Parse(responseContent, LoadOptions.None);
                    return OrganizationServiceFault.LoadFromXml(xdoc.Descendants(Util.ns.a + "OrganizationServiceFault").First());
                }
                catch (System.Xml.XmlException)
                {
                    // invalid xml.
                }
            }

            OrganizationServiceFault orgFault = new OrganizationServiceFault();
            orgFault.Message = responseContent;
            return orgFault;
        }


        static internal Entity ConvertToEarlyBound(Entity entity)
        {
            if (types == null)
                return entity;
            TypeInfo currentType = types.Where(x => x.Name.ToLower() == entity.LogicalName).FirstOrDefault();
            if (currentType == null)
                return entity;
            else
                // Then convert it by using Entity.ToEntity<T> method.
                return (Entity)typeof(Entity).GetRuntimeMethod("ToEntity", new Type[] { }).
                    MakeGenericMethod(currentType.AsType()).Invoke(entity, null);
        }

        protected abstract string GetOrgRestEndpointUrl();

        protected abstract string GetOrgWebEndpointUrl();

        protected abstract Task<string> GetAccessToken();
        //{
        //    var tokenResult = await _authenticationTokenProvider.GetAuthenticateTokenAsync();
        //    return tokenResult.AccessToken;
        //}

        #endregion helpercode

    }


}