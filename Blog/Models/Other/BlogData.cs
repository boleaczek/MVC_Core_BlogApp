using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.Other
{
    public class BlogData : IBlogResource
    {
        public string BlogName { get; set; }
        public string AuthorName { get; set; }
        public string MailAddress { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string IconPath { get; set; }
        public string LongDescription { get; set; }

        IHostingEnvironment environment;

        public BlogData(IHostingEnvironment env)
        {
            environment = env;
            Dictionary<string,string> dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(System.IO.File.ReadAllText($"{environment.WebRootPath}\\BlogData.json"));
            BlogName = dict["BlogName"];
            AuthorName = dict["AuthorName"];
            MailAddress = dict["MailAddress"];
            Description = dict["Description"];
            LongDescription = dict["LongDescription"];
        }

        public void SaveData(BlogData newData)
        {
            System.IO.File.WriteAllText($"{environment.WebRootPath}\\BlogData.json", JsonConvert.SerializeObject(newData));
        }
    }
}
