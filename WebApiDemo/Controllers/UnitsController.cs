using AppCoreLite.Entities;
using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UnitsController : ControllerBase
    {
        private readonly TreeNodeService _unitService;

        public UnitsController(TreeNodeService unitService)
        {
            _unitService = unitService;
            _unitService.Set(Languages.English);
        }

        [HttpPost("GetUnits")]
        public IActionResult GetUnits(TreeNodeFilter filter)
        {
            var units = _unitService.GetJqueryOrgchartNodes(filter);
            return Ok(units);
        }

        [HttpGet("GetUnit/{id}")]
        public IActionResult GetUnit(int id)
        {
            var unit = _unitService.GetItem(id);
            if (unit == null)
                return NotFound();
            return Ok(unit);
        }

        [HttpPost("PostUnit")]
        public IActionResult PostUnit(TreeNode unit)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = _unitService.Add(unit);
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
            return Ok(unit);
        }

        [HttpPut("PutUnit/{id}")]
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

        [HttpDelete("DeleteUnit/{id}")]
        public IActionResult DeleteUnit(int id)
        {
            var result = _unitService.Delete(id);
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
            return Ok(id);
        }

        [HttpGet("GetParentUnits/{level}")]
        public IActionResult GetParentUnits(int level)
        {
            var units = _unitService.GetNodesByLevel(level);
            return Ok(units);
        }

        [HttpGet("GetPositions/{level}")]
        public IActionResult GetPositions(int level)
        {
            var positions = _unitService.GetDetailNodes(level);
            return Ok(positions);
        }

        [HttpGet("GetPosition/{id}")]
        public IActionResult GetPosition(int id)
        {
            var position = _unitService.GetDetailNode(id);
            if (position == null)
                return NotFound();
            return Ok(position);
        }

        [HttpDelete("DeletePosition/{id}")]
        public IActionResult DeletePosition(int id)
        {
            var result = _unitService.DeleteDetailNode(id);
            if (!result.IsSuccessful)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
