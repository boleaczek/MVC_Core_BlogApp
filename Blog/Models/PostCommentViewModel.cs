﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostCommentViewModel
    {
        public Post Post { get; set; }
        public Comment Comment { get; set; }
    }
}
