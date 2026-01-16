using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace DockerComposeBuilder.Emitters;

public class ForceQuotedStringValuesEventEmitter : ChainedEventEmitter
{
    private readonly Stack<EmitterState> _state = new();

    public ForceQuotedStringValuesEventEmitter(
        IEventEmitter nextEmitter
    ) : base(nextEmitter)
    {
        _state.Push(new EmitterState(EmitterState.EventType.Root));
    }

    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
        var item = _state.Peek();
        var shouldApply = item.ShouldApplyAndToggle();

        if (shouldApply && eventInfo.Source.Type == typeof(string))
        {
            eventInfo = new ScalarEventInfo(eventInfo.Source)
            {
                Style = ScalarStyle.DoubleQuoted,
            };
        }

        base.Emit(eventInfo, emitter);
    }

    public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
    {
        _state.Peek().ConsumeValue();
        _state.Push(new EmitterState(EmitterState.EventType.Mapping));
        base.Emit(eventInfo, emitter);
    }

    public override void Emit(MappingEndEventInfo eventInfo, IEmitter emitter)
    {
        var item = _state.Pop();
        if (item.Type != EmitterState.EventType.Mapping)
        {
            throw new Exception();
        }

        base.Emit(eventInfo, emitter);
    }

    public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
    {
        _state.Peek().ConsumeValue();
        _state.Push(new EmitterState(EmitterState.EventType.Sequence));
        base.Emit(eventInfo, emitter);
    }

    public override void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter)
    {
        var item = _state.Pop();
        if (item.Type != EmitterState.EventType.Sequence)
        {
            throw new Exception();
        }

        base.Emit(eventInfo, emitter);
    }

    private sealed class EmitterState(
        EmitterState.EventType eventType
    )
    {
        public EventType Type { get; } = eventType;

        private bool _isAtKey = true;

        public bool ShouldApplyAndToggle()
        {
            if (Type == EventType.Mapping)
            {
                var wasAtKey = _isAtKey;
                _isAtKey = !_isAtKey;
                return !wasAtKey;
            }

            return Type == EventType.Sequence;
        }

        public void ConsumeValue()
        {
            if (Type == EventType.Mapping)
            {
                _isAtKey = true;
            }
        }

        public enum EventType : byte
        {
            Root,
            Mapping,
            Sequence,
        }
    }
}
