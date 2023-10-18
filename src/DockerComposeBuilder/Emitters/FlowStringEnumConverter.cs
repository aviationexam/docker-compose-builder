using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace DockerComposeBuilder.Emitters
{
    public class FlowStringEnumConverter : ChainedEventEmitter
    {
        public FlowStringEnumConverter(
            IEventEmitter nextEmitter
        ) : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.Type is { IsEnum: true } sourceType && eventInfo.Source.Value is { } value)
            {
                var enumMember = sourceType.GetMember(value.ToString()!).FirstOrDefault();
                var yamlValue = enumMember?.GetCustomAttributes<EnumMemberAttribute>(true).Select(ema => ema.Value).FirstOrDefault() ?? value.ToString();

                eventInfo = new ScalarEventInfo(new ObjectDescriptor(
                    yamlValue,
                    typeof(string),
                    typeof(string),
                    eventInfo.Source.ScalarStyle
                ));
            }

            nextEmitter.Emit(eventInfo, emitter);
        }
    }
}
