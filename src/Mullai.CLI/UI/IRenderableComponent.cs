using Spectre.Console.Rendering;

namespace Mullai.CLI.UI;

public interface IRenderableComponent
{
    IRenderable Render();
}
