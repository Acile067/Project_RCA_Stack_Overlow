﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.DTOs
{
    public class CreateAnswerDto
    {
        public string QuestionId { get; set; }
        public string Description { get; set; }

    }
}