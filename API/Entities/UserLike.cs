using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    //UserLike is Joint table  which will have userSourceId,UserLikeID
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        //FK
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; }
        ////FK
        public int LikedUserId { get; set; }

    }
}
