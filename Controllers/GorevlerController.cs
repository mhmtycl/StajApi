using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Gorevler;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/gorevler")]
public class GorevlerController : ControllerBase
{
    private readonly IQueryHandler<GetGorevlerQuery, List<Gorev>> _getGorevlerHandler;
    private readonly ICommandHandler<CreateGorevCommand, GorevIslemSonucu> _createHandler;
    private readonly ICommandHandler<UpdateGorevCommand, GorevIslemSonucu> _updateHandler;
    private readonly ICommandHandler<DeleteGorevCommand, GorevIslemSonucu> _deleteHandler;

    public GorevlerController(
        IQueryHandler<GetGorevlerQuery, List<Gorev>> getGorevlerHandler,
        ICommandHandler<CreateGorevCommand, GorevIslemSonucu> createHandler,
        ICommandHandler<UpdateGorevCommand, GorevIslemSonucu> updateHandler,
        ICommandHandler<DeleteGorevCommand, GorevIslemSonucu> deleteHandler)
    {
        _getGorevlerHandler = getGorevlerHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetGorevler()
    {
        var liste = await _getGorevlerHandler.Handle(new GetGorevlerQuery());

        return Ok(liste);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(CreateGorevCommand command)
    {
        var sonuc = await _createHandler.Handle(command);

        if (!sonuc.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = sonuc.Message
            });
        }

        return Ok(new
        {
            success = true,
            message = sonuc.Message,
            gorevId = sonuc.GorevId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Guncelle(int id, UpdateGorevCommand command)
    {
        command.GorevId = id;

        var sonuc = await _updateHandler.Handle(command);

        if (sonuc.BulunamadiMi)
        {
            return NotFound(new
            {
                success = false,
                message = sonuc.Message
            });
        }

        if (!sonuc.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = sonuc.Message
            });
        }

        return Ok(new
        {
            success = true,
            message = sonuc.Message
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Sil(int id)
    {
        var sonuc = await _deleteHandler.Handle(new DeleteGorevCommand
        {
            GorevId = id
        });

        if (sonuc.BulunamadiMi)
        {
            return NotFound(new
            {
                success = false,
                message = sonuc.Message
            });
        }

        if (!sonuc.Success)
        {
            return BadRequest(new
            {
                success = false,
                message = sonuc.Message
            });
        }

        return Ok(new
        {
            success = true,
            message = sonuc.Message
        });
    }
}
