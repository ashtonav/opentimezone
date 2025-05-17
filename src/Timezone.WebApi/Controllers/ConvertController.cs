namespace TimezoneWebApi.Controllers;

using Examples;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Timezone.Core.Extensions;
using Timezone.Core.Models;
using Timezone.Core.Services;

[ApiController]
[Route("[controller]")]
public class ConvertController(ITimezoneService timezoneService) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeZoneConversionResultResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [SwaggerRequestExample(typeof(TimeZoneConversionRequest), typeof(PostConvertExamples))]
    public async Task<TimeZoneConversionResultResponse> ConvertTime(TimeZoneConversionRequest request)
    {
        var domain = request.ToDomain();

        var conversionResult = timezoneService.ConvertFromOneTimezoneToAnother(domain);

        return conversionResult.ToResponse();
    }
}
