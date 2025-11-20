namespace DynamicFormBuilder.Models
{
    public class FormModel
    {
        public int FormId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<DropdownFieldModel> DropdownFields { get; set; } = new List<DropdownFieldModel>();
    }

    public class DropdownFieldModel
    {
        public int FieldId { get; set; }
        public int FormId { get; set; }
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public string SelectedValue { get; set; }
        public string OptionList { get; set; } // JSON or comma-separated values
        public List<string> Options
        {
            get
            {
                if (string.IsNullOrEmpty(OptionList))
                    return new List<string>();
                return OptionList.Split(',').Select(x => x.Trim()).ToList();
            }
        }
    }

    public class CreateFormViewModel
    {
        public string Title { get; set; }
        public List<DropdownFieldViewModel> DropdownFields { get; set; } = new List<DropdownFieldViewModel>();
    }

    public class DropdownFieldViewModel
    {
        public string Label { get; set; }
        public bool IsRequired { get; set; }
        public string SelectedValue { get; set; }
        public List<string> AvailableOptions { get; set; } = new List<string> { "Option 1", "Option 2", "Option 3" };
    }

    public class FormListViewModel
    {
        public int FormId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateFormatted => CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public Search Search { get; set; }
        public List<Order> Order { get; set; }
        public List<Column> Columns { get; set; }
    }

    public class Search
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class Order
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class Column
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public Search Search { get; set; }
    }

    public class DataTableResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; }
    }
}
