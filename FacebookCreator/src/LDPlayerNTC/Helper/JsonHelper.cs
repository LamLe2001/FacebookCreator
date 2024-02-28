using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LDPlayerAndADBController.Helper
{
    public class JsonHelper
    {
        private string configurationFile;

        private JObject _jobject;

        /// <summary>
        /// Khởi tạo một đối tượng JsonHelper.
        /// </summary>
        /// <param name="configurationString">Chuỗi cấu hình hoặc tên tệp cấu hình.</param>
        /// <param name="isJsonString">Xác định liệu configurationString có phải là chuỗi JSON hay không (mặc định là false).</param>
        public JsonHelper(string configurationString, bool isJsonString = false)
        {
            if (isJsonString)
            {
                if (configurationString.Trim() == "")
                {
                    configurationString = "{}";
                }
                _jobject = JObject.Parse(configurationString);
                return;
            }
            try
            {
                if (configurationString.Contains("\\") || configurationString.Contains("/"))
                {
                    configurationFile = configurationString;
                }
                else
                {
                    configurationFile = "settings\\" + configurationString + ".json";
                }
                if (!File.Exists(configurationFile))
                {
                    using (File.AppendText(configurationFile))
                    {
                    }
                }
                _jobject = JObject.Parse(File.ReadAllText(configurationFile));
            }
            catch
            {
                _jobject = new JObject();
            }
        }

        /// <summary>
        /// Chuyển đổi một đối tượng JObject thành một từ điển (Dictionary) có kiểu dữ liệu chung (object).
        /// </summary>
        /// <param name="jObject">Đối tượng JObject đầu vào.</param>
        /// <returns>Từ điển (Dictionary) chứa thông tin từ đối tượng JObject.</returns>
        public Dictionary<string, object> ConvertJObjectToDictionary(JObject jObject)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            try
            {
                dictionary = jObject.ToObject<Dictionary<string, object>>();
                List<string> objectKeys = (from kvp in dictionary
                                           where kvp.Value.GetType() == typeof(JObject)
                                           select kvp.Key).ToList();

                List<string> arrayKeys = (from kvp in dictionary
                                          where kvp.Value.GetType() == typeof(JArray)
                                          select kvp.Key).ToList();

                arrayKeys.ForEach(key =>
                {
                    dictionary[key] = ((JArray)dictionary[key]).Values().Select(x => ((JValue)x).Value).ToArray();
                });

                objectKeys.ForEach(key =>
                {
                    dictionary[key] = ConvertJObjectToDictionary(dictionary[key] as JObject);
                });
            }
            catch
            {
            }
            return dictionary;
        }


        public JsonHelper()
        {
            _jobject = new JObject();
        }

        /// <summary>
        /// Lấy giá trị từ một chuỗi đầu vào trong đối tượng JObject.
        /// </summary>
        /// <param name="key">Khóa (key) của giá trị cần lấy.</param>
        /// <param name="defaultValue">Giá trị mặc định trả về nếu không tìm thấy giá trị.</param>
        /// <returns>Giá trị tương ứng với khóa (key) hoặc giá trị mặc định nếu không tìm thấy.</returns>
        public string GetValuesFromInputString(string key, string defaultValue = "")
        {
            string result = defaultValue;
            try
            {
                result = (_jobject[key] == null) ? defaultValue : _jobject[key].ToString();
            }
            catch
            {
            }
            return result;
        }


        /// <summary>
        /// Lấy danh sách các giá trị từ một chuỗi đầu vào.
        /// </summary>
        /// <param name="inputString">Chuỗi đầu vào.</param>
        /// <param name="separatorType">Loại phân tách (mặc định là 0).</param>
        /// <returns>Danh sách các giá trị.</returns>
        public List<string> GetValuesList(string inputString, int separatorType = 0)
        {
            List<string> values = new List<string>();
            try
            {
                values = separatorType != 0 ? GetValuesFromInputString(inputString).Split(new string[1] { "\n|\n" }, StringSplitOptions.RemoveEmptyEntries).ToList() : GetValuesFromInputString(inputString).Split('\n').ToList();
            }
            catch
            {
            }
            return values;
        }

        /// <summary>
        /// Lấy loại phân tách từ một chuỗi đầu vào.
        /// </summary>
        /// <param name="inputString">Chuỗi đầu vào.</param>
        /// <param name="defaultType">Loại phân tách mặc định (mặc định là 0).</param>
        /// <returns>Loại phân tách.</returns>
        public int GetIntType(string inputString, int defaultType = 0)
        {
            int separatorType = defaultType;
            try
            {
                separatorType = (_jobject[inputString] == null) ? defaultType : Convert.ToInt32(_jobject[inputString].ToString());
            }
            catch
            {
            }
            return separatorType;
        }

        /// <summary>
        /// Lấy giá trị boolean từ một chuỗi đầu vào.
        /// </summary>
        /// <param name="inputString">Chuỗi đầu vào.</param>
        /// <param name="defaultValue">Giá trị mặc định (mặc định là false).</param>
        /// <returns>Giá trị boolean.</returns>
        public bool GetBooleanValue(string inputString, bool defaultValue = false)
        {
            bool value = defaultValue;
            try
            {
                value = (_jobject[inputString] == null) ? defaultValue : Convert.ToBoolean(_jobject[inputString].ToString());
                return value;
            }
            catch
            {
                return value;
            }
        }


        /// <summary>
        /// Thêm hoặc cập nhật một thuộc tính trong đối tượng JObject.
        /// </summary>
        /// <param name="key">Khóa (key) của thuộc tính cần thêm hoặc cập nhật.</param>
        /// <param name="value">Giá trị của thuộc tính cần thêm hoặc cập nhật.</param>
        public void AddOrUpdateProperty(string key, string value)
        {
            try
            {
                if (!_jobject.ContainsKey(key))
                {
                    _jobject.Add(key, JToken.FromObject(value));
                }
                else
                {
                    _jobject[key] = JToken.FromObject(value);
                }
            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// Thêm giá trị vào thuộc tính trong đối tượng JObject.
        /// </summary>
        /// <param name="key">Khóa (key) của thuộc tính cần thêm giá trị.</param>
        /// <param name="value">Giá trị cần thêm.</param>
        public void AddValue(string key, object value)
        {
            try
            {
                _jobject[key] = JToken.FromObject(value.ToString());
            }
            catch
            {
            }
        }
		/// <summary>
		/// Cập nhật giá trị của một thuộc tính trong đối tượng JSON với danh sách các chuỗi.
		/// </summary>
		/// <param name="string_1">Tên thuộc tính cần cập nhật.</param>
		/// <param name="list_0">Danh sách các chuỗi để cập nhật.</param>
		public void AddValueList(string string_1, List<string> list_0)
		{
			try
			{
				bool containsNewLine = list_0.Any(item => item.Contains("\n"));

				if (containsNewLine)
				{
					_jobject[string_1] = (JToken)string.Join("\n|\n", list_0);
				}
				else
				{
					_jobject[string_1] = (JToken)string.Join("\n", list_0);
				}
			}
			catch
			{
				// Xử lý ngoại lệ tại đây (nếu cần)
			}
		}

		/// <summary>
		/// Cập nhật giá trị của thuộc tính trong đối tượng JObject từ danh sách các chuỗi.
		/// </summary>
		/// <param name="key">Khóa (key) của thuộc tính cần cập nhật giá trị.</param>
		/// <param name="list">Danh sách chuỗi cần cập nhật.</param>
		/// <param name="formatType">Kiểu định dạng của danh sách chuỗi.</param>
		public void UpdateValues(string key, List<string> list, int formatType = 0)
        {
            try
            {
                string separator = (formatType == 0) ? "\n" : "\n|\n";
                _jobject[key] = JToken.FromObject(string.Join(separator, list));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Cập nhật giá trị của thuộc tính trong đối tượng JObject từ danh sách các chuỗi.
        /// </summary>
        /// <param name="key">Khóa (key) của thuộc tính cần cập nhật giá trị.</param>
        /// <param name="list">Danh sách chuỗi cần cập nhật.</param>
        public void UpdateValues(string key, List<string> list)
        {
            try
            {
                bool hasNewLine = false;
                foreach (string item in list)
                {
                    if (item.Contains("\n"))
                    {
                        hasNewLine = true;
                        break;
                    }
                }
                string separator = hasNewLine ? "\n|\n" : "\n";
                _jobject[key] = JToken.FromObject(string.Join(separator, list));
            }
            catch
            {
            }
        }


        /// <summary>
        /// Xóa thuộc tính khỏi đối tượng JObject.
        /// </summary>
        /// <param name="key">Khóa (key) của thuộc tính cần xóa.</param>
        public void Delete(string key)
        {
            try
            {
                _jobject.Remove(key);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Lưu đối tượng JObject thành một tệp tin JSON.
        /// </summary>
        /// <param name="filePath">Đường dẫn và tên tệp tin để lưu JSON (tùy chọn).</param>
        public void SaveJsonToFile(string filePath = "")
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = configurationFile;
                }
                File.WriteAllText(filePath, _jobject.ToString());
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        /// <summary>
        /// Trả về chuỗi JSON đại diện cho đối tượng JObject.
        /// </summary>
        /// <returns>Chuỗi JSON đại diện cho đối tượng JObject.</returns>
        public string GetJsonString()
        {
            string result = "";
            try
            {
                result = _jobject.ToString().Replace("\r\n", "");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return result;
        }

    }
}
