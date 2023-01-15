using MagicVilla_WebAPI_Endpoint.Models;
using MagicVilla_WebAPI_Endpoint.Models.Dto;

namespace MagicVilla_WebAPI_Endpoint.Data
{
    public class VillaStore
    {
        public static List<VillaDTO> vallaList = new List<VillaDTO>
            {
                new VillaDTO{ Id=1, Name="Mana Pools", Description="In Western Zimbabwe", Sqft=440},
                new VillaDTO{ Id=2, Name="Honde Valley", Description="In Southern Zimbabwe", Sqft=220}
            };
    }
}
