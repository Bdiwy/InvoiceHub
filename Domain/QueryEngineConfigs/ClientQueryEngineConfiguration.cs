using Domain.Entities;
using Domain.QueryEngineConfigs.core;

namespace Domain.QueryEngineConfigs;
public sealed class ClientQueryEngineConfiguration
    : QueryConfiguration<Client>
{
    public override void Configure(QueryConfig<Client> config)
    {
        config.Search(x => x.AddedBy);
        config.Search(x => x.ContactName);

        config.Filter(x => x.AddedBy);

        config.Sort(x => x.Invoices);
        config.Sort(x => x.CreatedAt);
    }
}