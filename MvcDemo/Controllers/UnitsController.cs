using AppCoreLite.Entities;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Services;
using Microsoft.AspNetCore.Mvc;

namespace MvcDemo.Controllers
{
	/*
	Main Web Api demo is in WebApiDemo project of this solution. 
    UnitsController.cs, Units.html and Units.js files in this project are only for MVC demo purpose.  
	*/
	[Route("api/[controller]")]
	[ApiController]
	public class UnitsController : ControllerBase
	{
		private readonly TreeNodeService _unitService;

		public UnitsController(TreeNodeService unitService)
		{
			_unitService = unitService;
			_unitService.Set(Languages.English);
		}

		[HttpPost("[action]")]
		public IActionResult GetUnits(TreeNodeFilter filter)
		{
			var units = _unitService.GetJqueryOrgchartNodes(filter);
			return Ok(units);
		}

		[HttpGet("[action]/{id}")]
		public IActionResult GetUnit(int id)
		{
			var unit = _unitService.GetItem(id);
			if (unit == null)
				return NotFound();
			return Ok(unit);
		}

		[HttpPost("[action]")]
		public IActionResult PostUnit(TreeNode unit)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var result = _unitService.Add(unit);
			if (!result.IsSuccessful)
				return BadRequest(result.Message);
			return Ok(unit);
		}

		[HttpPost("[action]/{id}")]
		public IActionResult PutUnit(int id, TreeNode unit)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			unit.Id = id;
			var result = _unitService.Update(unit);
			if (!result.IsSuccessful)
				return BadRequest(result.Message);
			return Ok(unit);
		}

		[HttpPost("[action]/{id}")]
		public IActionResult DeleteUnit(int id)
		{
			var result = _unitService.Delete(id);
			if (!result.IsSuccessful)
				return BadRequest(result.Message);
			return Ok(id);
		}

		[HttpGet("[action]/{level}")]
		public IActionResult GetParentUnits(int level)
		{
			var units = _unitService.GetNodesByLevel(level);
			return Ok(units);
		}

		[HttpGet("[action]/{level}")]
		public IActionResult GetPositions(int level)
		{
			var positions = _unitService.GetDetailNodes(level);
			return Ok(positions);
		}

		[HttpGet("[action]/{id}")]
		public IActionResult GetPosition(int id)
		{
			var position = _unitService.GetDetailNode(id);
			if (position == null)
				return NotFound();
			return Ok(position);
		}

		[HttpPost("[action]/{id}")]
		public IActionResult DeletePosition(int id)
		{
			var result = _unitService.DeleteDetailNode(id);
			if (!result.IsSuccessful)
				return BadRequest(result.Message);
			return Ok(result.Message);
		}
	}
}
