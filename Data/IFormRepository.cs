using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Data
{
    public interface IFormRepository
    {
        Task<int> CreateFormAsync(CreateFormViewModel model);
        Task<List<FormListViewModel>> GetAllFormsAsync();
        Task<FormModel> GetFormByIdAsync(int formId);
        Task<(List<FormListViewModel> Forms, int TotalCount)> GetFormsPagedAsync(int start, int length, string searchValue = "");
    }
}
