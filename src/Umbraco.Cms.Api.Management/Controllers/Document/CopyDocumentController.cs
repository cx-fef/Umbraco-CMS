﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Security.Authorization.Content;
using Umbraco.Cms.Api.Management.ViewModels.Document;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Cms.Api.Management.Controllers.Document;

[ApiVersion("1.0")]
public class CopyDocumentController : DocumentControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentEditingService _contentEditingService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public CopyDocumentController(
        IAuthorizationService authorizationService,
        IContentEditingService contentEditingService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
        _authorizationService = authorizationService;
        _contentEditingService = contentEditingService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    [HttpPost("{id:guid}/copy")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Copy(Guid id, CopyDocumentRequestModel copyDocumentRequestModel)
    {
        AuthorizationResult authorizationResult;

        if (copyDocumentRequestModel.TargetId.HasValue is false)
        {
            authorizationResult = await _authorizationService.AuthorizeAsync(User, new[] { ActionCopy.ActionLetter },
                $"New{AuthorizationPolicies.ContentPermissionAtRoot}");
        }
        else
        {
            var resource = new ContentPermissionResource(copyDocumentRequestModel.TargetId.Value, ActionCopy.ActionLetter);
            authorizationResult = await _authorizationService.AuthorizeAsync(User, resource,
                $"New{AuthorizationPolicies.ContentPermissionByResource}");
        }

        if (!authorizationResult.Succeeded)
        {
            return Forbidden();
        }

        Attempt<IContent?, ContentEditingOperationStatus> result = await _contentEditingService.CopyAsync(
            id,
            copyDocumentRequestModel.TargetId,
            copyDocumentRequestModel.RelateToOriginal,
            copyDocumentRequestModel.IncludeDescendants,
            CurrentUserKey(_backOfficeSecurityAccessor));

        return result.Success
            ? CreatedAtAction<ByKeyDocumentController>(controller => nameof(controller.ByKey), result.Result!.Key)
            : ContentEditingOperationStatusResult(result.Status);
    }
}