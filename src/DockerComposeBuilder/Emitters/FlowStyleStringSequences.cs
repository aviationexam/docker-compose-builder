using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace DockerComposeBuilder.Emitters;

public class FlowStyleStringSequences(
    IEventEmitter nextEmitter
) : ChainedEventEmitter(nextEmitter)
{
    public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
    {
        if (typeof(string[]) == eventInfo.Source.Type)
        {
            eventInfo = new SequenceStartEventInfo(eventInfo.Source)
            {
                Style = SequenceStyle.Flow,
            };
        }

        nextEmitter.Emit(eventInfo, emitter);
    }
}
