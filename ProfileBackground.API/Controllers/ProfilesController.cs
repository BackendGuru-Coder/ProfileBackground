using Microsoft.AspNetCore.Mvc;
using ProfileBackground.Domain.Interfaces;
using ProfileBackground.Domain.Models;

namespace ProfileBackground.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// API para listar todos os perfis 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll() => Ok(_profileService.GetAll());

        /// <summary>
        /// API para listar um único perfil
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        [HttpGet("{profileName}")]
        public IActionResult Get(string profileName)
        {
            var profile = _profileService.Get(profileName);
            return profile != null ? Ok(profile) : NotFound();
        }

        /// <summary>
        /// API para adicionar um novo perfil
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add(ProfileParameter profile)
        {
            _profileService.Add(profile);
            return CreatedAtAction(nameof(Get), new { profileName = profile.ProfileName }, profile);
        }

        /// <summary>
        /// API para atualizar o(s) parâmetro(s) do perfil
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPut("{profileName}")]
        public IActionResult Update(string profileName, Dictionary<string, string> parameters)
        {
            _profileService.Update(profileName, parameters);
            return NoContent();
        }

        /// <summary>
        /// API para deletar um perfil existente
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        [HttpDelete("{profileName}")]
        public IActionResult Delete(string profileName)
        {
            _profileService.Delete(profileName);
            return NoContent();
        }

        /// <summary>
        /// API para validar se o perfil possui permissão para uma ação específica
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpGet("{profileName}/validate")]
        public IActionResult Validate(string profileName, [FromQuery] string action)
        {
            var result = _profileService.Validate(profileName, action);
            return Ok(new { HasPermission = result });
        }
    }
}
