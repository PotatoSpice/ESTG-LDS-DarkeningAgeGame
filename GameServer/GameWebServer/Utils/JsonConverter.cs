using Newtonsoft.Json;
using System.Collections.Generic;

// The purpose of this class is to pick the methods refering to data on the graph and export it via JSON. 
//  The idea is to make EACH method more and more Atomized and smaller
// Why use this as with generic data types?
//  Since it returns a string with json format, it will work for any certain Object we might throw, 
//  more dinamically, which is important
// This means this code is far more adaptable and reusable.
namespace GameWebServer.Utils
{
    public static class JsonConverter<T>
    {

        public static string convertListToJsonString(List<T> objectList)
        {
            return JsonConvert.SerializeObject(objectList);
        }
        
        public static string convertObjectToJsonString(T paramObject)
        {
            return JsonConvert.SerializeObject(paramObject);
        }

    }
}
