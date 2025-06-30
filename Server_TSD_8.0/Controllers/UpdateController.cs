using System;
using System.IO;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server_TSD.Models;

namespace Server_TSD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        string PathApp = "app/app-debug.apk";

        [HttpGet("[action]")]
        public IActionResult GetApk()
        {            
            //Проверить есть ли такой файл
            if (!System.IO.File.Exists(PathApp))
                return NotFound();

            // Путь к файлу
            string full_path = System.IO.Path.GetFullPath(PathApp);
            // Тип файла - content-type
            string file_type = "application/apk";
            // Имя файла - необязательно
            string file_name = "app-debug.apk";

            return PhysicalFile(full_path, file_type, file_name);
        }

        [HttpGet("[action]")]
        public string curVersion()
        {
            bool isOutputMeta = false;
            string version = String.Empty;
            
            string path = "app/output.json";

            //Проверить есть ли такой файл
            if (!System.IO.File.Exists(path))
            {
                path = "app/output-metadata.json";
                //return version;
                //
                if (!System.IO.File.Exists(path))
                {
                    return version;
                }
                else
                {
                    isOutputMeta = true;
                }
            }

            var JsonString = System.IO.File.ReadAllText(path);

            if (!isOutputMeta)
            {
                JArray myJObject = JArray.Parse(JsonString);
                foreach (JObject item in myJObject)
                {
                    var itemProperties = item.SelectToken("apkData");
                    version = itemProperties.SelectToken("versionName").ToString();
                    break;
                }
            }
            else
            {
                var json = JsonConvert.DeserializeObject<output_metadata>(JsonString);

                foreach(var item in json.elements)
                {
                    version = item.versionName;
                }
            }            

            return version;
        }
    }
}