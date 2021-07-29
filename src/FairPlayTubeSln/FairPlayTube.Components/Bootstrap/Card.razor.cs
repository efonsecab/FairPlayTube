using Microsoft.AspNetCore.Components;

namespace FairPlayTube.Components.Bootstrap
{
    public partial class Card
    {
        [Parameter]
        public RenderFragment CardHeader { get; set; }
        [Parameter]
        public RenderFragment CardBody { get; set; }
        [Parameter]
        public RenderFragment CardFooter { get; set; }
        [Parameter]
        public string Width { get; set; } = "300px";
    }
}
