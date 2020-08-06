using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TweekBook.Domain
{
    public class PostTag
    {
        [ForeignKey(nameof(TagName))]
        public virtual Tags Tag { get; set; }

        public string TagName { get; set; }

        public virtual Post Post { get; set; }

        public Guid PostId { get; set; }
    }
}
