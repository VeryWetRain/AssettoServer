using System.Collections.Generic;
using AssettoServer.Shared.Model;
using AssettoServer.Utils;
using IniParser;
using IniParser.Model;
using JetBrains.Annotations;

namespace AssettoServer.Server.Configuration.Kunos;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public class Sections
{
    [IniSection("SECTION")] public IReadOnlyList<Section> Zones { get; init; } = new List<Section>();

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class Section : ISection
    {
        [IniField("IN")] public double In { get; init; }
        [IniField("OUT")] public double Out { get; init; }
        [IniField("TEXT")] public string Text { get; init; }
    }

    public static Sections FromFile(string path)
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(path);
        return data.DeserializeObject<Sections>();
    }
}
