using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
	[Route("api/camps")]
	[ApiVersion("2.0")]
	[ApiController]
	public class Camps2Controller : ControllerBase
	{
		private readonly ICampRepository _repository;
		private readonly IMapper _mapper;
		private readonly LinkGenerator _linkGenerator;

		public Camps2Controller(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
		{
			_repository = repository;
			_mapper = mapper;
			_linkGenerator = linkGenerator;
		}

		[HttpGet]
		public async Task<IActionResult> Get(bool includeTalks = false)
		{
			try
			{
				var results = await _repository.GetAllCampsAsync(includeTalks);

				var result = new {
					Results = _mapper.Map<CampModel[]>(results),
					Count = results.Count()
				};

				return Ok(result);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}
		}

		[HttpGet("{moniker}")]
		public async Task<ActionResult<CampModel>> Get(string moniker)
		{
			try
			{
				var result = await _repository.GetCampAsync(moniker);

				if (result == null)
				{
					return NotFound();
				}

				return _mapper.Map<CampModel>(result);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}
		}

		[HttpGet("search")]
		public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
		{
			try
			{
				var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

				if (!results.Any())
				{
					return NotFound();
				}

				return _mapper.Map<CampModel[]>(results);
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}
		}

		public async Task<ActionResult<CampModel>> Post(CampModel model)
		{
			try
			{
				var location = _linkGenerator.GetPathByAction("Get", "Camps11", new {
					moniker = model.Moniker
				});

				if (string.IsNullOrWhiteSpace(location))
				{
					return BadRequest("Could not use current moniker");
				}

				var camp = _mapper.Map<Camp>(model);

				_repository.Add(camp);

				if (await _repository.SaveChangesAsync())
				{
					return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}

			return BadRequest();
		}

		[HttpPut("{moniker}")]
		public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
		{
			try
			{
				var oldCamp = await _repository.GetCampAsync(moniker);

				if (oldCamp == null)
				{
					return NotFound($"Could not find camp with moniker of {moniker}");
				}

				_mapper.Map(model, oldCamp);

				if (await _repository.SaveChangesAsync())
				{
					return _mapper.Map<CampModel>(oldCamp);
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}

			return BadRequest();
		}

		[HttpDelete("{moniker}")]
		public async Task<IActionResult> Delete(string moniker)
		{
			try
			{
				var oldCamp = await _repository.GetCampAsync(moniker);

				if (oldCamp == null)
				{
					return NotFound($"Could not find camp with moniker of {moniker}");
				}

				_repository.Delete(oldCamp);

				if (await _repository.SaveChangesAsync())
				{
					return Ok();
				}
			}
			catch (Exception)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
			}

			return BadRequest();
		}
	}
}