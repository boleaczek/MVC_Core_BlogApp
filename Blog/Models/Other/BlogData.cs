﻿using Newtonsoft.Json;
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
        public string PhoneNumber { get; set; }
        public string ImagePath { get; set; }
        public string IconPath { get; set; }

        public BlogData()
        {
            Dictionary<string,string> dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(System.IO.File.ReadAllText(@"BlogData.json"));
            BlogName = dict["BlogName"];
            AuthorName = dict["AuthorName"];
            MailAddress = dict["MailAddress"];
            Description = dict["Description"];
            PhoneNumber = dict["PhoneNumber"];
            ImagePath = dict["ImagePath"];
            IconPath = dict["IconPath"];
        }

        public void SaveData(BlogData newData)
        {
            System.IO.File.WriteAllText(@"BlogData.json", JsonConvert.SerializeObject(newData));
        }
    }
}
