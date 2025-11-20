using System.Data;
using System.Data.SqlClient;
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Data
{
    public class FormRepository : IFormRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<FormRepository> _logger;

        public FormRepository(IConfiguration configuration, ILogger<FormRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        public async Task<int> CreateFormAsync(CreateFormViewModel model)
        {
            int formId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_CreateForm", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Title", model.Title ?? "");

                        SqlParameter formIdParam = new SqlParameter("@FormId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(formIdParam);

                        await command.ExecuteNonQueryAsync();
                        formId = (int)formIdParam.Value;
                    }

                    // Insert dropdown fields
                    if (model.DropdownFields != null && model.DropdownFields.Count > 0)
                    {
                        foreach (var field in model.DropdownFields)
                        {
                            using (SqlCommand command = new SqlCommand("sp_CreateDropdownField", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@FormId", formId);
                                command.Parameters.AddWithValue("@Label", field.Label ?? "");
                                command.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                                command.Parameters.AddWithValue("@SelectedValue", field.SelectedValue ?? "");
                                command.Parameters.AddWithValue("@OptionList", string.Join(",", field.AvailableOptions));

                                await command.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }

                return formId;
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error while creating form: {Message}", sqlEx.Message);
                throw; // let controller handle the exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating form: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<FormListViewModel>> GetAllFormsAsync()
        {
            var forms = new List<FormListViewModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_GetAllForms", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            forms.Add(new FormListViewModel
                            {
                                FormId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CreatedDate = reader.GetDateTime(2)
                            });
                        }
                    }
                }
            }

            return forms;
        }

        public async Task<FormModel> GetFormByIdAsync(int formId)
        {
            FormModel form = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("sp_GetFormById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FormId", formId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            form = new FormModel
                            {
                                FormId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CreatedDate = reader.GetDateTime(2),
                                DropdownFields = new List<DropdownFieldModel>()
                            };
                        }
                    }
                }

                // Get dropdown fields
                if (form != null)
                {
                    using (SqlCommand command = new SqlCommand("sp_GetFormFields", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FormId", formId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                form.DropdownFields.Add(new DropdownFieldModel
                                {
                                    FieldId = reader.GetInt32(0),
                                    FormId = reader.GetInt32(1),
                                    Label = reader.GetString(2),
                                    IsRequired = reader.GetBoolean(3),
                                    SelectedValue = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                    OptionList = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                                });
                            }
                        }
                    }
                }
            }

            return form;
        }

        public async Task<(List<FormListViewModel> Forms, int TotalCount)> GetFormsPagedAsync(int start, int length, string searchValue = "")
        {
            var forms = new List<FormListViewModel>();
            int totalCount = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get total count
                using (SqlCommand command = new SqlCommand("sp_GetFormsCount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@SearchValue", searchValue ?? "");

                    totalCount = (int)await command.ExecuteScalarAsync();
                }

                // Get paged data
                using (SqlCommand command = new SqlCommand("sp_GetFormsPaged", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Start", start);
                    command.Parameters.AddWithValue("@Length", length);
                    command.Parameters.AddWithValue("@SearchValue", searchValue ?? "");

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            forms.Add(new FormListViewModel
                            {
                                FormId = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CreatedDate = reader.GetDateTime(2)
                            });
                        }
                    }
                }
            }

            return (forms, totalCount);
        }
    }
}
