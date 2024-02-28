using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using System.Windows.Forms;

namespace FacebookCreator.Helper
{
    public class DataGridViewHelper
    {
        public static void SetCellValue(DataGridView dataGridView, int rowIndex, string columnName, object cellValue, int type = 0)
        {
            try
            {
                try
                {
                    if (rowIndex >= 0)
                    {
                        dataGridView.Invoke((MethodInvoker)delegate
                        {
                            dataGridView.Rows[rowIndex].Cells[columnName].Value = cellValue;
                            switch (type)
                            {
                                case 0:
                                    {
                                        dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;

                                        break;
                                    }
                                case 1:
                                    {
                                        dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Pink;

                                        break;
                                    }
                                case 2:
                                    {

                                        dataGridView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                                        break;
                                    }
                            }
                        });
                    }
                    
                }
                catch
                {
                    dataGridView.Rows[rowIndex].Cells[columnName].Value = cellValue;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
        }
        public static string GetCellValue(DataGridView dataGridView, int rowIndex, string columnName)
        {
            string cellValue = "";
            try
            {
                if (dataGridView.Rows[rowIndex].Cells[columnName].Value != null)
                {
                    try
                    {
                        cellValue = dataGridView.Rows[rowIndex].Cells[columnName].Value.ToString();
                    }
                    catch
                    {
                        dataGridView.Invoke((MethodInvoker)delegate
                        {
                            cellValue = dataGridView.Rows[rowIndex].Cells[columnName].Value.ToString();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
            return cellValue;
        }
        public static int GetRowIndexByColumnValue(DataGridView dataGridView, string columnName, string columnValue)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    string value =  string.Empty;
                    dataGridView.Invoke((MethodInvoker)delegate
                    {
                         value = dataGridView.Rows[row.Index].Cells[columnName].Value.ToString();
                    });
                    if (!string.IsNullOrEmpty(value) && value == columnValue)
                    {
                        return row.Index;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                return -1;
            }
        }
        public static void SetCellValueByColumnValue(DataGridView dataGridView, string columnValue, string columnName, string columnNameResult, object cellValueResult, int type = 0)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    string value = string.Empty;
                    dataGridView.Invoke((MethodInvoker)delegate
                    {
                        value = dataGridView.Rows[row.Index].Cells[columnName].Value.ToString();
                    });
                    if (!string.IsNullOrEmpty(value) && value == columnValue)
                    {
                        dataGridView.Invoke((MethodInvoker)delegate
                        {
                            row.Cells[columnNameResult].Value = cellValueResult;
                            switch (type)
                            {
                                case 0:
                                    {
                                        row.DefaultCellStyle.BackColor = Color.White;
                                        break;
                                    }
                                case 1:
                                    {
                                        row.DefaultCellStyle.BackColor = Color.Pink;
                                        break;
                                    }
                                case 2:
                                    {
                                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                                        break;
                                    }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
        }

    }
}
