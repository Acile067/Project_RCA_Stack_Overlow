using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackOverflowService.DTOs
{
    public class CreateVoteDto
    {
        public string AnswerId { get; set; }
        public string VotedByEmail { get; set; }
    }
}