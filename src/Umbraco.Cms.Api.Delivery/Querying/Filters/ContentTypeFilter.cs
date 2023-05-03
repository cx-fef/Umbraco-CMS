using Umbraco.Cms.Api.Delivery.Indexing.Filters;
using Umbraco.Cms.Core.DeliveryApi;

namespace Umbraco.Cms.Api.Delivery.Querying.Filters;

public sealed class ContentTypeFilter : IFilterHandler
{
    private const string ContentTypeSpecifier = "contentType:";

    /// <inheritdoc />
    public bool CanHandle(string query)
        => query.StartsWith(ContentTypeSpecifier, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public FilterOption BuildFilterOption(string filter)
    {
        var alias = filter.Substring(ContentTypeSpecifier.Length);

        var filterOption = new FilterOption
        {
            FieldName = ContentTypeFilterIndexer.FieldName,
            Value = string.Empty
        };

        // TODO: do we support negation?
        if (alias.StartsWith('!'))
        {
            filterOption.Value = alias.Substring(1);
            filterOption.Operator = FilterOperation.IsNot;
        }
        else
        {
            filterOption.Value = alias;
            filterOption.Operator = FilterOperation.Is;
        }

        return filterOption;
    }
}