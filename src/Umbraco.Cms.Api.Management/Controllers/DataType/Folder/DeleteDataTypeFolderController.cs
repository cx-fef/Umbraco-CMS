﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.ViewModels.Folder;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Api.Management.Controllers.DataType.Folder;

public class DeleteDataTypeFolderController : DataTypeFolderControllerBase
{
    public DeleteDataTypeFolderController(IBackOfficeSecurityAccessor backOfficeSecurityAccessor, IDataTypeService dataTypeService)
        : base(backOfficeSecurityAccessor, dataTypeService)
    {
    }

    [HttpDelete("{key:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid key)
        => await Task.FromResult(DeleteFolder(key));
}