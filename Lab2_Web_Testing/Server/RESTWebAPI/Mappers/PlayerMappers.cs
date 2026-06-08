using RESTWebAPI.Model;
using RESTWebAPI.Data.DTO;

namespace RESTWebAPI.Mappers
{
    public static class PlayerMappers
    {
        public static PlayerUsernameDTO ToPlayerUsernameDTO(this PlayerCharacterModel model)
        {
            return new PlayerUsernameDTO 
            {
                ID = model.ID,
                Name = model.Name
            };
        }
    }
}
