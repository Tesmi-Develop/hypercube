using Hypercube.Core.Graphics.Resources;
using Hypercube.Core.Resources;

namespace Hypercube.Core.Graphics.Fonts;

public interface IFontStorage
{
    Font Resolve(ResourcePath path, int size);
}