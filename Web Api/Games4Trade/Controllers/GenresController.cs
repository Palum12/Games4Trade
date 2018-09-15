using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Games4Trade.Dtos;
using Games4Trade.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Games4Trade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var result = await _genreService.GetGenres();
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] GenreCreateOrUpdateDto genre)
        {
            var result = await _genreService.CreateGenre(genre);
            if (result.IsSuccessful)
            {
                return Ok(result.Payload);
            }

            if (result.Payload != null)
            {
                return BadRequest($"Object {((GenreGetDto) result.Payload).Value} already exists!");
            }

            return StatusCode(500, result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] GenreCreateOrUpdateDto genre)
        {
            var result = await _genreService.EditGenre(id, genre);
            if (result.IsSuccessful)
            {
                return Ok(result.Payload);
            }

            return BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _genreService.DeleteGenre(id);
            if (result.IsSuccessful)
            {
                return Ok();
            }
            else
            {
                if (result.Payload != null)
                {
                    return StatusCode(500, result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
        }
    }
}
