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
    private readonly ICommandHandler<CreateCalisanCommand, int> _createCalisanHandler;
    private readonly ICommandHandler<UpdateCalisanCommand, bool> _updateCalisanHandler;
    private readonly ICommandHandler<DeleteCalisanCommand, bool> _deleteCalisanHandler;

    public CalisanlarController(
        IQueryHandler<GetCalisanlarQuery, List<Calisan>> getCalisanlarHandler,
        ICommandHandler<CreateCalisanCommand, int> createCalisanHandler,
        ICommandHandler<UpdateCalisanCommand, bool> updateCalisanHandler,
        ICommandHandler<DeleteCalisanCommand, bool> deleteCalisanHandler)
    {
        _getCalisanlarHandler = getCalisanlarHandler;
        _createCalisanHandler = createCalisanHandler;
        _updateCalisanHandler = updateCalisanHandler;
        _deleteCalisanHandler = deleteCalisanHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalisanlar()
    {
        var liste = await _getCalisanlarHandler.Handle(new GetCalisanlarQuery());

        return Ok(liste);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCalisan(CreateCalisanCommand command)
    {
        var yeniId = await _createCalisanHandler.Handle(command);

        return Created($"api/calisanlar/{yeniId}", new { calisanId = yeniId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCalisan(int id, UpdateCalisanCommand command)
    {
        command.CalisanId = id;

        var guncellendi = await _updateCalisanHandler.Handle(command);

        if (!guncellendi)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCalisan(int id)
    {
        var silindi = await _deleteCalisanHandler.Handle(
            new DeleteCalisanCommand { CalisanId = id });

        if (!silindi)
        {
            return NotFound();
        }

        return NoContent();
    }
}
