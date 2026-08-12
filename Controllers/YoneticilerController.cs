using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Yoneticiler;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/yoneticiler")]
public class YoneticilerController : ControllerBase
{
    private readonly IQueryHandler<GetYoneticilerQuery, List<Yonetici>> _getYoneticilerHandler;
    private readonly ICommandHandler<CreateYoneticiCommand, int> _createYoneticiHandler;
    private readonly ICommandHandler<UpdateYoneticiCommand, bool> _updateYoneticiHandler;
    private readonly ICommandHandler<DeleteYoneticiCommand, bool> _deleteYoneticiHandler;

    public YoneticilerController(
        IQueryHandler<GetYoneticilerQuery, List<Yonetici>> getYoneticilerHandler,
        ICommandHandler<CreateYoneticiCommand, int> createYoneticiHandler,
        ICommandHandler<UpdateYoneticiCommand, bool> updateYoneticiHandler,
        ICommandHandler<DeleteYoneticiCommand, bool> deleteYoneticiHandler)
    {
        _getYoneticilerHandler = getYoneticilerHandler;
        _createYoneticiHandler = createYoneticiHandler;
        _updateYoneticiHandler = updateYoneticiHandler;
        _deleteYoneticiHandler = deleteYoneticiHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetYoneticiler()
    {
        var liste = await _getYoneticilerHandler.Handle(new GetYoneticilerQuery());

        return Ok(liste);
    }

    [HttpPost]
    public async Task<IActionResult> CreateYonetici(CreateYoneticiCommand command)
    {
        var yeniId = await _createYoneticiHandler.Handle(command);

        return Created($"api/yoneticiler/{yeniId}", new { yoneticiId = yeniId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateYonetici(int id, UpdateYoneticiCommand command)
    {
        command.YoneticiId = id;

        var guncellendi = await _updateYoneticiHandler.Handle(command);

        if (!guncellendi)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteYonetici(int id)
    {
        var silindi = await _deleteYoneticiHandler.Handle(
            new DeleteYoneticiCommand { YoneticiId = id });

        if (!silindi)
        {
            return NotFound();
        }

        return NoContent();
    }
}
