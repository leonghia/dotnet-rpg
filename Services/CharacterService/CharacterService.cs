using dotnet_rpg.Dtos.Character;
using AutoMapper;
using dotnet_rpg.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{
    private readonly IMapper _mapper;
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

    public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _mapper = mapper;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        Character character = _mapper.Map<Character>(newCharacter);

        character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        response.Data = await _context.Characters.Where(c => c.User.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();

        return response;
    }

    public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
    {
        var response = new ServiceResponse<List<GetCharacterDto>>();
        try
        {
            Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

            if (character is not null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                response.Data = await _context.Characters.Where(c => c.User.Id == GetUserId()).Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            else
            {
                response.Success = false;
                response.Message = "Character not found or you do not have sufficient privilage to delete this character";
            }
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
        var dbCharacters = await _context.Characters.Include(c => c.Skills).Where(c => c.User.Id == GetUserId()).ToListAsync();

        // Map from type Character to type GetCharacterDto
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
        var dbCharacter = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
        response.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
    {
        var response = new ServiceResponse<GetCharacterDto>();

        try
        {
            var character = await _context.Characters.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);

            if (character is not null)
            {

                if (character.User.Id == GetUserId())
                {
                    _mapper.Map(updateCharacter, character);
                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    response.Success = false;
                    response.Message = "You do not have sufficient privilage to update this character.";
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Character not found";
            }
            
            /*
             * The mapper above could be done manually like below
             */
            // character.Class = updateCharacter.Class;
            // character.Defense = updateCharacter.Defense;
            // character.HitPoints = updateCharacter.HitPoints;
            // character.Intelligence = updateCharacter.Intelligence;
            // character.Name = updateCharacter.Name;
            // character.Strength = updateCharacter.Strength;



        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.StackTrace;
        }

        return response;
    }

    public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
    {
        var response = new ServiceResponse<GetCharacterDto>();

        try
        {
            var character = await _context.Characters.Include(c => c.Weapon).Include(c => c.Skills).FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());

            if (character is null)
            {
                response.Success = false;
                response.Message = "Character not found";
                return response;
            }

            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);

            if (skill is null)
            {
                response.Success = false;
                response.Message = "Skill not found";
                return response;
            }

            character.Skills.Add(skill);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterDto>(character);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }
}
