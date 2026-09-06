using Domain.Entities;
using Domain.QueryEngineConfigs.core;

namespace Domain.QueryEngineConfigs;
public sealed class ClientQueryEngineConfiguration
    : QueryConfiguration<Client>
{
    public override void Configure(QueryConfig<Client> config)
    {
        config.Search(x => x.CompanyName);
        config.Search(x => x.ContactName);
        config.Search(x => x.ContactEmail);
        config.Search(x => x.ContactPhone);
        config.Search(x => x.ContactAddress);
        config.Search(x => x.TradeLicenseNumber);

        config.Filter(x => x.CreatedBy);
        config.Filter(x => x.ContactName);
        config.Filter(x => x.ContactPhone);

        config.Sort(x => x.TradeLicenseNumber);
        config.Sort(x => x.CreatedAt);
        config.Sort(x => x.UpdatedAt);
    }
}