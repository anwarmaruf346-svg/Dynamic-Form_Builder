using Microsoft.AspNetCore.Mvc;
using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Controllers
{
    public class FormController : Controller
    {
        private readonly IFormRepository _formRepository;
        private readonly ILogger<FormController> _logger;

        public FormController(IFormRepository formRepository, ILogger<FormController> logger)
        {
            _formRepository = formRepository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateFormViewModel
            {
                DropdownFields = new List<DropdownFieldViewModel>
                {
                    new DropdownFieldViewModel
                    {
                        AvailableOptions = new List<string> { "Option 1", "Option 2", "Option 3" }
                    }
                }
            };
            return View(model);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> Create([FromBody] CreateFormViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Create called with null model");
                return BadRequest(new { success = false, message = "Invalid request data" });
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return BadRequest(new { success = false, message = "Form title is required" });
            }

            // Server-side validation for dropdown fields
            if (model.DropdownFields != null)
            {
                for (int i = 0; i < model.DropdownFields.Count; i++)
                {
                    var f = model.DropdownFields[i];
                    if (string.IsNullOrWhiteSpace(f.Label))
                    {
                        return BadRequest(new { success = false, message = $"Label is required for field #{i + 1}" });
                    }
                }
            }

            try
            {
                int formId = await _formRepository.CreateFormAsync(model);
                return Ok(new { success = true, formId = formId, message = "Form created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating form");
                // Return a simple message to the client and log details server-side
                return StatusCode(500, new { success = false, message = "An error occurred while creating the form. Please check server logs for details." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Grid()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetFormsData([FromBody] DataTableRequest request)
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

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting forms data");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            try
            {
                var form = await _formRepository.GetFormByIdAsync(id);
                if (form == null)
                    return NotFound();

                return View(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting form preview");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFormData(int id)
        {
            try
            {
                var form = await _formRepository.GetFormByIdAsync(id);
                if (form == null)
                    return NotFound();

                return Ok(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting form data");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
