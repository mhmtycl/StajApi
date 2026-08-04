using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Calisanlar;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/calisanlar")]
public class CalisanlarController : ControllerBase
{
    private readonly IQueryHandler<GetCalisanlarQuery, List<Calisan>> _getCalisanlarHandler;

    public CalisanlarController(
        IQueryHandler<GetCalisanlarQuery, List<Calisan>> getCalisanlarHandler)
    {
        _getCalisanlarHandler = getCalisanlarHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalisanlar()
    {
        var liste = await _getCalisanlarHandler.Handle(new GetCalisanlarQuery());

        return Ok(liste);
    }
}
