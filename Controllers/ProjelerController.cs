using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Projeler;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Authorize]
[Route("api/projeler")]
public class ProjelerController : ControllerBase
{
    private readonly IQueryHandler<GetProjelerQuery, List<Proje>> _getProjelerHandler;
    private readonly ICommandHandler<CreateProjeCommand, ProjeIslemSonucu> _createHandler;
    private readonly ICommandHandler<UpdateProjeCommand, ProjeIslemSonucu> _updateHandler;
    private readonly ICommandHandler<DeleteProjeCommand, ProjeIslemSonucu> _deleteHandler;

    public ProjelerController(
        IQueryHandler<GetProjelerQuery, List<Proje>> getProjelerHandler,
        ICommandHandler<CreateProjeCommand, ProjeIslemSonucu> createHandler,
        ICommandHandler<UpdateProjeCommand, ProjeIslemSonucu> updateHandler,
        ICommandHandler<DeleteProjeCommand, ProjeIslemSonucu> deleteHandler)
    {
        _getProjelerHandler = getProjelerHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjeler()
    {
        var liste = await _getProjelerHandler.Handle(new GetProjelerQuery());

        return Ok(liste);
    }

    [HttpPost]
    public async Task<IActionResult> Ekle(CreateProjeCommand command)
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
            projeId = sonuc.ProjeId
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Guncelle(int id, UpdateProjeCommand command)
    {
        command.ProjeId = id;

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
        var sonuc = await _deleteHandler.Handle(new DeleteProjeCommand
        {
            ProjeId = id
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
