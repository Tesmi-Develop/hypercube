using System.Text;
using Silk.NET.OpenAL;

namespace Hypercube.Core.Audio.Api.Realisations.OpenAl;

public partial class OpenAlAudioApi
{
    public override string Info
    {
        get
        {
            if (!Ready)
                return string.Empty;
                
            var builder = new StringBuilder();
            builder.Append("Vendor: ");
            builder.AppendLine(_al.GetStateProperty(StateString.Vendor));
            builder.Append("Renderer: ");
            builder.AppendLine(_al.GetStateProperty(StateString.Renderer));
            builder.Append("Version: ");
            builder.AppendLine(_al.GetStateProperty(StateString.Version));
            builder.Append("Extensions: ");
            builder.Append(_al.GetStateProperty(StateString.Extensions));
            return builder.ToString();
        }
    }
}