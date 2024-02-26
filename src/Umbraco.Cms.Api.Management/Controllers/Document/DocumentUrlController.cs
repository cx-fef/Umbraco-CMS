﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Factories;
using Umbraco.Cms.Api.Management.ViewModels.Document;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Api.Management.Controllers.Document;

[ApiVersion("1.0")]
public class DocumentUrlController : DocumentControllerBase
{
    private readonly IContentService _contentService;
    private readonly IDocumentUrlFactory _documentUrlFactory;

    public DocumentUrlController(
        IContentService contentService,
        IDocumentUrlFactory documentUrlFactory)
    {
        _contentService = contentService;
        _documentUrlFactory = documentUrlFactory;
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [HttpGet("urls")]
    [ProducesResponseType(typeof(Dictionary<Guid, DocumentUrlInfo>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUrls([FromQuery(Name = "id")] HashSet<Guid> ids)
    {
        IEnumerable<IContent> items = _contentService.GetByIds(ids);

        return Ok(await items.ToDictionaryAsync(content => content.Key, async content => await _documentUrlFactory.GetUrlsAsync(content)));
    }
}
