using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Departmanlar;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/departmanlar")]
public class DepartmanlarController : ControllerBase
{
    private readonly IQueryHandler<GetDepartmanlarQuery, List<Departman>> _getDepartmanlarHandler;
    private readonly ICommandHandler<CreateDepartmanCommand, DepartmanIslemSonucu> _createHandler;
    private readonly ICommandHandler<UpdateDepartmanCommand, DepartmanIslemSonucu> _updateHandler;
    private readonly ICommandHandler<DeleteDepartmanCommand, DepartmanIslemSonucu> _deleteHandler;

    public DepartmanlarController(
        IQueryHandler<GetDepartmanlarQuery, List<Departman>> getDepartmanlarHandler,
        ICommandHandler<CreateDepartmanCommand, DepartmanIslemSonucu> createHandler,
        ICommandHandler<UpdateDepartmanCommand, DepartmanIslemSonucu> updateHandler,
        ICommandHandler<DeleteDepartmanCommand, DepartmanIslemSonucu> deleteHandler)
    {
        _getDepartmanlarHandler = getDepartmanlarHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartmanlar()
    {
        var liste = await _getDepartmanlarHandler.Handle(new GetDepartmanlarQuery());

        return Ok(liste);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(CreateDepartmanCommand command)
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
            departmanId = sonuc.DepartmanId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Guncelle(int id, UpdateDepartmanCommand command)
    {
        command.DepartmanId = id;

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
        var sonuc = await _deleteHandler.Handle(new DeleteDepartmanCommand
        {
            DepartmanId = id
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
