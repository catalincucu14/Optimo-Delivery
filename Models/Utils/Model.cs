using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.Utils
{
    public class Model<T>
    {
        /// <summary>
        /// Function used to clone an object
        /// </summary>
        public T Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        /// <summary>
        /// Function used to cast the object to JSON
        /// </summary>
        public StringContent Json()
        {
            string json = JsonConvert.SerializeObject(this);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Function used to manually validate the model
        /// </summary>
        public List<string> Validate()
        {
            ValidationContext context = new(this, serviceProvider: null, items: null);

            List<ValidationResult> validationResults = new();

            Validator.TryValidateObject(this, context, validationResults: validationResults, validateAllProperties: true);

            return validationResults
                .Select(x => x.ErrorMessage)
                .ToList();
        }
    }
}
