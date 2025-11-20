using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsApiController : ControllerBase
    {
        private readonly IFormRepository _formRepository;
        private readonly ILogger<FormsApiController> _logger;

        public FormsApiController(IFormRepository formRepository, ILogger<FormsApiController> logger)
        {
            _formRepository = formRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<dynamic>> CreateForm([FromBody] CreateFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return BadRequest(new { success = false, message = "Form title is required" });
            }

            try
            {
                int formId = await _formRepository.CreateFormAsync(model);
                return Ok(new { success = true, formId = formId, message = "Form created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating form: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<FormListViewModel>>> GetAllForms()
        {
            try
            {
                var forms = await _formRepository.GetAllFormsAsync();
                return Ok(forms);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving forms: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FormModel>> GetFormById(int id)
        {
            try
            {
                var form = await _formRepository.GetFormByIdAsync(id);
                if (form == null)
                    return NotFound(new { success = false, message = "Form not found" });

                return Ok(form);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving form: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("paged")]
        public async Task<ActionResult<DataTableResponse<FormListViewModel>>> GetFormsPaged([FromBody] DataTableRequest request)
        {
            try
            {
                string searchValue = request.Search?.Value ?? "";
                var (forms, totalCount) = await _formRepository.GetFormsPagedAsync(request.Start, request.Length, searchValue);

                var response = new DataTableResponse<FormListViewModel>
                {
                    Draw = request.Draw,
                    RecordsTotal = totalCount,
                    RecordsFiltered = totalCount,
                    Data = forms
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving paged forms: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
