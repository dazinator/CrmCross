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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
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

    public enum EntityRole
    {
        Referenced = 1,
        Referencing = 0
    }
    public enum EntityState
    {
        Unchanged = 0,
        Created = 1,
        Changed = 2
    }
    public enum OperationStatus
    {
        Failed,
        Canceled,
        Retry,
        Suspended,
        Succeeded
    }
    public sealed class AliasedValue
    {
        public string AttributeLogicalName { get; set; }
        public string EntityLogicalName { get; set; }
        public Object Value { get; set; }
        static public AliasedValue LoadFromXml(XElement item)
        {
            AliasedValue aliasedValue = new AliasedValue()
            {
                AttributeLogicalName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "AttributeLogicalName")),
                EntityLogicalName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "EntityLogicalName")),
                Value = Util.ObjectFromXml(item.Element(Util.ns.a + "Value"))
            };
            return aliasedValue;
        }
    }
    public sealed class AttributeCollection : DataCollection<string, object>
    {
        internal string ToXml()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Count == 0)
            {
                return sb.Append("<a:Attributes/>").ToString();
            }
            sb.Append("<a:Attributes>");
            foreach (var item in this)
            {
                sb.Append("<a:KeyValuePairOfstringanyType>");
                sb.Append("<b:key>" + item.Key + "</b:key>");
                sb.Append(Util.ObjectToXml(item.Value, "b:value"));
                sb.Append("</a:KeyValuePairOfstringanyType>");
            }
            sb.Append("</a:Attributes>");
            return sb.ToString();
        }
        internal void LoadFromXml(XElement item)
        {
            foreach (var att in item.Elements(Util.ns.a + "Attributes").Elements(Util.ns.a + "KeyValuePairOfstringanyType"))
            {
                AttributeLoadFromXml(att);
            }
        }
        internal void AttributeLoadFromXml(XElement item)
        {
            string key = Util.LoadFromXml<string>(item.Element(Util.ns.b + "key"));
            object value = Util.ObjectFromXml(item.Element(Util.ns.b + "value"));
            this.Add(key, value);
        }
    }
    public sealed class AttributeLogicalNameAttribute : Attribute
    {
        public string LogicalName { get; set; }

        public AttributeLogicalNameAttribute(string logicalName)
        {
            if (String.IsNullOrEmpty(logicalName))
                throw new ArgumentNullException("logicalName");
            LogicalName = logicalName;
        }
    }
    public sealed class AttributeMapping
    {
        public int AllowedSyncDirection { get; set; }
        public string AttributeCrmDisplayName { get; set; }
        public string AttributeCrmName { get; set; }
        public string AttributeExchangeDisplayName { get; set; }
        public string AttributeExchangeName { get; set; }
        public Guid AttributeMappingId { get; set; }
        public DataCollection<string> ComputedProperties { get; set; }
        public int DefaultSyncDirection { get; set; }
        public int EntityTypeCode { get; set; }
        public bool IsComputed { get; set; }
        public string MappingName { get; set; }
        public int SyncDirection { get; set; }
        public AttributeMapping()
        {
            ComputedProperties = new DataCollection<string>();
        }
        public AttributeMapping(Guid attributeMappingId, string mappingName, string attributeCrmName,
            string attributeExchangeName, int entityTypeCode, int syncDirection, int defaultSyncDirection,
            int allowedSyncDirection, bool isComputed, DataCollection<string> computedProperties,
            string attributeCrmDisplayName, string attributeExchangeDisplayName)
            : base()
        {
            this.AttributeMappingId = attributeMappingId;
            this.MappingName = mappingName;
            this.AttributeCrmName = attributeCrmName;
            this.AttributeExchangeDisplayName = attributeExchangeDisplayName;
            this.EntityTypeCode = entityTypeCode;
            this.SyncDirection = syncDirection;
            this.DefaultSyncDirection = defaultSyncDirection;
            this.AllowedSyncDirection = allowedSyncDirection;
            this.IsComputed = isComputed;
            this.ComputedProperties = computedProperties;
            this.AttributeCrmDisplayName = attributeCrmDisplayName;
            this.AttributeExchangeDisplayName = attributeExchangeDisplayName;
        }
        static internal AttributeMapping LoadFromXml(XElement item)
        {
            AttributeMapping attributeMapping = new AttributeMapping()
            {
                AllowedSyncDirection = Util.LoadFromXml<int>(item.Element(Util.ns.m + "AllowedSyncDirection")),
                AttributeCrmDisplayName = Util.LoadFromXml<string>(item.Element(Util.ns.m + "AttributeCrmDisplayName")),
                AttributeCrmName = Util.LoadFromXml<string>(item.Element(Util.ns.m + "AttributeCrmName")),
                AttributeExchangeDisplayName = Util.LoadFromXml<string>(item.Element(Util.ns.c + "AttributeExchangeDisplayName")),
                AttributeExchangeName = Util.LoadFromXml<string>(item.Element(Util.ns.m + "AttributeExchangeName")),
                AttributeMappingId = Util.LoadFromXml<Guid>(item.Element(Util.ns.m + "AttributeMappingId")),
                //ComputedProperties = Util.LoadFromXml<DataCollection<string>>(item.Element(Util.ns.m + "ComputedProperties")),
                DefaultSyncDirection = Util.LoadFromXml<int>(item.Element(Util.ns.m + "DefaultSyncDirection")),
                EntityTypeCode = Util.LoadFromXml<int>(item.Element(Util.ns.m + "EntityTypeCode")),
                IsComputed = Util.LoadFromXml<bool>(item.Element(Util.ns.m + "IsComputed")),
                MappingName = Util.LoadFromXml<string>(item.Element(Util.ns.m + "MappingName")),
                SyncDirection = Util.LoadFromXml<int>(item.Element(Util.ns.m + "SyncDirection"))
            };
            foreach (XElement value in item.Elements(Util.ns.m + "ComputedProperties").Elements(Util.ns.f + "string"))
            {
                attributeMapping.ComputedProperties.Add(value.Value);
            }
            return attributeMapping;
        }
    }
    public sealed class AttributeMappingCollection : DataCollection<AttributeMapping>
    {
        public AttributeMappingCollection() { }
        public AttributeMappingCollection(IList<AttributeMapping> list)
        {
            this.AddRange(list);
        }
        static internal AttributeMappingCollection LoadFromXml(XElement item)
        {
            AttributeMappingCollection attributeMappingCollection = new AttributeMappingCollection();
            foreach (var attributeMapping in item.Elements(Util.ns.a + "AttributeMapping"))
            {
                attributeMappingCollection.Add(AttributeMapping.LoadFromXml(attributeMapping));
            }
            return attributeMappingCollection;
        }
    }
    public sealed class AttributePrivilege
    {
        public Guid AttributeId { get; set; }
        public int CanCreate { get; set; }
        public int CanRead { get; set; }
        public int CanUpdate { get; set; }
        public AttributePrivilege() { }
        public AttributePrivilege(Guid attributeId, int canCreate, int canRead, int canUpdate)
        {
            AttributeId = attributeId;
            CanCreate = canCreate;
            CanRead = canRead;
            CanUpdate = canUpdate;
        }
        static internal AttributePrivilege LoadFromXml(XElement item)
        {
            AttributePrivilege attributePrivilege = new AttributePrivilege()
            {
                AttributeId = Util.LoadFromXml<Guid>(item.Element(Util.ns.a + "AttributeId")),
                CanCreate = Util.LoadFromXml<int>(item.Element(Util.ns.a + "CanCreate")),
                CanRead = Util.LoadFromXml<int>(item.Element(Util.ns.a + "CanRead")),
                CanUpdate = Util.LoadFromXml<int>(item.Element(Util.ns.a + "CanUpdate"))
            };
            return attributePrivilege;
        }
    }
    public sealed class AttributePrivilegeCollection : DataCollection<AttributePrivilege>
    {
        public AttributePrivilegeCollection() { }
        public AttributePrivilegeCollection(IList<AttributePrivilege> list)
        {
            this.AddRange(list);
        }
        static internal AttributePrivilegeCollection LoadFromXml(XElement item)
        {
            AttributePrivilegeCollection attributePrivilegeCollection = new AttributePrivilegeCollection();
            foreach (var attributePrivilege in item.Elements(Util.ns.a + "AttributePrivilege"))
            {
                attributePrivilegeCollection.Add(AttributePrivilege.LoadFromXml(attributePrivilege));
            }
            return attributePrivilegeCollection;
        }
    }

    // Inheriting from Exception so that this class can be thrown as Exception
    public abstract class BaseServiceFault : Exception
    {
        public int ErrorCode { get; set; }
        public ErrorDetailCollection ErrorDetails { get; set; }
        public new string Message { get; set; }
        public DateTime Timestamp { get; set; }
        static internal void LoadFromXml(XElement item, BaseServiceFault fault)
        {
            if (item.Elements().Count() == 0)
                return;
            fault.ErrorCode = Util.LoadFromXml<int>(item.Element(Util.ns.a + "ErrorCode"));
            fault.ErrorDetails = ErrorDetailCollection.LoadFromXml(item.Element(Util.ns.a + "ErrorDetails"));
            fault.Message = Util.LoadFromXml<string>(item.Element(Util.ns.a + "Message"));
            fault.Timestamp = Util.LoadFromXml<DateTime>(item.Element(Util.ns.a + "Timestamp"));
        }
    }
    public sealed class BooleanManagedProperty : ManagedProperty<bool>
    {
        public BooleanManagedProperty() { }
        public BooleanManagedProperty(bool value)
        {
            this.Value = value;
        }
        internal new string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToValueXml());
            sb.Append(Util.ObjectToXml(Value, "a:Value", true));
            return sb.ToString();
        }
        static internal BooleanManagedProperty LoadFromXml(XElement item)
        {
            if (item.Elements().Count() == 0)
                return new BooleanManagedProperty();
            BooleanManagedProperty booleanManagedProperty = new BooleanManagedProperty();
            ManagedProperty<bool>.LoadFromXml(item, booleanManagedProperty);
            booleanManagedProperty.Value = Util.LoadFromXml<bool>(item.Element(Util.ns.a + "Value"));
            return booleanManagedProperty;
        }
    }
    public class DataCollection<T> : Collection<T>
    {
        public void AddRange(params T[] items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
        public T[] ToArray()
        {
            return this.ToArray<T>();
        }
    }
    public abstract class DataCollection<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public virtual bool IsReadOnly { get; set; }
        public new void Add(TKey key, TValue value)
        {
            if (IsReadOnly)
                return;
            base.Add(key, value);
        }
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (IsReadOnly)
                return;
            this.Add(item.Key, item.Value);
        }
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (IsReadOnly)
                return;
            this.AddRange(items.ToArray());
        }
        public void AddRange(params KeyValuePair<TKey, TValue>[] items)
        {
            if (IsReadOnly)
                return;
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
        public bool Contains(TKey key)
        {
            return base.ContainsKey(key);
        }
        public bool Contains(KeyValuePair<TKey, TValue> key)
        {
            return base.ContainsKey(key.Key) && base.ContainsValue(key.Value);
        }
    }
    public class Entity
    {
        public AttributeCollection Attributes { get; set; }
        public FormattedValueCollection FormattedValues { get; set; }
        public EntityState? EntityState { get; set; }
        public virtual Guid Id { get; set; }
        public string LogicalName { get; set; }
        public RelatedEntityCollection RelatedEntities { get; set; }
        public Entity()
        {
            Attributes = new AttributeCollection();
            FormattedValues = new FormattedValueCollection();
            RelatedEntities = new RelatedEntityCollection();
        }
        public Entity(string LogicalName)
            : this()
        {
            if (!String.IsNullOrEmpty(LogicalName))
                this.LogicalName = LogicalName;
        }
        public Entity(Entity entity)
        {
            Attributes = new AttributeCollection();
            FormattedValues = new FormattedValueCollection();
            RelatedEntities = new RelatedEntityCollection();
            this.LogicalName = entity.LogicalName;
            this.Attributes = entity.Attributes;
            this.FormattedValues = entity.FormattedValues;
            this.RelatedEntities = entity.RelatedEntities;
        }
        public object this[string attributeName]
        {
            get
            {
                if (this.Attributes.ContainsKey(attributeName))
                    return this.Attributes[attributeName];
                else
                    return null;
            }
            set
            {
                this.Attributes[attributeName] = value;
            }
        }
        public bool Contains(string attributeName)
        {
            if (!String.IsNullOrEmpty(attributeName))
                return this.Attributes.Contains(attributeName);
            else
                return false;
        }
        public virtual T GetAttributeValue<T>(string attributeLogicalName)
        {
            // Check if key exists and value type if same as specified.
            if (!String.IsNullOrEmpty(attributeLogicalName) && this.Contains(attributeLogicalName))
            {
                try
                {
                    return (T)this.Attributes[attributeLogicalName];
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
            // If value not exist yet, return default value.
            else
                return default(T);
        }
        protected virtual string GetFormattedAttributeValue(string attributeLogicalName)
        {
            // Check if this contains the key
            if (!String.IsNullOrEmpty(attributeLogicalName) && this.FormattedValues.Contains(attributeLogicalName))
                return this.FormattedValues[attributeLogicalName];
            else
                return null;
        }
        protected virtual IEnumerable<TEntity> GetRelatedEntities<TEntity>(string relationshipSchemaName,
            Nullable<EntityRole> primaryEntityRole) where TEntity : Entity
        {
            if (!String.IsNullOrEmpty(relationshipSchemaName))
            {
                Relationship key = new Relationship(relationshipSchemaName)
                {
                    PrimaryEntityRole = primaryEntityRole
                };

                // Create relationship
                if (RelatedEntities.Contains(key))
                    return RelatedEntities[key].Entities.Cast<TEntity>();
                else
                    return null;
            }
            else
                return null;
        }
        protected virtual TEntity GetRelatedEntity<TEntity>(string relationshipSchemaName,
            Nullable<EntityRole> primaryEntityRole) where TEntity : Entity
        {
            if (!String.IsNullOrEmpty(relationshipSchemaName))
            {
                // Create relationship
                Relationship key = new Relationship(relationshipSchemaName)
                {
                    PrimaryEntityRole = primaryEntityRole
                };

                if (RelatedEntities.Contains(key))
                    return (TEntity)RelatedEntities[key].Entities.FirstOrDefault();
                else
                    return null;
            }
            else
                return null;
        }
        protected virtual void SetAttributeValue(string attributeLogicalName, Object value)
        {
            if (!String.IsNullOrEmpty(attributeLogicalName))
                this[attributeLogicalName] = value;
        }
        protected virtual void SetRelatedEntities<TEntity>(string relationshipSchemaName, Nullable<EntityRole> primaryEntityRole,
            IEnumerable<TEntity> entities) where TEntity : Entity
        {
            // Check schemaname
            if (String.IsNullOrEmpty(relationshipSchemaName))
                return;

            // Create relationship
            Relationship key = new Relationship(relationshipSchemaName)
            {
                PrimaryEntityRole = primaryEntityRole
            };

            // check entities
            if (entities == null || entities.Count() == 0)
            {
                RelatedEntities[key] = null;
                return;
            }

            // Instantiate EntityCollection and pass properties
            EntityCollection collection = new EntityCollection();
            collection.EntityName = entities.First().LogicalName;
            collection.Entities.AddRange(entities);

            RelatedEntities[key] = collection;
        }
        protected virtual void SetRelatedEntity<TEntity>(string relationshipSchemaName, Nullable<EntityRole> primaryEntityRole,
            TEntity entity) where TEntity : Entity
        {
            // Check schemaname
            if (String.IsNullOrEmpty(relationshipSchemaName))
                return;

            // Create relationship
            Relationship key = new Relationship(relationshipSchemaName)
            {
                PrimaryEntityRole = primaryEntityRole
            };

            if (entity == null)
            {
                RelatedEntities[key] = null;
                return;
            }

            // Instantiate EntityCollection and pass properties
            EntityCollection collection = new EntityCollection();
            collection.EntityName = entity.LogicalName;
            collection.Entities.Add(entity);

            RelatedEntities[key] = collection;
        }
        // Convert Entity to early bound class like Account, Contact, etc.
        public T ToEntity<T>() where T : Entity
        {
            // If T is Entity, then just returns it's copy.
            if (typeof(T) == typeof(Entity))
            {
                Entity record = new Entity(this);
                return record as T;
            }

            // Instantiate early bound class object.
            T castedRecord = (T)Activator.CreateInstance(typeof(T), null);
            // Pass properties.
            castedRecord.Id = this.Id;
            castedRecord.Attributes = this.Attributes;
            castedRecord.FormattedValues = this.FormattedValues;
            return castedRecord;
        }
        // Convert Entity to EntityReference
        public EntityReference ToEntityReference()
        {
            if (this.Id == Guid.Empty)
                throw new Exception("Cannot convert to EntityReference without Id value");
            return new EntityReference()
            {
                Id = this.Id,
                LogicalName = this.LogicalName
            };
        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Attributes.ToXml());
            sb.Append("<a:EntityState i:nil='true' />");
            sb.Append(this.FormattedValues.ToXml());
            sb.Append("<a:Id>");
            sb.Append((this.Id == Guid.Empty) ? "00000000-0000-0000-0000-000000000000" : Id.ToString());
            sb.Append("</a:Id>");
            sb.Append("<a:LogicalName>" + this.LogicalName + "</a:LogicalName>");
            sb.Append(this.RelatedEntities.ToXml());
            return sb.ToString();
        }
        static internal Entity LoadFromXml(XElement item)
        {
            Entity entity = new Entity();
            entity.LogicalName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "LogicalName"));
            entity.Id = Util.LoadFromXml<Guid>(item.Element(Util.ns.a + "Id"));
            entity.Attributes.LoadFromXml(item);
            entity.FormattedValues.LoadFromXml(item);
            return BaseOrganizationDataWebServiceProxy.ConvertToEarlyBound(entity);
        }
    }
    public sealed class EntityCollection
    {
        public string EntityName { get; set; }
        public string MinActiveRowVersion { get; set; }
        public bool MoreRecords { get; set; }
        public string PagingCookie { get; set; }
        public int TotalRecordCount { get; set; }
        public bool TotalRecordCountLimitExceeded { get; set; }
        public DataCollection<Entity> Entities { get; set; }
        public EntityCollection()
        {
            this.Entities = new DataCollection<Entity>();
        }
        public EntityCollection(Entity[] Entities)
            : this()
        {
            foreach (Entity item in Entities)
            {
                this.Entities.Add(item);
            }

        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.ObjectToXml(Entities.ToArray(), "a:Entities", true));
            sb.Append(Util.ObjectToXml(EntityName, "a:EntityName", true));
            sb.Append(Util.ObjectToXml(MinActiveRowVersion, "a:MinActiveRowVersion", true));
            sb.Append(Util.ObjectToXml(MoreRecords, "a:MoreRecords", true));
            sb.Append(Util.ObjectToXml(PagingCookie, "a:PagingCookie", true));
            sb.Append(Util.ObjectToXml(TotalRecordCount, "a:TotalRecordCount", true));
            sb.Append(Util.ObjectToXml(TotalRecordCountLimitExceeded, "a:TotalRecordCountLimitExceeded", true));
            return sb.ToString();
        }
        public Entity this[int i]
        {
            get
            {
                if (Entities.Count > 0)
                    return this.Entities[i];
                else
                    return null;
            }
        }
        static internal EntityCollection LoadFromXml(XElement item)
        {
            EntityCollection entityCollection = new EntityCollection()
            {
                EntityName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "EntityName")),
                MinActiveRowVersion = Util.LoadFromXml<string>(item.Element(Util.ns.a + "MinActiveRowVersion")),
                PagingCookie = Util.LoadFromXml<string>(item.Element(Util.ns.a + "PagingCookie")),
                MoreRecords = Util.LoadFromXml<bool>(item.Element(Util.ns.a + "MoreRecords")),
                TotalRecordCount = Util.LoadFromXml<int>(item.Element(Util.ns.a + "TotalRecordCount")),
                TotalRecordCountLimitExceeded = Util.LoadFromXml<bool>(item.Element(Util.ns.a + "TotalRecordCountLimitExceeded")),
            };
            foreach (var entity in item.Element(Util.ns.a + "Entities").Elements(Util.ns.a + "Entity"))
            {
                entityCollection.Entities.Add(Entity.LoadFromXml(entity));
            }
            if (entityCollection.Entities.Count > 0)
                entityCollection.TotalRecordCount = entityCollection.Entities.Count;
            return entityCollection;
        }
    }
    public sealed class EntityImageCollection : DataCollection<string, Entity> { }
    public sealed class EntityReference
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LogicalName { get; set; }
        public EntityReference() { }
        public EntityReference(string logicalName, Guid id)
        {
            this.LogicalName = logicalName;
            this.Id = id;
        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.ObjectToXml(Id, "a:Id", true));
            sb.Append(Util.ObjectToXml(LogicalName, "a:LogicalName", true));
            sb.Append(Util.ObjectToXml(Name, "a:Name", true));
            return sb.ToString();
        }
        static internal EntityReference LoadFromXml(XElement item)
        {
            EntityReference entityReference = new EntityReference()
            {
                Id = Util.LoadFromXml<Guid>(item.Element(Util.ns.a + "Id")),
                LogicalName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "LogicalName")),
                Name = Util.LoadFromXml<string>(item.Element(Util.ns.a + "Name"))
            };
            return entityReference;
        }
    }
    public sealed class EntityReferenceCollection : DataCollection<EntityReference>
    {
        internal string ToValueXml()
        {
            return Util.ObjectToXml(this.ToArray(), "a:EntityReference", true);
        }
        static internal EntityReferenceCollection LoadFromXml(XElement item)
        {
            EntityReferenceCollection entityReferenceCollection = new EntityReferenceCollection();
            foreach (var entityReference in item.Elements(Util.ns.a + "EntityReference"))
            {
                entityReferenceCollection.Add(EntityReference.LoadFromXml(entityReference));
            }
            return entityReferenceCollection;
        }
    }
    public sealed class ErrorDetailCollection : DataCollection<string, Object>
    {
        static internal ErrorDetailCollection LoadFromXml(XElement item)
        {
            ErrorDetailCollection errorDetailCollection = new ErrorDetailCollection()
            {

            };
            return errorDetailCollection;
        }
    }
    public static class FieldPermissionType
    {
        public const int Allowed = 4;
        public const int NotAllowed = 0;
        public static void Validate(int value)
        { }
    }
    public sealed class FormattedValueCollection : DataCollection<string, string>
    {
        internal string ToXml()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Count == 0)
            {
                return sb.Append("<a:FormattedValues/>").ToString();
            }

            sb.Append("<a:FormattedValues>");
            foreach (var item in this)
            {
                sb.Append("<a:KeyValuePairOfstringstring>");
                sb.Append("<b:key>" + item.Key + "</b:key><b:value>" + item.Value + "</b:value>");
                sb.Append("</a:KeyValuePairOfstringstring>");
            }
            sb.Append("</a:FormattedValues>");
            return sb.ToString();
        }
        internal void LoadFromXml(XElement item)
        {
            foreach (var fmt in item.Elements(Util.ns.a + "FormattedValues").Elements(Util.ns.a + "KeyValuePairOfstringstring"))
            {
                FormattedValueLoadFromXml(fmt);
            }
        }
        internal void FormattedValueLoadFromXml(XElement item)
        {
            string key = Util.LoadFromXml<string>(item.Element(Util.ns.b + "key"));
            string value = Util.LoadFromXml<string>(item.Element(Util.ns.b + "value"));
            this.Add(key, value);
        }
    }
    public sealed class Label
    {
        public LocalizedLabelCollection LocalizedLabels { get; set; }
        public LocalizedLabel UserLocalizedLabel { get; set; }
        public Label()
        {
            this.LocalizedLabels = new LocalizedLabelCollection();
        }
        public Label(LocalizedLabel userLocalizedLabel, LocalizedLabel[] labels)
            : this()
        {
            this.UserLocalizedLabel = userLocalizedLabel;
            foreach (LocalizedLabel label in labels)
            {
                this.LocalizedLabels.Add(label);
            }
        }
        public Label(string label, int languageCode)
            : this()
        {
            this.LocalizedLabels.Add(new LocalizedLabel(label, languageCode));
        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.ObjectToXml(LocalizedLabels, "a:LocalizedLabels", true));
            sb.Append(Util.ObjectToXml(UserLocalizedLabel, "a:UserLocalizedLabel", true));
            return sb.ToString();
        }
        static internal Label LoadFromXml(XElement item)
        {
            if (item.Elements().Count() == 0)
                return new Label();
            Label label = new Label()
            {
                UserLocalizedLabel = LocalizedLabel.LoadFromXml(item.Element(Util.ns.a + "UserLocalizedLabel")),
                LocalizedLabels = LocalizedLabelCollection.LoadFromXml(item.Element(Util.ns.a + "LocalizedLabels"))
            };
            return label;
        }
    }
    public sealed class LocalizedLabel : MetadataBase
    {
        public bool? IsManaged { get; set; }
        public string Label { get; set; }
        public int LanguageCode { get; set; }
        public LocalizedLabel() { }
        public LocalizedLabel(string label, int languageCode)
        {
            this.Label = label;
            this.LanguageCode = languageCode;
        }
        internal new string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToValueXml());
            sb.Append(Util.ObjectToXml(IsManaged, "a:IsManaged", true));
            sb.Append(Util.ObjectToXml(Label, "a:Label", true));
            sb.Append(Util.ObjectToXml(LanguageCode, "a:LanguageCode", true));
            return sb.ToString();
        }
        static internal LocalizedLabel LoadFromXml(XElement item)
        {
            if (item.Elements().Count() == 0)
                return new LocalizedLabel();
            LocalizedLabel localizedLabel = new LocalizedLabel()
            {
                IsManaged = Util.LoadFromXml<bool?>(item.Element(Util.ns.a + "IsManaged")),
                Label = Util.LoadFromXml<string>(item.Element(Util.ns.a + "Label")),
                LanguageCode = Util.LoadFromXml<int>(item.Element(Util.ns.a + "LanguageCode")),
            };
            MetadataBase.LoadFromXml(item, localizedLabel);
            return localizedLabel;
        }
    }
    public sealed class LocalizedLabelCollection : DataCollection<LocalizedLabel>
    {
        public LocalizedLabelCollection() { }
        public LocalizedLabelCollection(IList<LocalizedLabel> list)
        {
            this.AddRange(list);
        }
        internal string ToValueXml()
        {
            return Util.ObjectToXml(this.ToArray(), "a:LocalizedLabel", true);
        }
        static internal LocalizedLabelCollection LoadFromXml(XElement item)
        {
            LocalizedLabelCollection LocalizedLabelCollection = new LocalizedLabelCollection();
            foreach (var localizedLabel in item.Elements(Util.ns.a + "LocalizedLabel"))
            {
                LocalizedLabelCollection.Add(LocalizedLabel.LoadFromXml(localizedLabel));
            }
            return LocalizedLabelCollection;
        }
    }
    public abstract class ManagedProperty<T>
    {
        public bool CanBeChanged { get; set; }
        public string ManagedPropertyLogicalName { get; set; }
        public T Value { get; set; }
        public ManagedProperty() { this.CanBeChanged = true; }
        public ManagedProperty(string managedPropertyLogicalName)
        {
            this.ManagedPropertyLogicalName = managedPropertyLogicalName;
            this.CanBeChanged = true;
        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.ObjectToXml(CanBeChanged, "a:CanBeChanged", true));
            sb.Append(Util.ObjectToXml(ManagedPropertyLogicalName, "a:ManagedPropertyLogicalName", true));
            return sb.ToString();
        }
        static internal void LoadFromXml(XElement item, ManagedProperty<T> meta)
        {
            if (item.Elements().Count() == 0)
                return;
            meta.CanBeChanged = Util.LoadFromXml<bool>(item.Element(Util.ns.a + "CanBeChanged"));
            meta.ManagedPropertyLogicalName = Util.LoadFromXml<string>(item.Element(Util.ns.a + "ManagedPropertyLogicalName"));
        }
    }
    public sealed class Money
    {
        public Money()
        {
        }
        public Money(decimal Value)
        {
            this.Value = Value;
        }
        public decimal Value { get; set; }
        internal string ToValueXml()
        {
            return Util.ObjectToXml(Value, "a:Value", true);
        }
        static internal Money LoadFromXml(XElement item)
        {
            Money money = new Money()
            {
                Value = Util.LoadFromXml<decimal>(item.Element(Util.ns.a + "Value"))
            };
            return money;
        }
    }
    public sealed class OptionSetValue
    {
        public int Value { get; set; }
        public OptionSetValue()
        {
        }
        public OptionSetValue(int value)
        {
            this.Value = value;
        }
        internal string ToValueXml()
        {
            return Util.ObjectToXml(Value, "a:Value", true);
        }
        static internal OptionSetValue LoadFromXml(XElement item)
        {
            OptionSetValue optionSet = new OptionSetValue()
            {
                Value = Util.LoadFromXml<int>(item.Element(Util.ns.a + "Value"))
            };
            return optionSet;
        }
    }
    public sealed class OrganizationServiceFault : BaseServiceFault
    {
        public OrganizationServiceFault InnerFault { get; set; }
        public string TraceText { get; set; }
        static internal OrganizationServiceFault LoadFromXml(XElement item)
        {
            if (item.Elements().Count() == 0)
                return null;
            OrganizationServiceFault organizationServiceFault = new OrganizationServiceFault()
            {
                TraceText = Util.LoadFromXml<string>(item.Element(Util.ns.a + "TraceText")),
                InnerFault = OrganizationServiceFault.LoadFromXml(item.Element(Util.ns.a + "InnerFault"))
            };
            BaseServiceFault.LoadFromXml(item, organizationServiceFault);
            return organizationServiceFault;
        }
    }
    public sealed class ParameterCollection : DataCollection<string, Object>
    {
    }
    public sealed class RelatedEntityCollection : DataCollection<Relationship, EntityCollection>
    {
        internal string ToXml()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Count == 0)
            {
                return sb.Append("<a:RelatedEntities/>").ToString();
            }
            sb.Append("<a:RelatedEntities>");
            foreach (var item in this)
            {
                sb.Append(RelatedEntityToXml(item));
            }
            sb.Append("</a:RelatedEntities>");
            return sb.ToString();
        }
        internal string RelatedEntityToXml(KeyValuePair<Relationship, EntityCollection> item)
        {
            return "<a:KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN>"
                + Util.ObjectToXml(item.Key, "b:key", true)
                + Util.ObjectToXml(item.Value, "b:value", true)
                + "</a:KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN>";
        }
    }
    public sealed class Relationship
    {
        public EntityRole? PrimaryEntityRole { get; set; }
        public string SchemaName { get; set; }
        public Relationship()
        {
        }
        public Relationship(string schemaName)
        {
            if (!String.IsNullOrEmpty(schemaName))
                this.SchemaName = schemaName;
        }
        internal string ToValueXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.ObjectToXml(PrimaryEntityRole, "a:PrimaryEntityRole", true));
            sb.Append(Util.ObjectToXml(SchemaName, "a:SchemaName", true));
            return sb.ToString();
        }
    }

    #region OrganizationRequest/Response

    // In this code file, I only put basic Request/Response class.
    // All the other messages are in separate file(s).
    public class OrganizationRequest
    {
        public string RequestName { get; set; }
        public Guid RequestId { get; set; }
        public Object item { get; set; }
        public ParameterCollection Parameters { get; set; }
        // responseType stores each message response instance so that
        // Execute method can return correct type.
        // I am not sure if it is good idea to instantiate the response
        // when request instantiated though.  
        internal OrganizationResponse ResponseType { get; set; }
        // ctor
        public OrganizationRequest()
        {
            this.Parameters = new ParameterCollection();
        }
        public object this[string parameterName]
        {
            get
            {
                if (this.Parameters.ContainsKey(parameterName))
                    return this.Parameters[parameterName];
                else
                    return null;
            }
            set
            {
                this.Parameters[parameterName] = value;
            }
        }
        // Each message request has override method which generates
        // correct SOAP request.
        internal virtual string GetRequestBody() { return ""; }
        /// <summary>
        /// Generate Soap XML request
        /// </summary>
        /// <returns></returns>
        internal string GetSoapBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<d:request>");
            sb.Append((this.Parameters.Count == 0) ? "<a:Parameters />" :
                "<a:Parameters>" + GetParameters() + "</a:Parameters>");
            sb.Append((this.RequestId == null || this.RequestId == Guid.Empty) ? "<a:RequestId i:nil='true' />" :
                "<a:RequestId>" + RequestId.ToString() + "</a:RequestId>");
            sb.Append("<a:RequestName>" + RequestName + "</a:RequestName>");
            sb.Append("</d:request>");
            return sb.ToString();
        }
        /// <summary>
        /// This method generates parameter nodes for Soap request
        /// </summary>
        /// <returns></returns>
        internal string GetParameters()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var parameter in Parameters)
            {
                if (parameter.Value == null)
                    continue;
                sb.Append("<a:KeyValuePairOfstringanyType>");
                sb.Append("<b:key>" + parameter.Key + "</b:key>");
                // Use util class method to generate appropriate node.
                sb.Append(Util.ObjectToXml(parameter.Value, "b:value"));
                sb.Append("</a:KeyValuePairOfstringanyType>");
            }
            return sb.ToString();
        }
    }
    public class OrganizationResponse
    {
        public string ResponseName { get; set; }
        public Collection<KeyValuePair<string, object>> Results { get; set; }
        public string Item { get; set; }
        // Each message response has override method which restore
        // result to its members.
        internal virtual void StoreResult(HttpResponseMessage httpResponse) { }
    }

    #endregion OrganizationRequest/Response
}