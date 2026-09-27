using Domain.Entites;

namespace Application.Handlers.ClientHandlers
{
    public class ResponseDto<TEntity>
        where TEntity : BaseDomainEntity
    {
        private object id;
        private object companyName;
        private object contactName;
        private object contactEmail;
        private object contactPhone;
        private object contactAddress;
        private object tradeLicenseNumber;
        private TEntity entity;

        public ResponseDto(TEntity entity)
        {
            this.entity = entity;
        }

        public ResponseDto(object id, object companyName, object contactName, object contactEmail, object contactPhone, object contactAddress, object tradeLicenseNumber)
        {
            this.id = id;
            this.companyName = companyName;
            this.contactName = contactName;
            this.contactEmail = contactEmail;
            this.contactPhone = contactPhone;
            this.contactAddress = contactAddress;
            this.tradeLicenseNumber = tradeLicenseNumber;
        }

        internal static ResponseDto<TEntity> Success(object entity)
        {
            throw new NotImplementedException();
        }
    }
}