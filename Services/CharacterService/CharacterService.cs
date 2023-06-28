using dotnet_rpg.Dtos.Character;
using AutoMapper;
using dotnet_rpg.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{

    private static List<Character> characters = new List<Character> {
        new Character(),
        new Character { Id = 1, Name = "Sam" }
    };

    private readonly IMapper _mapper;
    private readonly DataContext _context;

    public CharacterService(IMapper mapper, DataContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        Character character = _mapper.Map<Character>(newCharacter);
        _context.Characters.Add(character);
        await _context.SaveChangesAsync();
        response.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
        
        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            Character character = characters.First(c => c.Id == id);
            characters.Remove(character);
            response.Data = characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
    {
        // Initialize the service response
        var response = new ServiceResponse<List<GetCharacterDto>>();

        // Grab the characters from the database via context
        var dbCharacters = await _context.Characters.ToListAsync();

        // Automap from type Character to type GetCharacterDto
        response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();

        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
    {
        // My code (also correct btw)
        // var response = new ServiceResponse<GetCharacterDto>();
        // var dbCharacters = await _context.Characters.ToListAsync();
        // var character = dbCharacters.FirstOrDefault(c => c.Id == id);
        // response.Data = _mapper.Map<GetCharacterDto>(character);
        // return response;

        // Patrick's code
        var response = new ServiceResponse<GetCharacterDto>();
        var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
        response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
    {
        var response = new ServiceResponse<GetCharacterDto>();
        
        try
        {
            Character? character = characters.FirstOrDefault(c => c.Id == updateCharacter.Id);

            _mapper.Map(updateCharacter, character);
            // character.Class = updateCharacter.Class;
            // character.Defense = updateCharacter.Defense;
            // character.HitPoints = updateCharacter.HitPoints;
            // character.Intelligence = updateCharacter.Intelligence;
            // character.Name = updateCharacter.Name;
            // character.Strength = updateCharacter.Strength;

            response.Data = _mapper.Map<GetCharacterDto>(character);
            response.Message = "Your character has been successfully updated";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }
}
