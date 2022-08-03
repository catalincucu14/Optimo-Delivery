using ClientAdmin.Services.Api;
using ClientAdmin.Utils;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Models;
using Models.Resources;
using OfficeOpenXml;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public partial class ImportOrdersModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        protected BlazoredModalInstance BlazoredModal { get; set; }

        protected List<Order> Orders { get; set; } = new();

        protected List<string> Logs { get; set; } = new();


        protected string FileName = "";

        protected ComponentState ImportOrdersModalState { get; set; } = new();

        protected bool ImportPanel { get; set; } = false;

        protected bool OrdersPanel { get; set; } = false;

        protected bool LogButton { get; set; } = false;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ImportPanel = true;
        }

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CloseAsync(ModalResult.Ok(true));

        /// <summary>
        /// Function used to read an Excel file an create orders from it
        /// </summary>
        protected async Task UploadExcelAsync(InputFileChangeEventArgs file)
        {
            ImportOrdersModalState.Errors.Clear();
            ImportOrdersModalState.Processing = true;

            Orders.Clear();

            Logs.Clear();

            try
            {
                // Check if the file extension is for Excel file
                if (file.File == null || file.File.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    throw new Exception("Invalid File Format, Only Excel Files (.xlsx)");
                }

                FileName = file.File.Name.Remove(file.File.Name.Length - 5);

                using (MemoryStream memoryStream = new())
                {
                    await file.File.OpenReadStream().CopyToAsync(memoryStream);

                    // Extract orders from the Excel file and send them to the API
                    await ExtractOrdersFromExcelAsync(memoryStream);
                }

            }
            catch (Exception e)
            {
                ImportOrdersModalState.Errors.Add(e.Message);
            }

            ImportOrdersModalState.Processing = false;
        }

        /// <summary>
        /// Function used to extract the orders from an Excel file
        /// </summary>
        private async Task ExtractOrdersFromExcelAsync(MemoryStream bytes)
        {
            using (ExcelPackage package = new())
            {
                package.Load(bytes);

                using (ExcelWorkbook workbook = package.Workbook)
                {
                    // Read the Excel sheet
                    ExcelMapping sheetMapping = new();
                    ExcelWorksheet sheet = workbook.Worksheets[0];

                    // Extract the number of rows and columns
                    sheetMapping.Rows = sheet.Dimension.End.Row;
                    sheetMapping.Columns = sheet.Dimension.End.Column;

                    // Configure the file layout, where are certain columns
                    for (int column = 1; column <= sheetMapping.Columns; column++)
                    {
                        dynamic cellValue = sheet.Cells[1, column].Value;

                        // Check if the cell is empty
                        if (cellValue is not null)
                        {
                            // Map the Excel sheet
                            sheetMapping.ConfigureField(cellValue.ToString().ToLower(), column);
                        }
                    }

                    // Check if the mandatory columns are set
                    if (sheetMapping.Validate().Count != 0)
                    {
                        throw new Exception("Phone, City And Address Are Mandatory Fields");
                    }

                    // Create a list of orders from the Excel file
                    for (int row = 2; row <= sheetMapping.Rows; row++)
                    {
                        Order order = new();

                        bool emptyRow = true;

                        for (int column = 1; column <= sheetMapping.Columns; column++)
                        {
                            dynamic cellValue = sheet.Cells[row, column].Value;

                            // Check if the cell is empty
                            if (cellValue is not null)
                            {
                                emptyRow = false;

                                cellValue = cellValue.ToString();

                                if (column == sheetMapping.CustomId)
                                {
                                    order.CustomId = cellValue;
                                }
                                else if (column == sheetMapping.FirstName)
                                {
                                    order.FirstName = cellValue;
                                }
                                else if (column == sheetMapping.LastName)
                                {
                                    order.LastName = cellValue;
                                }
                                else if (column == sheetMapping.Mail)
                                {
                                    order.Mail = cellValue;
                                }
                                else if (column == sheetMapping.Phone)
                                {
                                    order.Phone = cellValue;
                                }
                                else if (column == sheetMapping.LeftToPay)
                                {
                                    order.LeftToPay = Convert.ToDouble(cellValue);
                                }
                                else if (column == sheetMapping.City)
                                {
                                    order.City = cellValue;
                                }
                                else if (column == sheetMapping.Address)
                                {
                                    order.Address = cellValue;
                                }
                            }
                        }

                        // Check if the row is empty, if so then skip the next steps
                        if (emptyRow)
                        {
                            continue;
                        }

                        // Validate the object and write the logs
                        List<string> errors = order.Validate();

                        if (errors.Count > 0)
                        {
                            Logs.Add($"[FAIL] : line {row}");

                            foreach (string error in errors)
                            {
                                Logs.Add($"    [ERROR] : {error}");
                            }
                        }
                        else
                        {
                            // Sent the orders to the API
                            // The location of the order will be checked
                            // If the location is not valid it will deny the order
                            Response<Order> responseOrder = await ApiService.Orders.CreateAsync(order);

                            if (responseOrder.Success)
                            {
                                Logs.Add($"[SUCCES] : line {row}");

                                Orders.Add(order);
                            }
                            else
                            {
                                Logs.Add($"[FAIL] : line {row}");

                                foreach (string error in responseOrder.Errors)
                                {
                                    Logs.Add($"    [ERROR] : {error}");
                                }
                            }
                        }

                        Logs.Add("\n");
                    }

                    LogButton = true;

                    if (Orders.Count == 0)
                    {
                        throw new Exception("No Valid Orders Could Be Extracted");
                    }
                    else
                    {
                        ImportPanel = false;
                        OrdersPanel = true;
                    }
                }
            }
        }

        /// <summary>
        /// Function used to create a log file for download
        /// </summary>
        public async Task DownloadLogsAsync()
        {
            string logFile = "";

            // Build the log file
            foreach (string log in Logs)
            {
                logFile += log + "\n";
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(logFile);

            // Open a download window 
            await JSRuntime.InvokeAsync<object>("saveAsFile", $"{FileName}.log", Convert.ToBase64String(bytes));
        }
    }
}
