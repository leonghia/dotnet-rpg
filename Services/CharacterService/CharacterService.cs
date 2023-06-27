namespace dotnet_rpg.Services.CharacterService;

public class CharacterService : ICharacterService
{

    private static List<Character> characters = new List<Character> {
        new Character(),
        new Character { Id = 1, Name = "Sam" }
    };

    public async Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter)
    {
        characters.Add(newCharacter);
        return new ServiceResponse<List<Character>> {
            Data = characters,
        };
    }

    public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
    {
        return new ServiceResponse<List<Character>> {
            Data = characters,
        };
    }

    public async Task<ServiceResponse<Character>> GetCharacterById(int id)
    {
        return new ServiceResponse<Character> {
            Data = characters.FirstOrDefault(c => c.Id == id),
        };
    }
}
