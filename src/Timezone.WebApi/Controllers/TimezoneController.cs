namespace TimezoneWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Timezone.Core.Extensions;
using Timezone.Core.Models;
using Timezone.Core.Services;

[ApiController]
public class TimezoneController(ITimezoneService timezoneService) : Controller
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimezoneResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [Route("timezone/{*timezoneId}")]
    public async Task<IActionResult> GetTimezone(string timezoneId)
    {
        var timezone = timezoneService.GetTimezone(timezoneId);

        return timezone is null
            ? NotFound()
            : Ok(timezone.ToResponse());
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("timezones")]
    public async Task<IEnumerable<TimezoneResponse>> GetTimezones()
    {
        var timezones = timezoneService.GetTimezones();

        return timezones.ToResponse();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTimezoneAbbreviationResponse))]
    [Route("timezones/abbreviations")]
    public async Task<ActionResult> GetTimezoneAbbreviations()
    {
        var timezoneAbbreviations = timezoneService.GetAllTimezoneAbbreviations();
        var response = timezoneAbbreviations.ToResponse();
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTimezoneAbbreviationResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [Route("timezones/abbreviations/search")]
    public async Task<ActionResult> GetTimezoneAbbreviationsSearch(string? abbreviation, bool includeNumeric = true)
    {
        var timezoneAbbreviations = timezoneService.GetAllTimezoneAbbreviations();

        if (!string.IsNullOrEmpty(abbreviation))
        {
            timezoneAbbreviations = timezoneAbbreviations
                .Where(x => string.Equals(x.Abbreviation, abbreviation, StringComparison.InvariantCultureIgnoreCase));
        }

        if (!includeNumeric)
        {
            timezoneAbbreviations = timezoneAbbreviations
                .Where(x => x.Abbreviation.First() is not '-' and not '+');
        }

        if (!timezoneAbbreviations.Any())
        {
            return NotFound();
        }

        var response = timezoneAbbreviations.ToResponse();
        return Ok(response);
    }
}
