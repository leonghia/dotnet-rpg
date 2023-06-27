using Microsoft.AspNetCore.Mvc;
using dotnet_rpg.Services.CharacterService;

namespace dotnet_rpg.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController : ControllerBase
{
    
    private readonly ICharacterService _characterService;

    // dependency injection
    public CharacterController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    /*
     * ActionResult means that we can send specific
     * respon code to the client, e,g. 200, 404, etc.
     */
    
    [HttpGet("GetAll")]
    public async Task<ActionResult<ServiceResponse<List<Character>>>> Get()
    {
        return Ok(await _characterService.GetAllCharacters());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<Character>>> GetSingle(int id)
    {
        return Ok(await _characterService.GetCharacterById(id));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<List<Character>>>> AddCharacter(Character newCharacter)
    {
        
        return Ok(await _characterService.AddCharacter(newCharacter));
    }
}
