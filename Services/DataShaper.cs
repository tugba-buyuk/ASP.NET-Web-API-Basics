using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        public PropertyInfo[] Properties { get; set; }
        public DataShaper()
        {
            Properties= typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);   
        }
        public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldString)
        {
            var requiredProperties= GetRequiredProperties(fieldString);
            return FetchData(entities, requiredProperties);
        }

        public ExpandoObject ShapeData(T entity, string fieldString)
        {
            var requiredProperties = GetRequiredProperties(fieldString);
            return FetchDataForEntity(entity, requiredProperties);
        }
    
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldString)
        {
            var requiredFields= new List<PropertyInfo>();
            if (!string.IsNullOrEmpty(fieldString))
            {
                var fields=fieldString.Split(','
                    ,StringSplitOptions.RemoveEmptyEntries);

                foreach(var field in fields)
                {
                    var property= Properties
                        .FirstOrDefault(pi=>pi.Name.Equals(field.Trim(),
                        StringComparison.InvariantCultureIgnoreCase));
                    if (property is null)
                        continue;
                    requiredFields.Add(property);
                }   
            }
            else
            {
                requiredFields = Properties.ToList();
            }
            return requiredFields;

        }
    
        private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ExpandoObject();

            foreach(var propert in requiredProperties)
            {
                var objectPropertyValue = propert.GetValue(entity);
                shapedObject.TryAdd(propert.Name, objectPropertyValue);
            }
            return shapedObject;
        }
    
        private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData= new List<ExpandoObject>();
            foreach(var entity in entities)
            {
                var shapedObject= FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
        
    
    
    }
}
