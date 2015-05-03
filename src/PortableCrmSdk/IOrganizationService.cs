using System;
using System.Threading.Tasks;

namespace CrmCross
{
    public interface IOrganizationService
    {
        void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
        Guid Create(Entity entity);
        void Delete(string entityName, Guid id);
        void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
        OrganizationResponse Execute(OrganizationRequest request);
        Entity Retrieve(string entityName, Guid id, ColumnSet columnSet);
        EntityCollection RetrieveMultiple(QueryBase query);
        void Update(Entity entity);

        Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
        Task<Guid> CreateAsync(Entity entity);
        Task DeleteAsync(string entityName, Guid id);
        Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities);
        Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request);
        Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet);
        Task<EntityCollection> RetrieveMultipleAsync(QueryBase query);
        Task UpdateAsync(Entity entity);
    }
}
