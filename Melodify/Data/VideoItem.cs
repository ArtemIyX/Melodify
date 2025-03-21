using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodify.Data;

public class VideoItem
{
    public string Thumbnail { get; set; }
    public string Title { get; set; }
    public string Channel { get; set; }
    public string Views { get; set; }
    public string Duration { get; set; }

    public string VideoId { get; set; }

}
