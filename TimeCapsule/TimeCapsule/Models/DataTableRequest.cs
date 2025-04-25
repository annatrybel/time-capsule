using Microsoft.AspNetCore.Mvc;

namespace TimeCapsule.Models
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        [FromForm(Name = "search[value]")]
        public string SearchValue { get; set; }

        [FromForm(Name = "order[0][column]")]
        public int OrderColumn { get; set; }

        [FromForm(Name = "order[0][dir]")]
        public string OrderDir { get; set; }
    }

}
