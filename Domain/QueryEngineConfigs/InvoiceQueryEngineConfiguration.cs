using Domain.Entities;
using Domain.QueryEngineConfigs.core;

namespace Domain.QueryEngineConfigs;
public sealed class InvoiceQueryEngineConfiguration
    : QueryConfiguration<Invoice>
{
    public override void Configure(QueryConfig<Invoice> config)
    {
        config.Search(x => x.InvoiceNumber);
        config.Search(x => x.CreatedAt); // issued at 
        config.Search(x => x.Client.CompanyName);

        config.Filter(x => x.Status);
        config.Filter(x => x.PaidAt);
        config.Filter(x => x.Client.CompanyName);

        config.Sort(x => x.PaidAt);
        config.Sort(x => x.CreatedAt);
        config.Sort(x => x.UpdatedAt);
    }
}