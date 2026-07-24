// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace cCoder.Mail.Exposures.Controllers;

[ApiController]
[Route("Api/Mail/Baseline")]
public sealed class BaselineController(
    IUIBaselineExposure uiBaselineExposure) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: uiBaselineExposure.GetPackages());
}